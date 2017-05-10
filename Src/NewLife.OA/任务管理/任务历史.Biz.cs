/*
 * XCoder v6.2.5498.35992
 * 作者：topyms/YMS-PC
 * 时间：2015-01-20 20:08:12
 * 版权：版权所有 (C) 新生命开发团队 2002~2015
*/
using System;
using System.Linq;
using System.ComponentModel;
using NewLife.Web;
using XCode;
using XCode.Membership;
using System.Net;

namespace NewLife.OA
{
    /// <summary>任务历史</summary>
    public partial class TaskHistory : UserTimeEntity<TaskHistory>
    {
        #region 对象操作﻿
        /// <summary>验证数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew"></param>
        public override void Valid(Boolean isNew)
        {
            // 建议先调用基类方法，基类方法会对唯一索引的数据进行验证
            base.Valid(isNew);

            if (!Dirtys[__.IP]) IP = WebHelper.UserHost;

            if (!Dirtys[__.SrcTaskID] && WebHelper.Params.ContainsKey("SrcTaskID")) SrcTaskID = WebHelper.Params["SrcTaskID"].ToInt();
        }

        public override int Update()
        {
            throw new XException("禁止修改任务历史！");
        }

        public override int Delete()
        {
            throw new XException("禁止删除任务历史！");
        }
        #endregion

        #region 扩展属性﻿
        /// <summary>关联任务</summary>
        [Map(__.TaskID, typeof(WorkTask), "ID")]
        public WorkTask Task { get { return WorkTask.FindByID(TaskID); } }

        /// <summary>任务名称</summary>
        [DisplayName("任务名称")]
        public String TaskName { get { var task = Task; return task != null ? task.Name : null; } }

        /// <summary>源任务</summary>
        [Map(__.SrcTaskID, typeof(WorkTask), "ID")]
        public WorkTask SrcTask { get { return WorkTask.FindByID(SrcTaskID); } }

        /// <summary>源任务名称</summary>
        [DisplayName("源任务名称")]
        public String SrcTaskName { get { var task = SrcTask; return task != null ? task.Name : null; } }

        /// <summary>物理地址</summary>
        [DisplayName("物理地址")]
        public String Address
        {
            get
            {
                if (IP.IsNullOrEmpty()) return null;

                IPAddress ip = null;
                if (!IPAddress.TryParse(IP, out ip)) return null;

                return ip.GetAddress();
            }
        }
        #endregion

        #region 扩展查询﻿
        /// <summary>根据任务查找</summary>
        /// <param name="worktaskid">任务</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static EntityList<TaskHistory> FindAllByTaskID(Int32 worktaskid)
        {
            if (Meta.Count >= 1000)
                return FindAll(_.TaskID, worktaskid);
            else // 实体缓存
                return Meta.Cache.Entities.FindAll(__.TaskID, worktaskid);
        }

        public static Int32 FindCountByTaskID(Int32 taskid)
        {
            if (Meta.Count >= 1000)
                return FindCount(__.TaskID, taskid);
            else
                return Meta.Cache.Entities.ToList().Count(e => e.TaskID == taskid);
        }

        /// <summary>根据种类。状态改变，优先级改变，积分改变，成员改变查找</summary>
        /// <param name="kind">种类。状态改变，优先级改变，积分改变，成员改变</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static EntityList<TaskHistory> FindAllByKind(Int32 kind)
        {
            if (Meta.Count >= 1000)
                return FindAll(_.Kind, kind);
            else // 实体缓存
                return Meta.Cache.Entities.FindAll(__.Kind, kind);
        }

        /// <summary>根据任务、种类。状态改变，优先级改变，积分改变，成员改变查找</summary>
        /// <param name="worktaskid">任务</param>
        /// <param name="kind">种类。状态改变，优先级改变，积分改变，成员改变</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static EntityList<TaskHistory> FindAllByTaskIDAndKind(Int32 worktaskid, String kind)
        {
            if (Meta.Count >= 1000)
                return FindAll(new String[] { __.TaskID, __.Kind }, new Object[] { worktaskid, kind });
            else // 实体缓存
                return Meta.Cache.Entities.FindAll(e => e.TaskID == worktaskid && e.Kind == kind);
        }
        #endregion

        #region 高级查询
        public static EntityList<TaskHistory> Search(Int32 taskid, Pager p)
        {
            var exp = _.TaskID == taskid;

            //if (p.Sort.IsNullOrEmpty()) p.Sort = _.ID.Desc();

            return FindAll(exp, p);
        }
        #endregion

        #region 扩展操作
        #endregion

        #region 业务
        /// <summary>添加任务历史</summary>
        /// <param name="taskid"></param>
        /// <param name="kind"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static TaskHistory Add(Int32 taskid, String kind, Object oldValue, Object newValue, String remark = null)
        {
            var entity = new TaskHistory();
            entity.TaskID = taskid;
            entity.Kind = kind;
            // 采用F格式化，内部特殊处理时间日期格式化
            entity.SrcValue = "{0}".F(oldValue);
            entity.NewValue = "{0}".F(newValue);
            entity.Remark = remark;

            entity.Insert();

            return entity;
        }
        #endregion
    }
}