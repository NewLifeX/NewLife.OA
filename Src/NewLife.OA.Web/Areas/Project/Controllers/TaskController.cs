using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
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

        //static TaskController()
        //{
        //    // 过滤要显示的字段
        //    var names = "MasterName,PlanStartTime,PlanEndTime,PlanCost,StartTime,UpdateUserName,UpdateTime".Split(",");
        //    var fs = WorkTask.Meta.AllFields;
        //    var list = names.Select(e => fs.FirstOrDefault(f => f.Name.EqualIgnoreCase(e))).Where(e => e != null);
        //    //list.RemoveAll(e => !names.Contains(e.Name));
        //    ListFields.Clear();
        //    ListFields.AddRange(list);
        //}

        protected override ActionResult IndexView(Pager p)
        {
            return ListView(p, 0, false, true);
        }

        /// <summary>增加新的独立菜单</summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [DisplayName("所有任务")]
        [EntityAuthorize(PermissionFlags.Detail | PermissionFlags.Insert | PermissionFlags.Update | PermissionFlags.Delete, ResourceName = "所有任务")]
        public ActionResult Show(Pager p)
        {
            ViewBag.Page = p;

            return ListView(p, 0, null, false);
        }

        /// <summary>我的任务，增加新的独立菜单</summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [DisplayName("我的任务")]
        [EntityAuthorize(PermissionFlags.Detail | PermissionFlags.Insert | PermissionFlags.Update | PermissionFlags.Delete, ResourceName = "我的任务")]
        public ActionResult My(Pager p)
        {
            ViewBag.Page = p;

            var masterid = ManageProvider.User.ID;

            return ListView(p, masterid, false, false);
        }

        private ActionResult ListView(Pager p, Int32 masterid, Boolean? deleted, Boolean expand)
        {
            var pid = RouteData.Values["id"].ToInt();
            var sts = Request["status"].SplitAsInt().Select(e => (TaskStatus)e).ToArray();
            var tps = Request["Priority"].SplitAsInt().Select(e => (TaskPriorities)e).ToArray();
            if (masterid == 0) masterid = Request["masterid"].ToInt();

            var start = Request["dtStart"].ToDateTime();
            var end = Request["dtEnd"].ToDateTime();

            // 如果不扩展，则显示所有任务
            if (pid == 0 && !expand) pid = -1;

            var list = WorkTask.Search(pid, sts, tps, masterid, start, end, deleted, p["Q"], p);

            // 扩展任务树
            if (expand) list = WorkTask.Expand(list, sts, tps, masterid, start, end, deleted, p["Q"]);

            return View("Index", list);
        }

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
                // 增加浏览数
                entity.Views++;
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
            using (var trans = WorkTask.Meta.CreateTrans())
            {
                var rs = base.OnInsert(entity);

                // 重新计算积分比重
                entity.FixPercent();

                // 修正父任务进度
                entity.FixParentProgress();

                // 上下修正积分
                entity.FixScore(true, true);

                trans.Commit();

                return rs;
            }
        }

        protected override int OnUpdate(WorkTask entity)
        {
            using (var trans = WorkTask.Meta.CreateTrans())
            {
                // 如果改变了积分，则上下一起修正
                if ((entity as IEntity).Dirtys[WorkTask._.Score]) entity.FixScore(true, true);

                // 重新计算积分比重
                entity.FixPercent();

                // 修正父任务进度
                entity.FixParentProgress();

                var rs = base.OnUpdate(entity);

                trans.Commit();

                return rs;
            }
        }

        protected override int OnDelete(WorkTask entity)
        {
            using (var trans = WorkTask.Meta.CreateTrans())
            {
                var rs = base.OnDelete(entity);

                // 重新计算积分比重
                entity.FixPercent();

                // 修正父任务进度
                entity.FixParentProgress();

                if (entity.Deleted)
                    // 向上修正积分，因为子孙任务会一起删除，所以
                    entity.FixScore(true, false);
                else
                    entity.FixScore(true, true);

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
                Js.Alert("非法参数", null, 2, "error");
                return new EmptyResult();
            }

            using (var trans = WorkTask.Meta.CreateTrans())
            {
                var ori = entity.TaskStatus;

                entity.SetStatus(status);

                // 取消时删除任务，重新计算积分
                if (status == TaskStatus.取消 || ori == TaskStatus.取消)
                    OnDelete(entity);
                else
                    entity.Update();

                trans.Commit();
            }

            Js.Alert("成功修改状态为[{0}]".F(status), null, 1, "info");

            return new EmptyResult();
        }
    }
}