/*
 * XCoder v6.2.5498.35992
 * 作者：topyms/YMS-PC
 * 时间：2015-01-20 20:08:12
 * 版权：版权所有 (C) 新生命开发团队 2002~2015
*/
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using NewLife.Web;
using XCode;
using XCode.Membership;

namespace NewLife.OA
{
    /// <summary>任务评论</summary>
    public partial class TaskComment : UserTimeEntity<TaskComment>
    {
        #region 对象操作﻿

        /// <summary>验证数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew"></param>
        public override void Valid(Boolean isNew)
        {
            // 这里验证参数范围，建议抛出参数异常，指定参数名，前端用户界面可以捕获参数异常并聚焦到对应的参数输入框
            //if (String.IsNullOrEmpty(Name)) throw new ArgumentNullException(_.Name, _.Name.DisplayName + "无效！");
            //if (!isNew && ID < 1) throw new ArgumentOutOfRangeException(_.ID, _.ID.DisplayName + "必须大于0！");

            // 建议先调用基类方法，基类方法会对唯一索引的数据进行验证
            base.Valid(isNew);

            if (!Dirtys[__.IP]) IP = WebHelper.UserHost;
        }

        public static Int32 FindCountByTaskID(Int32 taskid)
        {
            if (Meta.Count >= 1000)
                return FindCount(__.TaskID, taskid);
            else
                return Meta.Cache.Entities.ToList().Count(e => e.TaskID == taskid);
        }
        #endregion

        #region 扩展属性﻿
        /// <summary>关联任务</summary>
        [Map(__.TaskID, typeof(WorkTask), "ID")]
        public WorkTask Task { get { return WorkTask.FindByID(TaskID); } }

        /// <summary>任务名称</summary>
        [DisplayName("任务名称")]
        public String TaskName { get { var task = Task; return task != null ? task.Name : null; } }

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
        public static EntityList<TaskComment> FindAllByTaskID(Int32 worktaskid)
        {
            if (Meta.Count >= 1000)
                return FindAll(_.TaskID, worktaskid);
            else // 实体缓存
                return Meta.Cache.Entities.FindAll(__.TaskID, worktaskid);
        }
        #endregion

        #region 高级查询
        public static EntityList<TaskComment> Search(Int32 taskid, Pager p)
        {
            var exp = _.TaskID == taskid;

            return FindAll(exp, p);
        }
        #endregion

        #region 扩展操作
        #endregion

        #region 业务
        #endregion
    }
}