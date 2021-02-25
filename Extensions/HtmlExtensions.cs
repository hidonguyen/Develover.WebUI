using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Develover.WebUI.Extensions
{
    public static class HtmlExtensions
    {
        public static IHtmlContent SelectPickerFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, dynamic model, IList<SelectListItem> options)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            if (model == null) throw new ArgumentNullException(nameof(model));
            if (options == null) throw new ArgumentNullException(nameof(options));

            Func<TModel, TValue> method = expression.Compile();
            TValue value = method(html.ViewData.Model);

            var id = ((MemberExpression)expression.Body).Member.Name;

            var selectSize = model.GetType().GetProperty("selectSize").GetValue(model, null);
            var target = model.GetType().GetProperty("target").GetValue(model, null);
            var postbackClass = model.GetType().GetProperty("postbackClass").GetValue(model, null);

            StringBuilder sb = new StringBuilder();
            sb.Append($"<div class=\"field-group {selectSize}\">");
            sb.Append($"<div class=\"float-left w-91\">");
            sb.Append($"<select id = \"{id}\" name=\"{id}\" class=\"selectpicker form-control form-control-sm py-0 {postbackClass}\">");


            if (default(TValue) == null)
            {
                sb.Append($"<option selected value=\"\"></option>");
            }
            else
            {
                sb.Append($"<option hidden disabled selected value=\"\"></option>");
            }

            foreach (var option in options)
            {
                sb.Append($"<option value=\"{option.Value}\" {(option.Value.Equals(value?.ToString(), StringComparison.OrdinalIgnoreCase) ? "selected" : null)}>{option.Text}</option>");
            }
            sb.Append($"</select>");
            sb.Append($"<div class=\"invalid-feedback\"></div>");
            sb.Append($"</div>");
            sb.Append($"<a class=\"btn btn-sm btn-primary float-left new-catalog-shortcut\" href=\"javascript:void(0);\" title=\"New Catalog Shortcut\" data-toggle=\"modal\" data-target=\"#{target}\" data-postback-id=\"{id}\" data-postback-class=\"{postbackClass}\">");
            sb.Append($"<i class=\"fas fa-plus\"></i>");
            sb.Append($"</a>");
            sb.Append($"</div>");

            return new HtmlString(sb.ToString());
        }
        
        public static IHtmlContent SelectPickerFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IList<SelectListItem> options)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            if (options == null) throw new ArgumentNullException(nameof(options));

            Func<TModel, TValue> method = expression.Compile();
            TValue value = method(html.ViewData.Model);

            var id = ((MemberExpression)expression.Body).Member.Name;

            StringBuilder sb = new StringBuilder();
            sb.Append($"<select id = \"{id}\" name=\"{id}\" class=\"selectpicker form-control form-control-sm py-0\">");

            if (default(TValue) == null)
            {
                sb.Append($"<option selected value=\"\"></option>");
            }
            else
            {
                sb.Append($"<option hidden disabled selected value=\"\"></option>");
            }

            foreach (var option in options)
            {
                sb.Append($"<option value=\"{option.Value}\" {(option.Value.Equals(value?.ToString(), StringComparison.OrdinalIgnoreCase) ? "selected" : null)}>{option.Text}</option>");
            }
            sb.Append($"</select>");
            sb.Append($"<div class=\"invalid-feedback\"></div>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent SelectPicker<TModel>(this IHtmlHelper<TModel> html, string IdControl, dynamic model, IList<SelectListItem> options)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            if (model == null) throw new ArgumentNullException(nameof(model));
            if (options == null) throw new ArgumentNullException(nameof(options));


            var id = IdControl;

            var selectSize = model.GetType().GetProperty("selectSize").GetValue(model, null);
            var target = model.GetType().GetProperty("target").GetValue(model, null);
            var postbackClass = model.GetType().GetProperty("postbackClass").GetValue(model, null);

            StringBuilder sb = new StringBuilder();
            sb.Append($"<div class=\"field-group {selectSize}\">");
            sb.Append($"<div class=\"float-left w-91\">");
            sb.Append($"<select id = \"{id}\" name=\"{id}\" class=\"selectpicker form-control form-control-sm py-0 {postbackClass}\">");
            sb.Append($"<option hidden disabled selected value=\"\"></option>");
            foreach (var option in options)
            {
                sb.Append($"<option value=\"{option.Value}\" {(option.Value.Equals(Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase) ? "selected" : null)}>{option.Text}</option>");
            }
            sb.Append($"</select>");
            sb.Append($"<div class=\"invalid-feedback\"></div>");
            sb.Append($"</div>");
            sb.Append($"<a class=\"btn btn-sm btn-primary float-left new-catalog-shortcut\" title=\"New Catalog Shortcut\" href=\"javascript:void(0);\" data-toggle=\"modal\" data-target=\"#{target}\" data-postback-id=\"{id}\" data-postback-class=\"{postbackClass}\">");
            sb.Append($"<i class=\"fas fa-plus\"></i>");
            sb.Append($"</a>");
            sb.Append($"</div>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent OptionalSelectPicker<TModel>(this IHtmlHelper<TModel> html, string IdControl, dynamic model, IList<SelectListItem> options)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            if (model == null) throw new ArgumentNullException(nameof(model));
            if (options == null) throw new ArgumentNullException(nameof(options));


            var id = IdControl;

            var selectSize = model.GetType().GetProperty("selectSize").GetValue(model, null);
            var target = model.GetType().GetProperty("target").GetValue(model, null);
            var postbackClass = model.GetType().GetProperty("postbackClass").GetValue(model, null);

            StringBuilder sb = new StringBuilder();
            sb.Append($"<div class=\"field-group {selectSize}\">");
            sb.Append($"<div class=\"float-left w-91\">");
            sb.Append($"<select id = \"{id}\" name=\"{id}\" class=\"selectpicker form-control form-control-sm py-0 {postbackClass}\">");
            sb.Append($"<option selected value=\"\"></option>");
            foreach (var option in options)
            {
                sb.Append($"<option value=\"{option.Value}\" {(option.Value.Equals(Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase) ? "selected" : null)}>{option.Text}</option>");
            }
            sb.Append($"</select>");
            sb.Append($"<div class=\"invalid-feedback\"></div>");
            sb.Append($"</div>");
            sb.Append($"<a class=\"btn btn-sm btn-primary float-left new-catalog-shortcut\" title=\"New Catalog Shortcut\" href=\"javascript:void(0);\" data-toggle=\"modal\" data-target=\"#{target}\" data-postback-id=\"{id}\" data-postback-class=\"{postbackClass}\">");
            sb.Append($"<i class=\"fas fa-plus\"></i>");
            sb.Append($"</a>");
            sb.Append($"</div>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent SelectPicker<TModel>(this IHtmlHelper<TModel> html, string IdControl, IList<SelectListItem> options)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            if (options == null) throw new ArgumentNullException(nameof(options));


            var id = IdControl;

            StringBuilder sb = new StringBuilder();
            sb.Append($"<select id = \"{id}\" name=\"{id}\" class=\"selectpicker form-control form-control-sm py-0\">");
            sb.Append($"<option hidden disabled selected value=\"\"></option>");
            foreach (var option in options)
            {
                sb.Append($"<option value=\"{option.Value}\" {(option.Value.Equals(Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase) ? "selected" : null)}>{option.Text}</option>");
            }
            sb.Append($"</select>");
            sb.Append($"<div class=\"invalid-feedback\"></div>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent OptionalSelectPicker<TModel>(this IHtmlHelper<TModel> html, string IdControl, IList<SelectListItem> options)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            if (options == null) throw new ArgumentNullException(nameof(options));


            var id = IdControl;

            StringBuilder sb = new StringBuilder();
            sb.Append($"<select id = \"{id}\" name=\"{id}\" class=\"selectpicker form-control form-control-sm py-0\">");
            sb.Append($"<option selected value=\"\"></option>");
            foreach (var option in options)
            {
                sb.Append($"<option value=\"{option.Value}\" {(option.Value.Equals(Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase) ? "selected" : null)}>{option.Text}</option>");
            }
            sb.Append($"</select>");
            sb.Append($"<div class=\"invalid-feedback\"></div>");

            return new HtmlString(sb.ToString());
        }

        public static string CurrentViewName(this HtmlHelper html)
        {
            return System.IO.Path.GetFileNameWithoutExtension(
                ((RazorView)html.ViewContext.View).Path
            );
        }

        public static IHtmlContent CancelButton(this IHtmlHelper html)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-outline-secondary btn-sm\" title=\"Cancel (Z+C)\" href=\"javascript:void(0);\" data-hotkey=\"Z+C\" onclick=\"history.back();\">");
            sb.Append($"<i class=\"fal fa-ban\"></i>");
            sb.Append($"<span> Cancel</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent NewButton(this IHtmlHelper html, string href)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-outline-success btn-sm\" title=\"New (Z+N)\" href=\"{href}\" data-hotkey=\"Z+N\">");
            sb.Append($"<i class=\"fal fa-file-plus\"></i>");
            sb.Append($"<span> New</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent SaveButton(this IHtmlHelper html, string onClickCallBack, string formId)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-outline-success btn-sm\" title=\"Save (Z+S)\" href=\"javascript:void(0);\" data-hotkey=\"Z+S\" onclick=\"{onClickCallBack}($('#{formId}'));\">");
            sb.Append($"<i class=\"fal fa-save\"></i>");
            sb.Append($"<span> Save</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent SaveButton(this IHtmlHelper html, string onClickCallBack)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-outline-success btn-sm\" title=\"Save (Z+S)\" href=\"javascript:void(0);\" data-hotkey=\"Z+S\" onclick=\"{onClickCallBack}\">");
            sb.Append($"<i class=\"fal fa-save\"></i>");
            sb.Append($"<span> Save</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent EditButton(this IHtmlHelper html, string href)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-outline-warning btn-sm\" title=\"Edit (Z+E)\" href=\"{href}\" data-hotkey=\"Z+E\">");
            sb.Append($"<i class=\"fal fa-file-edit\"></i>");
            sb.Append($"<span> Edit</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent DeleteButton(this IHtmlHelper html, string onClickCallBack, string formId)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-outline-danger btn-sm\" title=\"Delete (Z+D)\" href=\"javascript:void(0);\" data-hotkey=\"Z+D\" onclick=\"{onClickCallBack}($('#{formId}'));\">");
            sb.Append($"<i class=\"fal fa-times\"></i>");
            sb.Append($"<span> Delete</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent DeleteButton(this IHtmlHelper html, string onClickCallBack)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-outline-danger btn-sm float-right\" title=\"Delete (Z+D)\" href=\"javascript:void(0);\" data-hotkey=\"Z+D\" onclick=\"{onClickCallBack}\">");
            sb.Append($"<i class=\"fal fa-times\"></i>");
            sb.Append($"<span> Delete</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent CloseModalButton(this IHtmlHelper html, string modalId)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-sm btn-secondary\" title=\"Cancel (Z+C)\" href=\"javascript:void(0);\" data-hotkey=\"ESC\" onclick=\"$('#{modalId}').modal('hide');\">");
            sb.Append($"<i class=\"fal fa-ban\"></i>");
            sb.Append($"<span>Close</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent CloseModalButtonHeader(this IHtmlHelper html, string modalId)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"close\" title=\"Close (Z+C)\" href=\"javascript:void(0);\" data-hotkey=\"ESC\" onclick=\"$('#{modalId}').modal('hide');\" aria-label=\"Close\">");
            sb.Append($"<span aria-hidden=\"true\">&times;</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent AddModalButton(this IHtmlHelper html, string onClickCallBack, string modalId, string formId)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-sm btn-primary save\" title=\"Add (Z+A)\" href=\"javascript:void(0);\" data-hotkey=\"Z+A\" onclick=\"{onClickCallBack}($('#{formId}'),$('#{modalId}'));\">");
            sb.Append($"<i class=\"fal fa-plus\"></i>");
            sb.Append($"<span>Add</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent AddCloseModalButton(this IHtmlHelper html, string onClickCallBack, string modalId, string formId)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-sm btn-primary save\" title=\"Add (Z+A)\" href=\"javascript:void(0);\" data-hotkey=\"Z+A\" onclick=\"{onClickCallBack}($('#{formId}'),$('#{modalId}'), true);\">");
            sb.Append($"<i class=\"fal fa-plus\"></i>");
            sb.Append($"<span>Add</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }
        public static IHtmlContent AddItem(this IHtmlHelper html, string target)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-sm text-primary add-item float-left\" title=\"Add item (Z+I)\" href=\"javascript:void(0);\" data-hotkey=\"Z+I\" data-toggle=\"modal\" data-target=\"#{target}\" onclick=\"this.blur();\">");
            sb.Append($"<i class=\"fas fa-plus-circle\"></i>");
            sb.Append($"<span>Add item</span>");
            sb.Append($"</a>");
            return new HtmlString(sb.ToString());
        }
    }
}
