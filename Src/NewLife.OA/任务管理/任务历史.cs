﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace NewLife.OA
{
    /// <summary>任务历史</summary>
    [Serializable]
    [DataObject]
    [Description("任务历史")]
    [BindIndex("IX_TaskHistory_TaskID", false, "TaskID")]
    [BindIndex("IX_TaskHistory_Kind", false, "Kind")]
    [BindIndex("IX_TaskHistory_TaskID_Kind", false, "TaskID,Kind")]
    [BindRelation("TaskID", false, "WorkTask", "ID")]
    [BindRelation("CreateUserID", false, "User", "ID")]
    [BindRelation("UpdateUserID", false, "User", "ID")]
    [BindTable("TaskHistory", Description = "任务历史", ConnName = "OA", DbType = DatabaseType.SqlServer)]
    public partial class TaskHistory : ITaskHistory
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

        private Int32 _TaskID;
        /// <summary>任务</summary>
        [DisplayName("任务")]
        [Description("任务")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(2, "TaskID", "任务", null, "int", 10, 0, false)]
        public virtual Int32 TaskID
        {
            get { return _TaskID; }
            set { if (OnPropertyChanging(__.TaskID, value)) { _TaskID = value; OnPropertyChanged(__.TaskID); } }
        }

        private String _Kind;
        /// <summary>种类</summary>
        [DisplayName("种类")]
        [Description("种类")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn(3, "Kind", "种类", null, "nvarchar(50)", 0, 0, true)]
        public virtual String Kind
        {
            get { return _Kind; }
            set { if (OnPropertyChanging(__.Kind, value)) { _Kind = value; OnPropertyChanged(__.Kind); } }
        }

        private String _SrcValue;
        /// <summary>原来的值</summary>
        [DisplayName("原来的值")]
        [Description("原来的值")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn(4, "SrcValue", "原来的值", null, "nvarchar(50)", 0, 0, true)]
        public virtual String SrcValue
        {
            get { return _SrcValue; }
            set { if (OnPropertyChanging(__.SrcValue, value)) { _SrcValue = value; OnPropertyChanged(__.SrcValue); } }
        }

        private String _NewValue;
        /// <summary>新的值</summary>
        [DisplayName("新的值")]
        [Description("新的值")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn(5, "NewValue", "新的值", null, "nvarchar(50)", 0, 0, true)]
        public virtual String NewValue
        {
            get { return _NewValue; }
            set { if (OnPropertyChanging(__.NewValue, value)) { _NewValue = value; OnPropertyChanged(__.NewValue); } }
        }

        private Int32 _CreateUserID;
        /// <summary>创建者</summary>
        [DisplayName("创建者")]
        [Description("创建者")]
        [DataObjectField(false, false, true, 10)]
        [BindColumn(6, "CreateUserID", "创建者", null, "int", 10, 0, false)]
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
        [BindColumn(7, "CreateTime", "创建时间", null, "datetime", 3, 0, false)]
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
        [BindColumn(8, "UpdateUserID", "更新者", null, "int", 10, 0, false)]
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
        [BindColumn(9, "UpdateTime", "更新时间", null, "datetime", 3, 0, false)]
        public virtual DateTime UpdateTime
        {
            get { return _UpdateTime; }
            set { if (OnPropertyChanging(__.UpdateTime, value)) { _UpdateTime = value; OnPropertyChanged(__.UpdateTime); } }
        }

        private String _Remark;
        /// <summary>备注</summary>
        [DisplayName("备注")]
        [Description("备注")]
        [DataObjectField(false, false, true, 500)]
        [BindColumn(10, "Remark", "备注", null, "nvarchar(500)", 0, 0, true)]
        public virtual String Remark
        {
            get { return _Remark; }
            set { if (OnPropertyChanging(__.Remark, value)) { _Remark = value; OnPropertyChanged(__.Remark); } }
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
                    case __.TaskID : return _TaskID;
                    case __.Kind : return _Kind;
                    case __.SrcValue : return _SrcValue;
                    case __.NewValue : return _NewValue;
                    case __.CreateUserID : return _CreateUserID;
                    case __.CreateTime : return _CreateTime;
                    case __.UpdateUserID : return _UpdateUserID;
                    case __.UpdateTime : return _UpdateTime;
                    case __.Remark : return _Remark;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case __.ID : _ID = Convert.ToInt32(value); break;
                    case __.TaskID : _TaskID = Convert.ToInt32(value); break;
                    case __.Kind : _Kind = Convert.ToString(value); break;
                    case __.SrcValue : _SrcValue = Convert.ToString(value); break;
                    case __.NewValue : _NewValue = Convert.ToString(value); break;
                    case __.CreateUserID : _CreateUserID = Convert.ToInt32(value); break;
                    case __.CreateTime : _CreateTime = Convert.ToDateTime(value); break;
                    case __.UpdateUserID : _UpdateUserID = Convert.ToInt32(value); break;
                    case __.UpdateTime : _UpdateTime = Convert.ToDateTime(value); break;
                    case __.Remark : _Remark = Convert.ToString(value); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得任务历史字段信息的快捷方式</summary>
        public partial class _
        {
            ///<summary>编号</summary>
            public static readonly Field ID = FindByName(__.ID);

            ///<summary>任务</summary>
            public static readonly Field TaskID = FindByName(__.TaskID);

            ///<summary>种类</summary>
            public static readonly Field Kind = FindByName(__.Kind);

            ///<summary>原来的值</summary>
            public static readonly Field SrcValue = FindByName(__.SrcValue);

            ///<summary>新的值</summary>
            public static readonly Field NewValue = FindByName(__.NewValue);

            ///<summary>创建者</summary>
            public static readonly Field CreateUserID = FindByName(__.CreateUserID);

            ///<summary>创建时间</summary>
            public static readonly Field CreateTime = FindByName(__.CreateTime);

            ///<summary>更新者</summary>
            public static readonly Field UpdateUserID = FindByName(__.UpdateUserID);

            ///<summary>更新时间</summary>
            public static readonly Field UpdateTime = FindByName(__.UpdateTime);

            ///<summary>备注</summary>
            public static readonly Field Remark = FindByName(__.Remark);

            static Field FindByName(String name) { return Meta.Table.FindByName(name); }
        }

        /// <summary>取得任务历史字段名称的快捷方式</summary>
        partial class __
        {
            ///<summary>编号</summary>
            public const String ID = "ID";

            ///<summary>任务</summary>
            public const String TaskID = "TaskID";

            ///<summary>种类</summary>
            public const String Kind = "Kind";

            ///<summary>原来的值</summary>
            public const String SrcValue = "SrcValue";

            ///<summary>新的值</summary>
            public const String NewValue = "NewValue";

            ///<summary>创建者</summary>
            public const String CreateUserID = "CreateUserID";

            ///<summary>创建时间</summary>
            public const String CreateTime = "CreateTime";

            ///<summary>更新者</summary>
            public const String UpdateUserID = "UpdateUserID";

            ///<summary>更新时间</summary>
            public const String UpdateTime = "UpdateTime";

            ///<summary>备注</summary>
            public const String Remark = "Remark";

        }
        #endregion
    }

    /// <summary>任务历史接口</summary>
    public partial interface ITaskHistory
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>任务</summary>
        Int32 TaskID { get; set; }

        /// <summary>种类</summary>
        String Kind { get; set; }

        /// <summary>原来的值</summary>
        String SrcValue { get; set; }

        /// <summary>新的值</summary>
        String NewValue { get; set; }

        /// <summary>创建者</summary>
        Int32 CreateUserID { get; set; }

        /// <summary>创建时间</summary>
        DateTime CreateTime { get; set; }

        /// <summary>更新者</summary>
        Int32 UpdateUserID { get; set; }

        /// <summary>更新时间</summary>
        DateTime UpdateTime { get; set; }

        /// <summary>备注</summary>
        String Remark { get; set; }
        #endregion

        #region 获取/设置 字段值
        /// <summary>获取/设置 字段值。</summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        Object this[String name] { get; set; }
        #endregion
    }
}