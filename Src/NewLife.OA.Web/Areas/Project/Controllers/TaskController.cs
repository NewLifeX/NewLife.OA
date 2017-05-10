﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using NewLife.Collections;
using NewLife.Cube;
using NewLife.Log;
using NewLife.Web;
using XCode;
using XCode.Membership;

namespace NewLife.OA.Web.Areas.Project.Controllers
{
    [DisplayName("任务")]
    public class TaskController : EntityController<WorkTask>
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.HeaderTitle = "";
            ViewBag.HeaderContent = "";

            base.OnActionExecuting(filterContext);
        }

        protected override ActionResult IndexView(Pager p)
        {
            ViewBag.Page = p;

            var ps = WebHelper.Params;
            if (!ps.ContainsKey("Status")) ps["Status"] = new TaskStatus[] { TaskStatus.进行中 }.Cast<Int32>().Join();

            var list = GetList(p, 0, false, 1);

            return View("Index", list);
        }

        /// <summary>增加新的独立菜单</summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [DisplayName("所有任务")]
        [EntityAuthorize(PermissionFlags.Detail | PermissionFlags.Insert | PermissionFlags.Update | PermissionFlags.Delete, ResourceName = "所有任务")]
        public ActionResult Show(Pager p)
        {
            ViewBag.Page = p;

            var ps = WebHelper.Params;
            if (!ps.ContainsKey("Status")) ps["Status"] = new TaskStatus[] { TaskStatus.计划, TaskStatus.进行中, TaskStatus.暂停 }.Cast<Int32>().Join();

            var list = GetList(p, 0, null, 2);

            return View("Index", list);
        }

        /// <summary>我的任务，增加新的独立菜单</summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [DisplayName("我的任务")]
        [EntityAuthorize(PermissionFlags.Detail | PermissionFlags.Insert | PermissionFlags.Update | PermissionFlags.Delete, ResourceName = "我的任务")]
        public ActionResult My(Pager p)
        {
            ViewBag.Page = p;

            var ps = WebHelper.Params;

            // 我的任务支持查看他人视图
            var masterid = ps["masterid"].ToInt();
            if (masterid <= 0)
            {
                masterid = ManageProvider.User.ID;
                ps["masterid"] = masterid.ToString();
            }
            if (!ps.ContainsKey("Status")) ps["Status"] = new TaskStatus[] { TaskStatus.计划, TaskStatus.进行中, TaskStatus.暂停 }.Cast<Int32>().Join();

            var list = GetList(p, masterid, false, 3);

            return View("Index", list);
        }

        /// <summary>增加新的独立菜单</summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [DisplayName("全部任务")]
        [EntityAuthorize(PermissionFlags.Detail | PermissionFlags.Insert | PermissionFlags.Update | PermissionFlags.Delete, ResourceName = "全部任务")]
        public ActionResult All(Pager p)
        {
            ViewBag.Page = p;

            var ps = WebHelper.Params;
            if (!ps.ContainsKey("Status")) ps["Status"] = new TaskStatus[] { TaskStatus.计划, TaskStatus.进行中 }.Cast<Int32>().Join();

            var list = GetList(p, 0, false, 1);

            return View("Index", list);
        }

        private EntityList<WorkTask> GetList(Pager p, Int32 masterid, Boolean? deleted, Int32 expand)
        {
            var pid = RouteData.Values["id"].ToInt();

            var ps = WebHelper.Params;
            var sts = ps["status"].SplitAsInt().Select(e => (TaskStatus)e).ToArray();
            var tps = ps["Priority"].SplitAsInt().Select(e => (TaskPriorities)e).ToArray();
            if (masterid == 0) masterid = ps["masterid"].ToInt();

            var start = ps["dtStart"].ToDateTime();
            var end = ps["dtEnd"].ToDateTime();

            // 如果不扩展，则显示所有任务
            if (pid == 0 && expand > 1) pid = -1;

            var list = WorkTask.Search(pid, sts, tps, masterid, start, end, deleted, p["Q"], p);

            // 扩展任务树
            if (expand == 1)
                list = WorkTask.Expand(list, sts, tps, masterid, start, end, deleted, p["Q"]);
            else if (expand == 3)
                list = WorkTask.ExpandParent(list);

            return list;
        }

        static DictionaryCache<Int32, Int32> _cacheView = new DictionaryCache<int, int>();
        /// <summary>表单页视图。子控制器可以重载，以传递更多信息给视图，比如修改要显示的列</summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override ActionResult FormView(WorkTask entity)
        {
            if (entity.ID == 0)
            {
                // 如果是新增任务，那么路由的id就是父任务ID，这里处理一下
                entity.ParentID = Request["ParentID"].ToInt();

                // 默认今天开始，一天结束
                entity.PlanStartTime = DateTime.Now.Date;
                entity.PlanEndTime = DateTime.Now.Date.AddDays(1);
            }
            else
            {
                // 增加浏览数。借助缓存，避免重复更新
                _cacheView.GetItem(entity.ID, k => entity.Views++);
                //entity.Views++;

                // 有些不合格的数据可能保存失败
                try
                {
                    entity.SaveWithoutValid();
                }
                catch (Exception ex)
                {
                    XTrace.WriteException(ex);
                }
            }

            return base.FormView(entity);
        }

        public override ActionResult Add(WorkTask entity)
        {
            // 如果是新增任务，那么路由的id就是父任务ID，这里处理一下
            entity.ParentID = Request["ParentID"].ToInt();
            //entity.ID = 0;

            return base.Add(entity);
        }

        protected override int OnInsert(WorkTask entity)
        {
            WebHelper.Params["SrcTaskID"] = entity.ID + "";
            using (var trans = WorkTask.Meta.CreateTrans())
            {
                var rs = base.OnInsert(entity);

                // 上下修正积分
                entity.FixScore(true, true);

                // 重新计算积分比重
                entity.FixPercent();

                // 修正父任务进度
                entity.FixParentProgress();

                trans.Commit();

                return rs;
            }
        }

        protected override int OnUpdate(WorkTask entity)
        {
            WebHelper.Params["SrcTaskID"] = entity.ID + "";
            using (var trans = WorkTask.Meta.CreateTrans())
            {
                // 如果改变了积分，则上下一起修正
                if ((entity as IEntity).Dirtys[WorkTask._.Score]) entity.FixScore(true, true);

                // 重新计算积分比重
                entity.FixPercent();

                var rs = base.OnUpdate(entity);

                // 修正父任务进度
                entity.FixParentProgress();

                trans.Commit();

                return rs;
            }
        }

        protected override int OnDelete(WorkTask entity)
        {
            WebHelper.Params["SrcTaskID"] = entity.ID + "";
            using (var trans = WorkTask.Meta.CreateTrans())
            {
                var rs = base.OnDelete(entity);

                if (entity.Deleted)
                    // 向上修正积分，因为子孙任务会一起删除，所以
                    entity.FixScore(true, false);
                else
                    entity.FixScore(true, true);

                // 重新计算积分比重
                entity.FixPercent();

                // 修正父任务进度
                entity.FixParentProgress();

                trans.Commit();

                return rs;
            }
        }

        public ActionResult SetStatus(Int32? id, TaskStatus status)
        {
            var url = Request.UrlReferrer + "";
            //if (id == null) return Redirect(url);

            var entity = WorkTask.FindByID(id ?? 0);
            if (entity == null)
            {
                Js.Alert("非法参数", null, 5000).Redirect(url);
                return new EmptyResult();
            }

            WebHelper.Params["SrcTaskID"] = entity.ID + "";

            var msg = "";
            using (var trans = WorkTask.Meta.CreateTrans())
            {
                var ori = entity.TaskStatus;

                try
                {
                    msg = entity.SetStatus(status) ?? "成功";

                    // 取消时删除任务，重新计算积分
                    if (status == TaskStatus.取消 || ori == TaskStatus.取消)
                        OnDelete(entity);
                    else
                        entity.Update();

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    //msg = ex.Message;
                    ModelState.AddModelError("", ex.Message);
                }
            }

            //Js.Alert(msg, null, 2000).Redirect(url);

            //return new EmptyResult();
            return FormView(entity);
        }
    }
}