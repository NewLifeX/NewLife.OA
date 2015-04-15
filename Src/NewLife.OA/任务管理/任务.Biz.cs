/*
 * XCoder v6.2.5498.35992
 * 作者：topyms/YMS-PC
 * 时间：2015-01-20 20:08:12
 * 版权：版权所有 (C) 新生命开发团队 2002~2015
*/
﻿using System;
using System.ComponentModel;
using XCode;
using XCode.Membership;

namespace NewLife.OA
{
    /// <summary>任务</summary>
    public partial class WorkTask : Entity<WorkTask>
    {
        #region 对象操作﻿
        protected override WorkTask CreateInstance(bool forEdit = false)
        {
            var entity = base.CreateInstance(forEdit);
            if (forEdit)
            {
                entity.CreateUserID = ManageProvider.Provider.Current.ID;
                entity.CreateTime = DateTime.Now;
            }
            return entity;
        }

        /// <summary>验证数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew"></param>
        public override void Valid(Boolean isNew)
        {
            // 这里验证参数范围，建议抛出参数异常，指定参数名，前端用户界面可以捕获参数异常并聚焦到对应的参数输入框
            //if (String.IsNullOrEmpty(Name)) throw new ArgumentNullException(_.Name, _.Name.DisplayName + "无效！");
            //if (!isNew && ID < 1) throw new ArgumentOutOfRangeException(_.ID, _.ID.DisplayName + "必须大于0！");

            // 建议先调用基类方法，基类方法会对唯一索引的数据进行验证
            base.Valid(isNew);

            // 在新插入数据或者修改了指定字段时进行唯一性验证，CheckExist内部抛出参数异常
            //if (isNew || Dirtys[__.Name]) CheckExist(__.Name);

            if (isNew && !Dirtys[__.CreateTime]) CreateTime = DateTime.Now;
            if (!Dirtys[__.UpdateTime]) UpdateTime = DateTime.Now;
        }

        ///// <summary>首次连接数据库时初始化数据，仅用于实体类重载，用户不应该调用该方法</summary>
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //protected override void InitData()
        //{
        //    base.InitData();

        //    // InitData一般用于当数据表没有数据时添加一些默认数据，该实体类的任何第一次数据库操作都会触发该方法，默认异步调用
        //    // Meta.Count是快速取得表记录数
        //    if (Meta.Count > 0) return;

        //    // 需要注意的是，如果该方法调用了其它实体类的首次数据库操作，目标实体类的数据初始化将会在同一个线程完成
        //    if (XTrace.Debug) XTrace.WriteLine("开始初始化{0}[{1}]数据……", typeof(WorkTask).Name, Meta.Table.DataTable.DisplayName);

        //    var entity = new WorkTask();
        //    entity.Name = "abc";
        //    entity.ParentID = 0;
        //    entity.Score = 0;
        //    entity.Priority = 0;
        //    entity.Remark = "abc";
        //    entity.Status = 0;
        //    entity.PlanTime = DateTime.Now;
        //    entity.PlanCost = 0;
        //    entity.StartTime = DateTime.Now;
        //    entity.EndTime = DateTime.Now;
        //    entity.Cost = 0;
        //    entity.Progress = 0;
        //    entity.MasterID = 0;
        //    entity.Members = "abc";
        //    entity.CreateUserID = 0;
        //    entity.CreateTime = DateTime.Now;
        //    entity.UpdateUserID = 0;
        //    entity.UpdateTime = DateTime.Now;
        //    entity.Insert();

        //    if (XTrace.Debug) XTrace.WriteLine("完成初始化{0}[{1}]数据！", typeof(WorkTask).Name, Meta.Table.DataTable.DisplayName);
        //}


        ///// <summary>已重载。基类先调用Valid(true)验证数据，然后在事务保护内调用OnInsert</summary>
        ///// <returns></returns>
        //public override Int32 Insert()
        //{
        //    return base.Insert();
        //}

        ///// <summary>已重载。在事务保护范围内处理业务，位于Valid之后</summary>
        ///// <returns></returns>
        //protected override Int32 OnInsert()
        //{
        //    return base.OnInsert();
        //}
        #endregion

        #region 扩展属性﻿
        public IManageUser Creater { get { return ManageProvider.Provider.FindByID(CreateUserID); } }

        public String CreateName { get { return Creater == null ? "" : Creater.ToString(); } }
        #endregion

        #region 扩展查询﻿
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
        // 以下为自定义高级查询的例子

        ///// <summary>
        ///// 查询满足条件的记录集，分页、排序
        ///// </summary>
        ///// <param name="key">关键字</param>
        ///// <param name="orderClause">排序，不带Order By</param>
        ///// <param name="startRowIndex">开始行，0表示第一行</param>
        ///// <param name="maximumRows">最大返回行数，0表示所有行</param>
        ///// <returns>实体集</returns>
        //[DataObjectMethod(DataObjectMethodType.Select, true)]
        //public static EntityList<WorkTask> Search(String key, String orderClause, Int32 startRowIndex, Int32 maximumRows)
        //{
        //    return FindAll(SearchWhere(key), orderClause, null, startRowIndex, maximumRows);
        //}

        ///// <summary>
        ///// 查询满足条件的记录总数，分页和排序无效，带参数是因为ObjectDataSource要求它跟Search统一
        ///// </summary>
        ///// <param name="key">关键字</param>
        ///// <param name="orderClause">排序，不带Order By</param>
        ///// <param name="startRowIndex">开始行，0表示第一行</param>
        ///// <param name="maximumRows">最大返回行数，0表示所有行</param>
        ///// <returns>记录数</returns>
        //public static Int32 SearchCount(String key, String orderClause, Int32 startRowIndex, Int32 maximumRows)
        //{
        //    return FindCount(SearchWhere(key), null, null, 0, 0);
        //}

        /// <summary>构造搜索条件</summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        private static String SearchWhere(String key)
        {
            // WhereExpression重载&和|运算符，作为And和Or的替代
            // SearchWhereByKeys系列方法用于构建针对字符串字段的模糊搜索
            var exp = SearchWhereByKeys(key, null);

            // 以下仅为演示，Field（继承自FieldItem）重载了==、!=、>、<、>=、<=等运算符（第4行）
            //if (userid > 0) exp &= _.OperatorID == userid;
            //if (isSign != null) exp &= _.IsSign == isSign.Value;
            //if (start > DateTime.MinValue) exp &= _.OccurTime >= start;
            //if (end > DateTime.MinValue) exp &= _.OccurTime < end.AddDays(1).Date;

            return exp;
        }
        #endregion

        #region 扩展操作
        #endregion

        #region 业务
        #endregion
    }
}