﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace NewLife.OA
{
    /// <summary>任务</summary>
    [Serializable]
    [DataObject]
    [Description("任务")]
    [BindIndex("IU_WorkTask_Name_ParentID", true, "Name,ParentID")]
    [BindIndex("IX_WorkTask_Name", false, "Name")]
    [BindIndex("IX_WorkTask_ParentID", false, "ParentID")]
    [BindIndex("IX_WorkTask_MasterID", false, "MasterID")]
    [BindIndex("IX_WorkTask_Status", false, "Status")]
    [BindRelation("ParentID", false, "WorkTask", "ID")]
    [BindRelation("MasterID", false, "User", "ID")]
    [BindRelation("CreateUserID", false, "User", "ID")]
    [BindRelation("UpdateUserID", false, "User", "ID")]
    [BindTable("WorkTask", Description = "任务", ConnName = "OA", DbType = DatabaseType.SqlServer)]
    public partial class WorkTask : IWorkTask
    {
        #region 属性
        private Int32 _ID;
        /// <summary>编号</summary>
        [DisplayName("编号")]
        [Description("编号")]
        [DataObjectField(true, true, false, 10)]
        [BindColumn(1, "ID", "编号", null, "int", 10, 0, false)]
        public virtual Int32 ID
        {
            get { return _ID; }
            set { if (OnPropertyChanging(__.ID, value)) { _ID = value; OnPropertyChanged(__.ID); } }
        }

        private String _Name;
        /// <summary>名称</summary>
        [DisplayName("名称")]
        [Description("名称")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn(2, "Name", "名称", null, "nvarchar(50)", 0, 0, true, Master=true)]
        public virtual String Name
        {
            get { return _Name; }
            set { if (OnPropertyChanging(__.Name, value)) { _Name = value; OnPropertyChanged(__.Name); } }
        }

        private Int32 _ParentID;
        /// <summary>父任务。顶级任务的父任务为0</summary>
        [DisplayName("父任务")]
        [Description("父任务。顶级任务的父任务为0")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(3, "ParentID", "父任务。顶级任务的父任务为0", null, "int", 10, 0, false)]
        public virtual Int32 ParentID
        {
            get { return _ParentID; }
            set { if (OnPropertyChanging(__.ParentID, value)) { _ParentID = value; OnPropertyChanged(__.ParentID); } }
        }

        private Int32 _ChildCount;
        /// <summary>子任务数</summary>
        [DisplayName("子任务数")]
        [Description("子任务数")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(4, "ChildCount", "子任务数", null, "int", 10, 0, false)]
        public virtual Int32 ChildCount
        {
            get { return _ChildCount; }
            set { if (OnPropertyChanging(__.ChildCount, value)) { _ChildCount = value; OnPropertyChanged(__.ChildCount); } }
        }

        private Int32 _Score;
        /// <summary>积分。全局权重，可用于绩效考核</summary>
        [DisplayName("积分")]
        [Description("积分。全局权重，可用于绩效考核")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(5, "Score", "积分。全局权重，可用于绩效考核", null, "int", 10, 0, false)]
        public virtual Int32 Score
        {
            get { return _Score; }
            set { if (OnPropertyChanging(__.Score, value)) { _Score = value; OnPropertyChanged(__.Score); } }
        }

        private Boolean _LockScore;
        /// <summary>积分锁定。锁定后，下一级子任务积分总和不能超过当前任务</summary>
        [DisplayName("积分锁定")]
        [Description("积分锁定。锁定后，下一级子任务积分总和不能超过当前任务")]
        [DataObjectField(false, false, true, 1)]
        [BindColumn(6, "LockScore", "积分锁定。锁定后，下一级子任务积分总和不能超过当前任务", null, "bit", 0, 0, false)]
        public virtual Boolean LockScore
        {
            get { return _LockScore; }
            set { if (OnPropertyChanging(__.LockScore, value)) { _LockScore = value; OnPropertyChanged(__.LockScore); } }
        }

        private Int32 _Percent;
        /// <summary>同级比重。0~100在同级任务中的比重百分比</summary>
        [DisplayName("同级比重")]
        [Description("同级比重。0~100在同级任务中的比重百分比")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(7, "Percent", "同级比重。0~100在同级任务中的比重百分比", null, "int", 10, 0, false)]
        public virtual Int32 Percent
        {
            get { return _Percent; }
            set { if (OnPropertyChanging(__.Percent, value)) { _Percent = value; OnPropertyChanged(__.Percent); } }
        }

        private Int32 _Priority;
        /// <summary>优先级。数字越大优先级越高</summary>
        [DisplayName("优先级")]
        [Description("优先级。数字越大优先级越高")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(8, "Priority", "优先级。数字越大优先级越高", null, "int", 10, 0, false)]
        public virtual Int32 Priority
        {
            get { return _Priority; }
            set { if (OnPropertyChanging(__.Priority, value)) { _Priority = value; OnPropertyChanged(__.Priority); } }
        }

        private Int32 _Status;
        /// <summary>状态。准备、进行、暂停、取消、完成</summary>
        [DisplayName("状态")]
        [Description("状态。准备、进行、暂停、取消、完成")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(9, "Status", "状态。准备、进行、暂停、取消、完成", null, "int", 10, 0, false)]
        public virtual Int32 Status
        {
            get { return _Status; }
            set { if (OnPropertyChanging(__.Status, value)) { _Status = value; OnPropertyChanged(__.Status); } }
        }

        private DateTime _PlanStartTime;
        /// <summary>计划开始时间</summary>
        [DisplayName("计划开始时间")]
        [Description("计划开始时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn(10, "PlanStartTime", "计划开始时间", null, "datetime", 3, 0, false)]
        public virtual DateTime PlanStartTime
        {
            get { return _PlanStartTime; }
            set { if (OnPropertyChanging(__.PlanStartTime, value)) { _PlanStartTime = value; OnPropertyChanged(__.PlanStartTime); } }
        }

        private DateTime _PlanEndTime;
        /// <summary>计划结束时间</summary>
        [DisplayName("计划结束时间")]
        [Description("计划结束时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn(11, "PlanEndTime", "计划结束时间", null, "datetime", 3, 0, false)]
        public virtual DateTime PlanEndTime
        {
            get { return _PlanEndTime; }
            set { if (OnPropertyChanging(__.PlanEndTime, value)) { _PlanEndTime = value; OnPropertyChanged(__.PlanEndTime); } }
        }

        private Int32 _PlanCost;
        /// <summary>计划工作日。需要多少个工作日</summary>
        [DisplayName("计划工作日")]
        [Description("计划工作日。需要多少个工作日")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(12, "PlanCost", "计划工作日。需要多少个工作日", null, "int", 10, 0, false)]
        public virtual Int32 PlanCost
        {
            get { return _PlanCost; }
            set { if (OnPropertyChanging(__.PlanCost, value)) { _PlanCost = value; OnPropertyChanged(__.PlanCost); } }
        }

        private Boolean _LockPlanTime;
        /// <summary>计划时间锁定。锁定后，子任务的计划时间不得超过父任务</summary>
        [DisplayName("计划时间锁定")]
        [Description("计划时间锁定。锁定后，子任务的计划时间不得超过父任务")]
        [DataObjectField(false, false, true, 1)]
        [BindColumn(13, "LockPlanTime", "计划时间锁定。锁定后，子任务的计划时间不得超过父任务", null, "bit", 0, 0, false)]
        public virtual Boolean LockPlanTime
        {
            get { return _LockPlanTime; }
            set { if (OnPropertyChanging(__.LockPlanTime, value)) { _LockPlanTime = value; OnPropertyChanged(__.LockPlanTime); } }
        }

        private DateTime _StartTime;
        /// <summary>开始时间</summary>
        [DisplayName("开始时间")]
        [Description("开始时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn(14, "StartTime", "开始时间", null, "datetime", 3, 0, false)]
        public virtual DateTime StartTime
        {
            get { return _StartTime; }
            set { if (OnPropertyChanging(__.StartTime, value)) { _StartTime = value; OnPropertyChanged(__.StartTime); } }
        }

        private DateTime _EndTime;
        /// <summary>结束时间</summary>
        [DisplayName("结束时间")]
        [Description("结束时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn(15, "EndTime", "结束时间", null, "datetime", 3, 0, false)]
        public virtual DateTime EndTime
        {
            get { return _EndTime; }
            set { if (OnPropertyChanging(__.EndTime, value)) { _EndTime = value; OnPropertyChanged(__.EndTime); } }
        }

        private Int32 _Cost;
        /// <summary>实际工作日</summary>
        [DisplayName("实际工作日")]
        [Description("实际工作日")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(16, "Cost", "实际工作日", null, "int", 10, 0, false)]
        public virtual Int32 Cost
        {
            get { return _Cost; }
            set { if (OnPropertyChanging(__.Cost, value)) { _Cost = value; OnPropertyChanged(__.Cost); } }
        }

        private Int32 _Progress;
        /// <summary>进度。0到100</summary>
        [DisplayName("进度")]
        [Description("进度。0到100")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(17, "Progress", "进度。0到100", null, "int", 10, 0, false)]
        public virtual Int32 Progress
        {
            get { return _Progress; }
            set { if (OnPropertyChanging(__.Progress, value)) { _Progress = value; OnPropertyChanged(__.Progress); } }
        }

        private Int32 _MasterID;
        /// <summary>负责人</summary>
        [DisplayName("负责人")]
        [Description("负责人")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(18, "MasterID", "负责人", null, "int", 10, 0, false)]
        public virtual Int32 MasterID
        {
            get { return _MasterID; }
            set { if (OnPropertyChanging(__.MasterID, value)) { _MasterID = value; OnPropertyChanged(__.MasterID); } }
        }

        private Int32 _Views;
        /// <summary>浏览数</summary>
        [DisplayName("浏览数")]
        [Description("浏览数")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(19, "Views", "浏览数", null, "int", 10, 0, false)]
        public virtual Int32 Views
        {
            get { return _Views; }
            set { if (OnPropertyChanging(__.Views, value)) { _Views = value; OnPropertyChanged(__.Views); } }
        }

        private Int32 _Historys;
        /// <summary>修改次数</summary>
        [DisplayName("修改次数")]
        [Description("修改次数")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(20, "Historys", "修改次数", null, "int", 10, 0, false)]
        public virtual Int32 Historys
        {
            get { return _Historys; }
            set { if (OnPropertyChanging(__.Historys, value)) { _Historys = value; OnPropertyChanged(__.Historys); } }
        }

        private Int32 _Comments;
        /// <summary>评论数</summary>
        [DisplayName("评论数")]
        [Description("评论数")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(21, "Comments", "评论数", null, "int", 10, 0, false)]
        public virtual Int32 Comments
        {
            get { return _Comments; }
            set { if (OnPropertyChanging(__.Comments, value)) { _Comments = value; OnPropertyChanged(__.Comments); } }
        }

        private DateTime _LastComment;
        /// <summary>最后评论</summary>
        [DisplayName("最后评论")]
        [Description("最后评论")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn(22, "LastComment", "最后评论", null, "datetime", 3, 0, false)]
        public virtual DateTime LastComment
        {
            get { return _LastComment; }
            set { if (OnPropertyChanging(__.LastComment, value)) { _LastComment = value; OnPropertyChanged(__.LastComment); } }
        }

        private Boolean _Deleted;
        /// <summary>已删除</summary>
        [DisplayName("已删除")]
        [Description("已删除")]
        [DataObjectField(false, false, true, 1)]
        [BindColumn(23, "Deleted", "已删除", null, "bit", 0, 0, false)]
        public virtual Boolean Deleted
        {
            get { return _Deleted; }
            set { if (OnPropertyChanging(__.Deleted, value)) { _Deleted = value; OnPropertyChanged(__.Deleted); } }
        }

        private Int32 _CreateUserID;
        /// <summary>创建者</summary>
        [DisplayName("创建者")]
        [Description("创建者")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(24, "CreateUserID", "创建者", null, "int", 10, 0, false)]
        public virtual Int32 CreateUserID
        {
            get { return _CreateUserID; }
            set { if (OnPropertyChanging(__.CreateUserID, value)) { _CreateUserID = value; OnPropertyChanged(__.CreateUserID); } }
        }

        private DateTime _CreateTime;
        /// <summary>创建时间</summary>
        [DisplayName("创建时间")]
        [Description("创建时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn(25, "CreateTime", "创建时间", null, "datetime", 3, 0, false)]
        public virtual DateTime CreateTime
        {
            get { return _CreateTime; }
            set { if (OnPropertyChanging(__.CreateTime, value)) { _CreateTime = value; OnPropertyChanged(__.CreateTime); } }
        }

        private Int32 _UpdateUserID;
        /// <summary>更新者</summary>
        [DisplayName("更新者")]
        [Description("更新者")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(26, "UpdateUserID", "更新者", null, "int", 10, 0, false)]
        public virtual Int32 UpdateUserID
        {
            get { return _UpdateUserID; }
            set { if (OnPropertyChanging(__.UpdateUserID, value)) { _UpdateUserID = value; OnPropertyChanged(__.UpdateUserID); } }
        }

        private DateTime _UpdateTime;
        /// <summary>更新时间</summary>
        [DisplayName("更新时间")]
        [Description("更新时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn(27, "UpdateTime", "更新时间", null, "datetime", 3, 0, false)]
        public virtual DateTime UpdateTime
        {
            get { return _UpdateTime; }
            set { if (OnPropertyChanging(__.UpdateTime, value)) { _UpdateTime = value; OnPropertyChanged(__.UpdateTime); } }
        }

        private String _Content;
        /// <summary>内容</summary>
        [DisplayName("内容")]
        [Description("内容")]
        [DataObjectField(false, false, true, 500)]
        [BindColumn(28, "Content", "内容", null, "nvarchar(500)", 0, 0, true)]
        public virtual String Content
        {
            get { return _Content; }
            set { if (OnPropertyChanging(__.Content, value)) { _Content = value; OnPropertyChanged(__.Content); } }
        }
        #endregion

        #region 获取/设置 字段值
        /// <summary>
        /// 获取/设置 字段值。
        /// 一个索引，基类使用反射实现。
        /// 派生实体类可重写该索引，以避免反射带来的性能损耗
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        public override Object this[String name]
        {
            get
            {
                switch (name)
                {
                    case __.ID : return _ID;
                    case __.Name : return _Name;
                    case __.ParentID : return _ParentID;
                    case __.ChildCount : return _ChildCount;
                    case __.Score : return _Score;
                    case __.LockScore : return _LockScore;
                    case __.Percent : return _Percent;
                    case __.Priority : return _Priority;
                    case __.Status : return _Status;
                    case __.PlanStartTime : return _PlanStartTime;
                    case __.PlanEndTime : return _PlanEndTime;
                    case __.PlanCost : return _PlanCost;
                    case __.LockPlanTime : return _LockPlanTime;
                    case __.StartTime : return _StartTime;
                    case __.EndTime : return _EndTime;
                    case __.Cost : return _Cost;
                    case __.Progress : return _Progress;
                    case __.MasterID : return _MasterID;
                    case __.Views : return _Views;
                    case __.Historys : return _Historys;
                    case __.Comments : return _Comments;
                    case __.LastComment : return _LastComment;
                    case __.Deleted : return _Deleted;
                    case __.CreateUserID : return _CreateUserID;
                    case __.CreateTime : return _CreateTime;
                    case __.UpdateUserID : return _UpdateUserID;
                    case __.UpdateTime : return _UpdateTime;
                    case __.Content : return _Content;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case __.ID : _ID = Convert.ToInt32(value); break;
                    case __.Name : _Name = Convert.ToString(value); break;
                    case __.ParentID : _ParentID = Convert.ToInt32(value); break;
                    case __.ChildCount : _ChildCount = Convert.ToInt32(value); break;
                    case __.Score : _Score = Convert.ToInt32(value); break;
                    case __.LockScore : _LockScore = Convert.ToBoolean(value); break;
                    case __.Percent : _Percent = Convert.ToInt32(value); break;
                    case __.Priority : _Priority = Convert.ToInt32(value); break;
                    case __.Status : _Status = Convert.ToInt32(value); break;
                    case __.PlanStartTime : _PlanStartTime = Convert.ToDateTime(value); break;
                    case __.PlanEndTime : _PlanEndTime = Convert.ToDateTime(value); break;
                    case __.PlanCost : _PlanCost = Convert.ToInt32(value); break;
                    case __.LockPlanTime : _LockPlanTime = Convert.ToBoolean(value); break;
                    case __.StartTime : _StartTime = Convert.ToDateTime(value); break;
                    case __.EndTime : _EndTime = Convert.ToDateTime(value); break;
                    case __.Cost : _Cost = Convert.ToInt32(value); break;
                    case __.Progress : _Progress = Convert.ToInt32(value); break;
                    case __.MasterID : _MasterID = Convert.ToInt32(value); break;
                    case __.Views : _Views = Convert.ToInt32(value); break;
                    case __.Historys : _Historys = Convert.ToInt32(value); break;
                    case __.Comments : _Comments = Convert.ToInt32(value); break;
                    case __.LastComment : _LastComment = Convert.ToDateTime(value); break;
                    case __.Deleted : _Deleted = Convert.ToBoolean(value); break;
                    case __.CreateUserID : _CreateUserID = Convert.ToInt32(value); break;
                    case __.CreateTime : _CreateTime = Convert.ToDateTime(value); break;
                    case __.UpdateUserID : _UpdateUserID = Convert.ToInt32(value); break;
                    case __.UpdateTime : _UpdateTime = Convert.ToDateTime(value); break;
                    case __.Content : _Content = Convert.ToString(value); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得任务字段信息的快捷方式</summary>
        public partial class _
        {
            ///<summary>编号</summary>
            public static readonly Field ID = FindByName(__.ID);

            ///<summary>名称</summary>
            public static readonly Field Name = FindByName(__.Name);

            ///<summary>父任务。顶级任务的父任务为0</summary>
            public static readonly Field ParentID = FindByName(__.ParentID);

            ///<summary>子任务数</summary>
            public static readonly Field ChildCount = FindByName(__.ChildCount);

            ///<summary>积分。全局权重，可用于绩效考核</summary>
            public static readonly Field Score = FindByName(__.Score);

            ///<summary>积分锁定。锁定后，下一级子任务积分总和不能超过当前任务</summary>
            public static readonly Field LockScore = FindByName(__.LockScore);

            ///<summary>同级比重。0~100在同级任务中的比重百分比</summary>
            public static readonly Field Percent = FindByName(__.Percent);

            ///<summary>优先级。数字越大优先级越高</summary>
            public static readonly Field Priority = FindByName(__.Priority);

            ///<summary>状态。准备、进行、暂停、取消、完成</summary>
            public static readonly Field Status = FindByName(__.Status);

            ///<summary>计划开始时间</summary>
            public static readonly Field PlanStartTime = FindByName(__.PlanStartTime);

            ///<summary>计划结束时间</summary>
            public static readonly Field PlanEndTime = FindByName(__.PlanEndTime);

            ///<summary>计划工作日。需要多少个工作日</summary>
            public static readonly Field PlanCost = FindByName(__.PlanCost);

            ///<summary>计划时间锁定。锁定后，子任务的计划时间不得超过父任务</summary>
            public static readonly Field LockPlanTime = FindByName(__.LockPlanTime);

            ///<summary>开始时间</summary>
            public static readonly Field StartTime = FindByName(__.StartTime);

            ///<summary>结束时间</summary>
            public static readonly Field EndTime = FindByName(__.EndTime);

            ///<summary>实际工作日</summary>
            public static readonly Field Cost = FindByName(__.Cost);

            ///<summary>进度。0到100</summary>
            public static readonly Field Progress = FindByName(__.Progress);

            ///<summary>负责人</summary>
            public static readonly Field MasterID = FindByName(__.MasterID);

            ///<summary>浏览数</summary>
            public static readonly Field Views = FindByName(__.Views);

            ///<summary>修改次数</summary>
            public static readonly Field Historys = FindByName(__.Historys);

            ///<summary>评论数</summary>
            public static readonly Field Comments = FindByName(__.Comments);

            ///<summary>最后评论</summary>
            public static readonly Field LastComment = FindByName(__.LastComment);

            ///<summary>已删除</summary>
            public static readonly Field Deleted = FindByName(__.Deleted);

            ///<summary>创建者</summary>
            public static readonly Field CreateUserID = FindByName(__.CreateUserID);

            ///<summary>创建时间</summary>
            public static readonly Field CreateTime = FindByName(__.CreateTime);

            ///<summary>更新者</summary>
            public static readonly Field UpdateUserID = FindByName(__.UpdateUserID);

            ///<summary>更新时间</summary>
            public static readonly Field UpdateTime = FindByName(__.UpdateTime);

            ///<summary>内容</summary>
            public static readonly Field Content = FindByName(__.Content);

            static Field FindByName(String name) { return Meta.Table.FindByName(name); }
        }

        /// <summary>取得任务字段名称的快捷方式</summary>
        partial class __
        {
            ///<summary>编号</summary>
            public const String ID = "ID";

            ///<summary>名称</summary>
            public const String Name = "Name";

            ///<summary>父任务。顶级任务的父任务为0</summary>
            public const String ParentID = "ParentID";

            ///<summary>子任务数</summary>
            public const String ChildCount = "ChildCount";

            ///<summary>积分。全局权重，可用于绩效考核</summary>
            public const String Score = "Score";

            ///<summary>积分锁定。锁定后，下一级子任务积分总和不能超过当前任务</summary>
            public const String LockScore = "LockScore";

            ///<summary>同级比重。0~100在同级任务中的比重百分比</summary>
            public const String Percent = "Percent";

            ///<summary>优先级。数字越大优先级越高</summary>
            public const String Priority = "Priority";

            ///<summary>状态。准备、进行、暂停、取消、完成</summary>
            public const String Status = "Status";

            ///<summary>计划开始时间</summary>
            public const String PlanStartTime = "PlanStartTime";

            ///<summary>计划结束时间</summary>
            public const String PlanEndTime = "PlanEndTime";

            ///<summary>计划工作日。需要多少个工作日</summary>
            public const String PlanCost = "PlanCost";

            ///<summary>计划时间锁定。锁定后，子任务的计划时间不得超过父任务</summary>
            public const String LockPlanTime = "LockPlanTime";

            ///<summary>开始时间</summary>
            public const String StartTime = "StartTime";

            ///<summary>结束时间</summary>
            public const String EndTime = "EndTime";

            ///<summary>实际工作日</summary>
            public const String Cost = "Cost";

            ///<summary>进度。0到100</summary>
            public const String Progress = "Progress";

            ///<summary>负责人</summary>
            public const String MasterID = "MasterID";

            ///<summary>浏览数</summary>
            public const String Views = "Views";

            ///<summary>修改次数</summary>
            public const String Historys = "Historys";

            ///<summary>评论数</summary>
            public const String Comments = "Comments";

            ///<summary>最后评论</summary>
            public const String LastComment = "LastComment";

            ///<summary>已删除</summary>
            public const String Deleted = "Deleted";

            ///<summary>创建者</summary>
            public const String CreateUserID = "CreateUserID";

            ///<summary>创建时间</summary>
            public const String CreateTime = "CreateTime";

            ///<summary>更新者</summary>
            public const String UpdateUserID = "UpdateUserID";

            ///<summary>更新时间</summary>
            public const String UpdateTime = "UpdateTime";

            ///<summary>内容</summary>
            public const String Content = "Content";

        }
        #endregion
    }

    /// <summary>任务接口</summary>
    public partial interface IWorkTask
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>名称</summary>
        String Name { get; set; }

        /// <summary>父任务。顶级任务的父任务为0</summary>
        Int32 ParentID { get; set; }

        /// <summary>子任务数</summary>
        Int32 ChildCount { get; set; }

        /// <summary>积分。全局权重，可用于绩效考核</summary>
        Int32 Score { get; set; }

        /// <summary>积分锁定。锁定后，下一级子任务积分总和不能超过当前任务</summary>
        Boolean LockScore { get; set; }

        /// <summary>同级比重。0~100在同级任务中的比重百分比</summary>
        Int32 Percent { get; set; }

        /// <summary>优先级。数字越大优先级越高</summary>
        Int32 Priority { get; set; }

        /// <summary>状态。准备、进行、暂停、取消、完成</summary>
        Int32 Status { get; set; }

        /// <summary>计划开始时间</summary>
        DateTime PlanStartTime { get; set; }

        /// <summary>计划结束时间</summary>
        DateTime PlanEndTime { get; set; }

        /// <summary>计划工作日。需要多少个工作日</summary>
        Int32 PlanCost { get; set; }

        /// <summary>计划时间锁定。锁定后，子任务的计划时间不得超过父任务</summary>
        Boolean LockPlanTime { get; set; }

        /// <summary>开始时间</summary>
        DateTime StartTime { get; set; }

        /// <summary>结束时间</summary>
        DateTime EndTime { get; set; }

        /// <summary>实际工作日</summary>
        Int32 Cost { get; set; }

        /// <summary>进度。0到100</summary>
        Int32 Progress { get; set; }

        /// <summary>负责人</summary>
        Int32 MasterID { get; set; }

        /// <summary>浏览数</summary>
        Int32 Views { get; set; }

        /// <summary>修改次数</summary>
        Int32 Historys { get; set; }

        /// <summary>评论数</summary>
        Int32 Comments { get; set; }

        /// <summary>最后评论</summary>
        DateTime LastComment { get; set; }

        /// <summary>已删除</summary>
        Boolean Deleted { get; set; }

        /// <summary>创建者</summary>
        Int32 CreateUserID { get; set; }

        /// <summary>创建时间</summary>
        DateTime CreateTime { get; set; }

        /// <summary>更新者</summary>
        Int32 UpdateUserID { get; set; }

        /// <summary>更新时间</summary>
        DateTime UpdateTime { get; set; }

        /// <summary>内容</summary>
        String Content { get; set; }
        #endregion

        #region 获取/设置 字段值
        /// <summary>获取/设置 字段值。</summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        Object this[String name] { get; set; }
        #endregion
    }
}