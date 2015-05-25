using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewLife.Cube;

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

        static TaskController()
        {
            // 过滤要显示的字段
            var names = "ID,Name,Score,TaskPriority,TaskStatus,Progress,MasterName,PlanStartTime,PlanEndTime,PlanCost,StartTime,UpdateUserName,UpdateTime".Split(",");
            var fs = WorkTask.Meta.AllFields;
            var list = names.Select(e => fs.FirstOrDefault(f => f.Name.EqualIgnoreCase(e))).Where(e => e != null);
            //list.RemoveAll(e => !names.Contains(e.Name));
            ListFields.Clear();
            ListFields.AddRange(list);
        }

        ///// <summary>验证实体对象</summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //protected override bool Valid(WorkTask entity)
        //{
        //    // 接收处理成员列表
        //    var mbs = Request["MemberIDs"].SplitAsInt();

        //    return base.Valid(entity);
        //}
    }
}