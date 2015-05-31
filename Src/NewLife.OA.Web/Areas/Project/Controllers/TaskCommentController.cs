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
            ViewBag.Task = WorkTask.FindByID(id ?? 0);
            var list = TaskComment.Search(id ?? 0, p);

            ViewBag.HeaderTitle = null;
            ViewBag.HeaderContent = null;

            return View(list);
        }

        [ValidateInput(false)]
        public ActionResult AddComment(Int32 id,String content)
        {
            var entity = new TaskComment();
            entity.TaskID = id;
            entity.Content = content;
            entity.Insert();

            return RedirectToAction("Show", new { id = entity.TaskID });
        }
    }
}