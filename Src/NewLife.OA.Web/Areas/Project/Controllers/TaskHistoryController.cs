using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewLife.Cube;
using NewLife.Web;

namespace NewLife.OA.Web.Areas.Project.Controllers
{
    public class TaskHistoryController : EntityController<TaskHistory>
    {
        protected override ActionResult IndexView(Pager p)
        {
            var id = RouteData.Values["id"].ToInt();
            var list = TaskHistory.Search(id, p);

            return View("List", list);
        }
    }
}