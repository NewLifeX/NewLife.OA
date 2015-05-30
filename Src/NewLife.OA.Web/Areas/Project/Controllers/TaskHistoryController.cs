using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewLife.Cube;
using NewLife.Web;

namespace NewLife.OA.Web.Areas.Project.Controllers
{
    [DisplayName("任务历史")]
    public class TaskHistoryController : EntityController<TaskHistory>
    {
        //protected override ActionResult IndexView(Pager p)
        //{
        //    var id = RouteData.Values["id"].ToInt();
        //    var list = TaskHistory.Search(id, p);

        //    return View("List", list);
        //}

        public ActionResult Show(Int32? id, Pager p)
        {
            var list = TaskHistory.Search(id ?? 0, p);

            return View(list);
        }
    }
}