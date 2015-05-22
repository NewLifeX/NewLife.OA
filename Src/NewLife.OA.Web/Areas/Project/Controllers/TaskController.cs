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
        static TaskController()
        {
            // 过滤要显示的字段
            var names = "ID,Name,ParentName,Score,Priority,WorkStatus,Progress,MasterName,StartTime,UpdateName,UpdateTime".Split(",");
            var fs = WorkTask.Meta.AllFields;
            var list = names.Select(e => fs.FirstOrDefault(f => f.Name.EqualIgnoreCase(e))).Where(e => e != null);
            //list.RemoveAll(e => !names.Contains(e.Name));
            ListFields.Clear();
            ListFields.AddRange(list);
        }
    }
}