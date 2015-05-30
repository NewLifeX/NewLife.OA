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
    [DisplayName("任务评论")]
    public class TaskCommentController : EntityController<TaskComment>
    {
        //protected override ActionResult IndexView(Pager p)
        //{
        //    var id = RouteData.Values["id"].ToInt();
        //    var list = TaskComment.Search(id, p);

        //    return View("List", list);
        //}

        public ActionResult Show(Int32? id, Pager p)
        {
            var list = TaskComment.Search(id ?? 0, p);

            return View(list);
        }
    }
}