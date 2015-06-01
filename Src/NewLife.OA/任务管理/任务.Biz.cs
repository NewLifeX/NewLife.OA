﻿/*
 * XCoder v6.2.5498.35992
 * 作者：topyms/YMS-PC
 * 时间：2015-01-20 20:08:12
 * 版权：版权所有 (C) 新生命开发团队 2002~2015
*/
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NewLife.Web;
using XCode;
using XCode.Configuration;
using XCode.Membership;

namespace NewLife.OA
{
    /// <summary>任务状态</summary>
    public enum TaskStatus
    {
        准备 = 0,
        进行 = 1,
        暂停 = 2,
        取消 = 3,
        完成 = 4
    }

    /// <summary>任务优先级</summary>
    public enum TaskPriorities
    {
        空闲,
        低级,
        普通,
        高级,
        实时
    }

    /// <summary>任务</summary>
    public partial class WorkTask : UserTimeEntity<WorkTask>
    {
        #region 对象操作﻿
        protected override WorkTask CreateInstance(bool forEdit = false)
        {
            var entity = base.CreateInstance(forEdit);
            if (forEdit)
            {
                entity.CreateUserID = ManageProvider.User.ID;
                entity.CreateTime = DateTime.Now;
            }
            return entity;
        }

        /// <summary>验证数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew"></param>
        public override void Valid(Boolean isNew)
        {
            if (!HasDirty) return;

            if (String.IsNullOrEmpty(Name)) throw new ArgumentNullException(_.Name, _.Name.DisplayName + "不能为空！");
            if (MasterID <= 0) throw new ArgumentNullException(_.MasterID, _.MasterID.DisplayName + "不能为空！");
            if (PlanStartTime <= DateTime.MinValue) throw new ArgumentNullException(_.PlanStartTime, _.PlanStartTime.DisplayName + "不能为空！");
            if (PlanEndTime <= DateTime.MinValue) throw new ArgumentNullException(_.PlanEndTime, _.PlanEndTime.DisplayName + "不能为空！");

            if (PlanStartTime > PlanEndTime) throw new ArgumentOutOfRangeException(_.PlanEndTime, _.PlanEndTime.DisplayName + "不能大于开始时间！");

            // 建议先调用基类方法，基类方法会对唯一索引的数据进行验证
            base.Valid(isNew);

            // 计算计划工作日，采取进一法
            PlanCost = (Int32)Math.Ceiling((PlanEndTime - PlanStartTime).TotalDays);

            // 计算实际工作日
            if (EndTime > DateTime.MinValue && StartTime > DateTime.MinValue) Cost = (Int32)Math.Ceiling((EndTime - StartTime).TotalDays);

            // 计算ChildCount
            if (Parent != null)
            {
                Parent.ChildCount = FindCountByParentID(ParentID);
                Parent.Save();
            }

            // 不管如何，都修正本级子节点数
            ChildCount = FindCountByParentID(ID);
        }

        WorkTask _bak;
        protected override void OnLoad()
        {
            base.OnLoad();

            _bak = this.CloneEntity();
        }

        protected override int OnInsert()
        {
            var rs = base.OnInsert();

            TaskHistory.Add(ID, "创建", null, Name);

            return rs;
        }

        protected override int OnUpdate()
        {
            if (Deleted) throw new Exception("任务已删除，禁止更新操作！");

            WriteHistory();

            // 更新历史数和评论数
            Historys = TaskHistory.FindCountByTaskID(ID);
            Comments = TaskComment.FindCountByTaskID(ID);

            var rs = base.OnUpdate();

            return rs;
        }

        protected override int OnDelete()
        {
            //var rs = base.OnDelete();
            var ori = Deleted;
            Deleted = !Deleted;
            var rs = base.OnUpdate();

            TaskHistory.Add(ID, ori ? "恢复" : "删除", null, Name);

            return rs;
        }
        #endregion

        #region 扩展属性﻿
        //public IManageUser Creater { get { return ManageProvider.Provider.FindByID(CreateUserID); } }

        //[DisplayName("创建人")]
        //public String CreateName { get { return Creater == null ? "" : Creater.ToString(); } }

        //public IManageUser Updater { get { return ManageProvider.Provider.FindByID(UpdateUserID); } }

        //[DisplayName("更新人")]
        //public String UpdateName { get { return Updater == null ? "" : Updater.ToString(); } }

        private WorkTask _Parent;
        /// <summary>父任务</summary>
        public WorkTask Parent
        {
            get
            {
                if (_Parent == null && ParentID > 0 && !Dirtys.ContainsKey("Parent"))
                {
                    _Parent = FindByID(ParentID);
                    Dirtys["Parent"] = true;
                }
                return _Parent;
            }
        }

        /// <summary>父任务名</summary>
        [DisplayName("父任务")]
        public String ParentName { get { return Parent != null ? Parent.Name : null; } }

        /// <summary>深度</summary>
        public Int32 Deepth { get { return Parent != null ? Parent.Deepth + 1 : 1; } }

        /// <summary>树形节点名，根据深度带全角空格前缀</summary>
        [DisplayName("任务名")]
        public virtual String TreeNodeText
        {
            get
            {
                Int32 d = Deepth;
                if (d <= 0) return "根";

                //return new String('　', d) + "" + Name;
                var sb = new StringBuilder();
                for (int i = 0; i < d - 1; i++)
                {
                    sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
                }
                if (d > 1) sb.Append("|- ");
                sb.Append(Name);
                return sb.ToString();
            }
        }

        private UserX _Master;
        /// <summary>负责人</summary>
        public UserX Master
        {
            get
            {
                if (_Master == null && MasterID > 0 && !Dirtys.ContainsKey("Master"))
                {
                    _Master = UserX.FindByID(MasterID);
                    Dirtys["Master"] = true;
                }
                return _Master;
            }
        }

        /// <summary>负责人</summary>
        [DisplayName("负责人")]
        public String MasterName { get { return Master != null ? Master.ToString() : null; } }

        /// <summary>任务状态</summary>
        [DisplayName("状态")]
        public TaskStatus TaskStatus { get { return (TaskStatus)Status; } set { Status = (Int32)value; } }

        /// <summary>任务优先级</summary>
        [DisplayName("优先级")]
        public TaskPriorities TaskPriority { get { return (TaskPriorities)Priority; } set { Priority = (Int32)value; } }

        private EntityList<TaskMember> _Members;
        /// <summary>成员</summary>
        [DisplayName("成员")]
        public EntityList<TaskMember> Members
        {
            get
            {
                if (_Members == null && ID > 0 && !Dirtys.ContainsKey("Members"))
                {
                    _Members = TaskMember.FindAllByWorkTaskIDAndKind(ID, MemberKinds.成员);
                    Dirtys["Members"] = true;
                }
                return _Members;
            }
        }

        private Int32[] _MemberIDs;
        /// <summary>成员</summary>
        [DisplayName("成员")]
        public Int32[] MemberIDs
        {
            get
            {
                if (_MemberIDs == null) _MemberIDs = Members == null ? new Int32[0] : Members.ToList().Select(m => m.MemberID).ToArray();
                return _MemberIDs;
            }
            set
            {
                _MemberIDs = value;

                _Members = null;
                Dirtys.Remove("Members");
            }
        }
        #endregion

        #region 扩展查询﻿
        public static WorkTask FindByID(Int32 id)
        {
            if (Meta.Count < 1000) return Meta.Cache.Entities.Find(__.ID, id);

            return Meta.SingleCache[id];
        }

        /// <summary>根据名称查找</summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static EntityList<WorkTask> FindAllByName(String name)
        {
            if (Meta.Count >= 1000)
                return FindAll(_.Name, name);
            else // 实体缓存
                return Meta.Cache.Entities.FindAll(__.Name, name);
        }

        public static Int32 FindCountByParentID(Int32 pid)
        {
            if (Meta.Count >= 1000)
                return FindCount(__.ParentID, pid);
            else // 实体缓存
                return Meta.Cache.Entities.ToList().Where(e => e.ParentID == pid).Count();
        }

        /// <summary>根据父任务。顶级任务的父任务为0查找</summary>
        /// <param name="parentid">父任务。顶级任务的父任务为0</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static EntityList<WorkTask> FindAllByParentID(Int32 parentid)
        {
            if (Meta.Count >= 1000)
                return FindAll(_.ParentID, parentid);
            else // 实体缓存
                return Meta.Cache.Entities.FindAll(__.ParentID, parentid);
        }

        /// <summary>根据负责人查找</summary>
        /// <param name="masterid">负责人</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static EntityList<WorkTask> FindAllByMasterID(Int32 masterid)
        {
            if (Meta.Count >= 1000)
                return FindAll(_.MasterID, masterid);
            else // 实体缓存
                return Meta.Cache.Entities.FindAll(__.MasterID, masterid);
        }

        /// <summary>根据状态。准备、进行、暂停、取消、完成查找</summary>
        /// <param name="status">状态。准备、进行、暂停、取消、完成</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static EntityList<WorkTask> FindAllByStatus(Int32 status)
        {
            if (Meta.Count >= 1000)
                return FindAll(_.Status, status);
            else // 实体缓存
                return Meta.Cache.Entities.FindAll(__.Status, status);
        }
        #endregion

        #region 高级查询
        public static EntityList<WorkTask> Search(Int32 pid, TaskStatus[] status, TaskPriorities[] tps, Int32 masterid, DateTime start, DateTime end, Boolean? deleted, String key, Pager p)
        {
            var exp = SearchWhereByKeys(key);
            if (pid >= 0) exp &= _.ParentID == pid;
            if (status != null) exp &= _.Status.In(status);
            if (tps != null) exp &= _.Priority.In(tps);
            if (masterid > 0) exp &= _.MasterID == masterid;

            if (start > DateTime.MinValue) exp &= _.PlanStartTime >= start;
            if (end > DateTime.MinValue) exp &= _.PlanEndTime < end.Date.AddDays(1);

            exp &= _.Deleted.IsTrue(deleted);

            // 默认按照最后更新时间排序
            if (p.Sort.IsNullOrEmpty()) p.Sort = _.UpdateTime.Desc();

            return FindAll(exp, p);
        }

        /// <summary>扩展任务的子孙节点</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static EntityList<WorkTask> Expand(IEnumerable<WorkTask> collection, TaskStatus[] status, TaskPriorities[] tps, Int32 masterid, DateTime start, DateTime end, Boolean? deleted, String key)
        {
            if (status == null) status = new TaskStatus[0];
            if (tps == null) tps = new TaskPriorities[0];

            var list = new EntityList<WorkTask>();
            foreach (var item in collection)
            {
                // 过滤
                if (status.Length > 0 && !status.Contains(item.TaskStatus)) continue;
                if (tps.Length > 0 && !tps.Contains(item.TaskPriority)) continue;
                if (masterid > 0 && item.MasterID != masterid) continue;
                if (start > DateTime.MinValue && item.PlanStartTime < start) continue;
                if (end > DateTime.MinValue && item.PlanEndTime >= end.Date.AddDays(1)) continue;
                if (deleted != null && item.Deleted != deleted.Value) continue;
                if (!key.IsNullOrEmpty() && !item.Name.Contains(key) && !item.Content.Contains(key)) continue;

                list.Add(item);
                if (item.ChildCount > 0)
                {
                    var childs = FindAllByParentID(item.ID);
                    if (childs.Count > 0) list.AddRange(Expand(childs, status, tps, masterid, start, end, deleted, key));
                }
            }

            return list;
        }
        #endregion

        #region 扩展操作
        #endregion

        #region 业务
        /// <summary>写任务历史</summary>
        void WriteHistory()
        {
            if (_bak == null) throw new Exception("非法更新任务！");

            // 找到旧有数据
            var entity = _bak;

            var names = new Field[] { _.Name, _.ParentID, _.Percent, _.PlanStartTime, _.PlanEndTime, _.Progress };
            foreach (var item in names)
            {
                if (Dirtys[item.Name])
                {
                    TaskHistory.Add(ID, item.DisplayName, _bak[item.Name], this[item.Name], Name);
                }
            }
            if (Dirtys[__.Priority]) TaskHistory.Add(ID, _.Priority.DisplayName, _bak.TaskPriority, this.TaskPriority, Name);
            if (Dirtys[__.Status]) TaskHistory.Add(ID, _.Status.DisplayName, _bak.TaskStatus, this.TaskStatus, Name);
            if (Dirtys[__.MasterID]) TaskHistory.Add(ID, _.MasterID.DisplayName, _bak.MasterName, this.MasterName, Name);
            if (Dirtys[__.Content]) TaskHistory.Add(ID, _.Content.DisplayName, _bak.Content, this.Content, Name);
        }
        #endregion
    }
}