/*
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
using NewLife.Log;
using NewLife.Web;
using XCode;
using XCode.Configuration;
using XCode.Membership;

namespace NewLife.OA
{
    /// <summary>任务状态</summary>
    /// <remarks>
    /// 准备 => 进行
    /// 进行 => 准备、暂停、取消、完成
    /// 暂停 => 进行
    /// 取消 => 进行
    /// 完成 => 进行
    /// </remarks>
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
            // 新建也有份
            if (!forEdit)
            {
                entity.Score = 1;

                var user = ManageProvider.User;
                if (user != null)
                {
                    entity.MasterID = ManageProvider.User.ID;

                    entity.CreateUserID = ManageProvider.User.ID;
                }

                entity.CreateTime = DateTime.Now;

                // 清除脏数据
                entity.Dirtys.Clear();
            }
            return entity;
        }

        /// <summary>验证数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew"></param>
        public override void Valid(Boolean isNew)
        {
            if (!HasDirty) return;

            if (String.IsNullOrEmpty(Name)) throw new ArgumentNullException(_.Name, _.Name.DisplayName + "不能为空！");
            if (Score <= 0) throw new ArgumentNullException(_.Score, _.Score.DisplayName + "最小为1！");
            if (MasterID <= 0) throw new ArgumentNullException(_.MasterID, _.MasterID.DisplayName + "不能为空！");
            if (PlanStartTime <= DateTime.MinValue) throw new ArgumentNullException(_.PlanStartTime, _.PlanStartTime.DisplayName + "不能为空！");
            if (PlanEndTime <= DateTime.MinValue) throw new ArgumentNullException(_.PlanEndTime, _.PlanEndTime.DisplayName + "不能为空！");

            if (PlanStartTime > PlanEndTime) throw new ArgumentOutOfRangeException(_.PlanEndTime, _.PlanEndTime.DisplayName + "不能大于开始时间！");

            // 检查计划时间范围，如果父级锁定，自己不得超过父级
            if (Parent != null && Parent.LockPlanTime)
            {
                if (PlanStartTime < Parent.PlanStartTime)
                    throw new ArgumentOutOfRangeException(_.PlanStartTime, _.PlanStartTime.DisplayName + "不能超过已锁定的父级开始时间{0}！".F(Parent.PlanStartTime));
                if (PlanEndTime > Parent.PlanEndTime)
                    throw new ArgumentOutOfRangeException(_.PlanEndTime, _.PlanEndTime.DisplayName + "不能超过已锁定的父级结束时间{0}！".F(Parent.PlanEndTime));
            }

            if (_bak != null && _bak.TaskStatus != TaskStatus.进行 && Dirtys[__.Progress]) throw new ArgumentException(__.Status, "只有[{0}]的任务才允许修改进度".F(TaskStatus.进行));

            // 准备、进行中 两种状态以外的状态，不得修改状态和已删除以外的字段
            if (!Dirtys[__.Status] && _bak != null && _bak.TaskStatus != TaskStatus.准备 && _bak.TaskStatus != TaskStatus.进行)
            {
                foreach (var item in Meta.FieldNames)
                {
                    if (item.EqualIgnoreCase(__.Status, __.Deleted)) continue;

                    if (Dirtys[item]) throw new XException("处于[{0}]状态时禁止修改[{1}]", _bak.TaskStatus, item);
                }
            }

            // 建议先调用基类方法，基类方法会对唯一索引的数据进行验证
            base.Valid(isNew);

            // 计算计划工作日，采取进一法
            PlanCost = (Int32)Math.Ceiling((PlanEndTime - PlanStartTime).TotalDays);

            // 计算实际工作日
            if (EndTime > DateTime.MinValue && StartTime > DateTime.MinValue) Cost = (Int32)Math.Ceiling((EndTime - StartTime).TotalDays);

            FixChildCount();

            // 外部没有修改时，修正本级子任务数
            if (!isNew && !Dirtys[__.ChildCount]) ChildCount = Childs.Count;
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

            _bak = this.CloneEntity();

            TaskHistory.Add(ID, "创建", null, Name);

            rs += OnUpdate();

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
            _bak = this.CloneEntity();

            return rs;
        }

        protected override int OnDelete()
        {
            //var rs = base.OnDelete();
            var ori = Deleted;
            Deleted = !Deleted;
            var rs = base.OnUpdate();

            TaskHistory.Add(ID, ori ? "恢复" : "删除", null, Name);

            FixChildCount();

            // 子孙任务集体删除
            if (!ori)
            {
                foreach (var item in Childs)
                {
                    // 有些子任务已经被删除，这里先把它改为跟当前任务的原状态，方便一体化操作
                    item[__.Deleted] = ori;
                    item.Delete();
                }
            }
            else
            // 父任务集体恢复
            {
                if (Parent != null)
                {
                    Parent[__.Deleted] = ori;
                    Parent.Delete();
                }
            }

            return rs;
        }

        protected override void OnPropertyChanged(string fieldName)
        {
            if (fieldName.EqualIgnoreCase(__.ParentID, __.Deleted))
            {
                _Parent = null;
                Dirtys.Remove("Parent");
            }

            base.OnPropertyChanged(fieldName);
        }
        #endregion

        #region 扩展属性﻿
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

        /// <summary>子任务集合</summary>
        public EntityList<WorkTask> Childs { get { return ID > 0 ? FindAllByParentID(ID) : new EntityList<WorkTask>(); } }

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
            if (id <= 0) return null;

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
                return FindAll(_.ParentID == parentid & _.Deleted.IsTrue(false), null, null, 0, 0);
            else // 实体缓存
                return Meta.Cache.Entities.FindAll(e => e.ParentID == parentid && e.Deleted == false);
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
                if (!key.IsNullOrEmpty() &&
                    (item.Name.IsNullOrEmpty() || !item.Name.Contains(key)) &&
                    (item.Content.IsNullOrEmpty() || !item.Content.Contains(key))) continue;

                list.Add(item);
                if (item.ChildCount > 0)
                {
                    var childs = item.Childs;
                    if (childs.Count > 0)
                    {
                        // 同级子任务按照最后更新时间降序
                        var childs2 = childs.Clone();
                        childs2.Sort((x, y) => y.UpdateTime.CompareTo(x.UpdateTime));

                        var list2 = Expand(childs2, status, tps, masterid, start, end, deleted, key);

                        list.AddRange(list2);
                    }
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
            if (_bak == null) throw new XException("非法更新任务{0}！", ID);

            // 找到旧有数据
            var entity = _bak;

            var names = new Field[] { _.Name, _.ParentID, _.ChildCount, _.Percent, _.PlanStartTime, _.PlanEndTime, _.Progress, _.Score, _.LockScore, _.LockPlanTime };
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

        /// <summary>修正父任务的子任务数</summary>
        void FixChildCount()
        {
            // 统计父任务的子任务数，如果是新增任务，则加一
            if (Parent != null)
            {
                var count = Parent.Childs.Count;
                if (ID == 0) count++;
                if (Parent.ChildCount != count)
                {
                    //XTrace.WriteLine("父任务{0}的子任务数修改为{1}", ParentID, count);
                    Parent.ChildCount = count;
                    Parent.Save();
                }
            }
        }

        /// <summary>设定任务状态</summary>
        /// <param name="status"></param>
        public String SetStatus(TaskStatus status)
        {
            // 如果任务状态相同，则跳出
            if (TaskStatus == status) return null;

            // 状态切换有效性检查
            if (TaskStatus == TaskStatus.进行)
            {
                // 进行中状态可以切换到任何其它状态

                EndTime = DateTime.Now;

                // 如果任务已完成，则100%完成度
                if (status == TaskStatus.完成) Progress = 100;
            }
            else
            {
                // 其它状态只能切换到进行中状态
                if (status != TaskStatus.进行) throw new XException("当前状态[{0}]不能切换到[{1}]状态", TaskStatus, status);

                //if (TaskStatus == TaskStatus.取消) Deleted = false;

                // 重新计算时间
                StartTime = DateTime.Now;
            }

            TaskStatus = status;

            switch (status)
            {
                case TaskStatus.准备:
                    return "还原到草稿状态";
                case TaskStatus.进行:
                    return "成功开启任务";
                case TaskStatus.暂停:
                    return "成功暂停任务";
                case TaskStatus.取消:
                    return "取消成功，任务已删除";
                case TaskStatus.完成:
                    return "任务已完成，进度100%";
                default:
                    return null;
            }
        }
        #endregion

        #region 积分设定
        /// <summary>向下设定积分</summary>
        public void FixScoreDown()
        {
            // 计算新旧积分的百分比
            var p = (Double)Score / _bak.Score;

            // 子任务的积分全部乘以这个百分比，但是子任务的积分权重不变
            var total = 0;
            for (int i = 0; i < Childs.Count; i++)
            {
                var item = Childs[i];

                // 修正最后一个子任务，确保积分总和
                if (i == Childs.Count - 1)
                    item.Score = this.Score - total;
                else
                    item.Score = (Int32)(p * item.Score);

                // 递归修正子级任务
                item.FixScoreDown();

                item.Save();

                total += item.Score;
            }
        }

        /// <summary>修正积分</summary>
        /// <remarks>
        /// 采取向上向下两个参数的设计，完全避免可能出现死循环的问题。
        /// 第一层调用可以使用两个true，然后内部递归只用一个true
        /// </remarks>
        /// <param name="up">向上修正</param>
        /// <param name="down">向下修正</param>
        public Int32 FixScore(Boolean up, Boolean down)
        {
            var rs = 0;

            // 需要使用事务保护
            using (var trans = Meta.CreateTrans())
            {
                var ori = 0;
                if (_bak != null) ori = _bak.Score;

                // 首先保存当前级别，上下级统计需要用到
                if (!Deleted) rs += this.Save();

                // 积分的改变，首先会影响上一级
                var parent = Parent;
                if (parent != null)
                {
                    // 累加同级子任务的积分
                    var total = parent.Childs.ToList().Sum(e => e.Score);

                    // 子任务积分总和超过父任务积分，才保存更新
                    if (total > parent.Score)
                    {
                        // 如果超过父任务积分且锁定了积分，则报错
                        if (parent.LockScore)
                            throw new XException("任务[{0}]{1}的积分锁定为{2:n0}，但子任务积分总和为{3:n0}", parent.ID, parent.Name, parent.Score, total);

                        parent.Score = total;
                        //rs += parent.Save();

                        // 上一级递归
                        rs += parent.FixScore(true, false);
                    }
                }

                // 积分的缩小，会等比例缩小子任务积分，扩大则不影响子任务
                if (ori > Score)
                {
                    // 计算缩减的比例
                    var p = (Double)Score / ori;
                    // 累加，因为比例系数的原因，可能导致子任务积分总和略小于父任务积分，这里把剩余量加在最后一个子任务上
                    //var total = 0;
                    // 不行，不能这样子把剩余量加在最后一个子任务，因为父任务积分本来就很有可能超过子任务积分总和
                    for (int i = 0; i < Childs.Count; i++)
                    {
                        var item = Childs[i];

                        // 等比例缩小积分
                        //if (i == Childs.Count - 1)
                        //    item.Score = Score - total;
                        //else
                        item.Score = (Int32)(item.Score * p);

                        rs += item.FixScore(false, true);
                    }
                }

                trans.Commit();
            }

            return rs;
        }

        /// <summary>修正百分比。紧跟在积分之后，根据积分进行计算，确保同级任务百分比总和为100</summary>
        public void FixPercent()
        {
            var parent = Parent;
            // 顶级任务100%
            if (parent == null)
            {
                Percent = 100;
                return;
            }

            var list = parent.Childs.Clone();
            var total = 0;
            // 如果当前是新增任务，则累加进去
            if (ID == 0) list.Add(this);

            // 必须固定排序，否则计算百分比剩余量的时候，会变来变去
            list.Sort((x, y) => x.ID.CompareTo(y.ID));

            // 首先算一算总分
            foreach (var item in list)
            {
                total += item.Score;
            }

            // 其次开始计算百分比
            var tp = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];

                var p = (Int32)((Double)item.Score / total * 100);

                if (tp < 100)
                {
                    // 修正最后一个子任务，确保积分总和
                    if (i == list.Count - 1)
                        item.Percent = 100 - tp;
                    else
                    {
                        item.Percent = p;
                        tp += p;
                    }
                }
                else // 不够分配了
                    item.Percent = 0;

                // 不用保存自己，留给外部吧
                //if (item.ID > 0) item.Update();
                if (item != this) item.OnUpdate();
            }
        }

        /// <summary>修正父任务进度。</summary>
        public void FixParentProgress()
        {
            var parent = Parent;
            // 顶级任务
            if (parent == null) return;

            // 进度
            var progress = parent.Childs.ToList().Sum(e => e.Percent * e.Progress);
            parent.Progress = progress / 100;
            parent.Save();

            parent.FixParentProgress();
        }
        #endregion
    }
}