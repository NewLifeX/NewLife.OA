using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using XCode;

namespace NewLife.OA
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString DateTimeFor(this HtmlHelper htmlHelper, String name, DateTime value, String format = null, Object htmlAttributes = null)
        {
            var atts = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (!atts.ContainsKey("type")) atts.Add("type", "date");
            if (!atts.ContainsKey("class")) atts.Add("class", "form-control date form_datetime");

            var obj = value.ToFullString();
            if (value <= DateTime.MinValue) obj = null;
            //if (format.IsNullOrWhiteSpace()) format = "yyyy-MM-dd HH:mm:ss";

            // 首先输出图标
            var ico = htmlHelper.Raw("<span class=\"input-group-addon\"><i class=\"fa fa-calendar\"></i></span>");

            var txt = htmlHelper.TextBox(name, obj, format, atts);

            return new MvcHtmlString(ico.ToString() + txt.ToString());
        }

        public static MvcHtmlString DateTimeFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, String format = null, Object htmlAttributes = null)
        {
            var meta = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var entity = htmlHelper.ViewData.Model as IEntity;
            var value = (DateTime)entity[meta.PropertyName];

            return htmlHelper.DateTimeFor(meta.PropertyName, value, format, htmlAttributes);
        }
    }
}