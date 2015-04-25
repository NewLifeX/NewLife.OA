﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace NewLife.OA
{
    /// <summary>任务成员</summary>
    [Serializable]
    [DataObject]
    [Description("任务成员")]
    [BindIndex("IU_TaskMember_WorkTaskID_MemberID", true, "WorkTaskID,MemberID")]
    [BindIndex("IX_TaskMember_WorkTaskID", false, "WorkTaskID")]
    [BindIndex("IX_TaskMember_MemberID", false, "MemberID")]
    [BindIndex("IX_TaskMember_MemberID_Kind", false, "MemberID,Kind")]
    [BindTable("TaskMember", Description = "任务成员", ConnName = "OA", DbType = DatabaseType.SqlServer)]
    public partial class TaskMember : ITaskMember
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

        private Int32 _WorkTaskID;
        /// <summary>任务</summary>
        [DisplayName("任务")]
        [Description("任务")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(2, "WorkTaskID", "任务", null, "int", 10, 0, false)]
        public virtual Int32 WorkTaskID
        {
            get { return _WorkTaskID; }
            set { if (OnPropertyChanging(__.WorkTaskID, value)) { _WorkTaskID = value; OnPropertyChanged(__.WorkTaskID); } }
        }

        private Int32 _MemberID;
        /// <summary>成员</summary>
        [DisplayName("成员")]
        [Description("成员")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(3, "MemberID", "成员", null, "int", 10, 0, false)]
        public virtual Int32 MemberID
        {
            get { return _MemberID; }
            set { if (OnPropertyChanging(__.MemberID, value)) { _MemberID = value; OnPropertyChanged(__.MemberID); } }
        }

        private Int32 _Kind;
        /// <summary>种类</summary>
        [DisplayName("种类")]
        [Description("种类")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(4, "Kind", "种类", null, "int", 10, 0, false)]
        public virtual Int32 Kind
        {
            get { return _Kind; }
            set { if (OnPropertyChanging(__.Kind, value)) { _Kind = value; OnPropertyChanged(__.Kind); } }
        }

        private DateTime _LastViewTime;
        /// <summary>最后查看时间</summary>
        [DisplayName("最后查看时间")]
        [Description("最后查看时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn(5, "LastViewTime", "最后查看时间", null, "datetime", 3, 0, false)]
        public virtual DateTime LastViewTime
        {
            get { return _LastViewTime; }
            set { if (OnPropertyChanging(__.LastViewTime, value)) { _LastViewTime = value; OnPropertyChanged(__.LastViewTime); } }
        }

        private DateTime _LastUpdateTime;
        /// <summary>最后更新时间</summary>
        [DisplayName("最后更新时间")]
        [Description("最后更新时间")]
        [DataObjectField(false, false, true, 3)]
        [BindColumn(6, "LastUpdateTime", "最后更新时间", null, "datetime", 3, 0, false)]
        public virtual DateTime LastUpdateTime
        {
            get { return _LastUpdateTime; }
            set { if (OnPropertyChanging(__.LastUpdateTime, value)) { _LastUpdateTime = value; OnPropertyChanged(__.LastUpdateTime); } }
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
                    case __.WorkTaskID : return _WorkTaskID;
                    case __.MemberID : return _MemberID;
                    case __.Kind : return _Kind;
                    case __.LastViewTime : return _LastViewTime;
                    case __.LastUpdateTime : return _LastUpdateTime;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case __.ID : _ID = Convert.ToInt32(value); break;
                    case __.WorkTaskID : _WorkTaskID = Convert.ToInt32(value); break;
                    case __.MemberID : _MemberID = Convert.ToInt32(value); break;
                    case __.Kind : _Kind = Convert.ToInt32(value); break;
                    case __.LastViewTime : _LastViewTime = Convert.ToDateTime(value); break;
                    case __.LastUpdateTime : _LastUpdateTime = Convert.ToDateTime(value); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得任务成员字段信息的快捷方式</summary>
        public partial class _
        {
            ///<summary>编号</summary>
            public static readonly Field ID = FindByName(__.ID);

            ///<summary>任务</summary>
            public static readonly Field WorkTaskID = FindByName(__.WorkTaskID);

            ///<summary>成员</summary>
            public static readonly Field MemberID = FindByName(__.MemberID);

            ///<summary>种类</summary>
            public static readonly Field Kind = FindByName(__.Kind);

            ///<summary>最后查看时间</summary>
            public static readonly Field LastViewTime = FindByName(__.LastViewTime);

            ///<summary>最后更新时间</summary>
            public static readonly Field LastUpdateTime = FindByName(__.LastUpdateTime);

            static Field FindByName(String name) { return Meta.Table.FindByName(name); }
        }

        /// <summary>取得任务成员字段名称的快捷方式</summary>
        partial class __
        {
            ///<summary>编号</summary>
            public const String ID = "ID";

            ///<summary>任务</summary>
            public const String WorkTaskID = "WorkTaskID";

            ///<summary>成员</summary>
            public const String MemberID = "MemberID";

            ///<summary>种类</summary>
            public const String Kind = "Kind";

            ///<summary>最后查看时间</summary>
            public const String LastViewTime = "LastViewTime";

            ///<summary>最后更新时间</summary>
            public const String LastUpdateTime = "LastUpdateTime";

        }
        #endregion
    }

    /// <summary>任务成员接口</summary>
    public partial interface ITaskMember
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>任务</summary>
        Int32 WorkTaskID { get; set; }

        /// <summary>成员</summary>
        Int32 MemberID { get; set; }

        /// <summary>种类</summary>
        Int32 Kind { get; set; }

        /// <summary>最后查看时间</summary>
        DateTime LastViewTime { get; set; }

        /// <summary>最后更新时间</summary>
        DateTime LastUpdateTime { get; set; }
        #endregion

        #region 获取/设置 字段值
        /// <summary>获取/设置 字段值。</summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        Object this[String name] { get; set; }
        #endregion
    }
}