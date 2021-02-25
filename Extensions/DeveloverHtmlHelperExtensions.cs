using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Text;

namespace Develover.WebUI.Extensions
{
    public static class DeveloverHtmlHelperExtensions
    {
        public static IHtmlContent DeveloverLabel(this IHtmlHelper htmlHelper, string label, string @for = "", string labelClass = "")
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<label class=\"form-label mb-2 {labelClass}\" for=\"{@for}\">{label}</label>");

            return new HtmlString(sb.ToString());
        }
        public static IHtmlContent DeveloverCheckBox(this IHtmlHelper htmlHelper, string id, string label, string inputClass = "", string validation = "", string validationMessage = "")
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<div class=\"custom-control custom-checkbox mb-3\">");
            sb.Append($"<input type=\"checkbox\" class=\"custom-control-input {inputClass}\" id=\"{id}\" name=\"{id}\" {validation}>");
            sb.Append($"<label class=\"custom-control-label\" for=\"{id}\">{label}</label>");
            sb.Append($"<div class=\"invalid-feedback\">{validationMessage}</div>");
            sb.Append($"</div>");

            return new HtmlString(sb.ToString());
        }
        public static IHtmlContent DeveloverSwitch(this IHtmlHelper htmlHelper, string id, string label, string inputClass = "", string validation = "")
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<div class=\"custom-control custom-switch mb-3\">");
            sb.Append($"<input type=\"checkbox\" class=\"custom-control-input {inputClass}\" id=\"{id}\" name=\"{id}\" {validation}>");
            sb.Append($"<label class=\"custom-control-label\" for=\"{id}\">{label}</label>");
            sb.Append($"</div>");

            return new HtmlString(sb.ToString());
        }
        public static IHtmlContent DeveloverHidden(this IHtmlHelper htmlHelper, string id, string inputClass = "")
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<input type=\"hidden\" class=\"form-control {inputClass}\" id=\"{id}\" name=\"{id}\">");

            return new HtmlString(sb.ToString());
        }
        public static IHtmlContent DeveloverPassword(this IHtmlHelper htmlHelper, string id, string label, string inputClass = "", string placeholder = "", string validation = "", string validationMessage = "")
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<label class=\"form-label\" for=\"{id}\">{label}{(validation.Contains("required") ? "<span class=\"text-danger\"> *</span>" : "")}</label>");
            sb.Append($"<input type=\"password\" class=\"form-control {inputClass}\" id=\"{id}\" name=\"{id}\" placeholder=\"{placeholder}\" {validation}>");
            sb.Append($"<div class=\"invalid-feedback\">{validationMessage}</div>");

            return new HtmlString(sb.ToString());
        }
        public static IHtmlContent DeveloverEmail(this IHtmlHelper htmlHelper, string id, string label, string inputClass = "", string placeholder = "", string validation = "", string validationMessage = "")
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<label class=\"form-label\" for=\"{id}\">{label}{(validation.Contains("required") ? "<span class=\"text-danger\"> *</span>" : "")}</label>");
            sb.Append($"<input type=\"email\" class=\"form-control {inputClass}\" id=\"{id}\" name=\"{id}\" placeholder=\"{placeholder}\" {validation}>");
            sb.Append($"<div class=\"invalid-feedback\">{validationMessage}</div>");

            return new HtmlString(sb.ToString());
        }
        public static IHtmlContent DeveloverRadioButton(this IHtmlHelper htmlHelper, string id, string name, string label, string inputClass = "", string validation = "", string validationMessage = "")
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<div class=\"custom-control custom-radio mb-2\">");
            sb.Append($"<input type=\"radio\" class=\"custom-control-input {inputClass}\" id=\"{id}\" name=\"{name}\" {validation}>");
            sb.Append($"<label class=\"custom-control-label\" for=\"{id}\">{label}</label>");
            sb.Append($"<div class=\"invalid-feedback\">{validationMessage}</div>");
            sb.Append($"</div>");

            return new HtmlString(sb.ToString());
        }
        public static IHtmlContent DeveloverTextArea(this IHtmlHelper htmlHelper, string id, string label, int rows = 3, string inputClass = "", string placeholder = "", string validation = "", string validationMessage = "")
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<label class=\"form-label\" for=\"{id}\">{label}{(validation.Contains("required") ? "<span class=\"text-danger\"> *</span>" : "")}</label>");
            sb.Append($"<textarea type=\"text\" class=\"form-control {inputClass}\" id=\"{id}\" name=\"{id}\" rows={rows} placeholder=\"{placeholder}\" {validation}></textarea>");
            sb.Append($"<div class=\"invalid-feedback\">{validationMessage}</div>");

            return new HtmlString(sb.ToString());
        }
        public static IHtmlContent DeveloverTextBox(this IHtmlHelper htmlHelper, string id, string label, string inputClass = "", string placeholder = "", string validation = "", string validationMessage = "")
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<label class=\"form-label\" for=\"{id}\">{label}{(validation.Contains("required") ? "<span class=\"text-danger\"> *</span>" : "")}</label>");
            sb.Append($"<input type=\"text\" class=\"form-control {inputClass}\" id=\"{id}\" name=\"{id}\" placeholder=\"{placeholder}\" {validation}>");
            sb.Append($"<div class=\"invalid-feedback\">{validationMessage}</div>");

            return new HtmlString(sb.ToString());
        }
        public static IHtmlContent DeveloverSelect2(this IHtmlHelper html, string id, string label, string validation = "", string validationMessage = "")
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<label class=\"form-label\" for=\"{id}\">{label}{(validation.Contains("required") ? "<span class=\"text-danger\"> *</span>" : "")}</label>");
            sb.Append($"<select id = \"{id}\" name=\"{id}\" class=\"select2 form-control w-100\" {validation}>");
            sb.Append($"<option hidden disabled selected value=\"\"></option>");
            sb.Append($"<option value=\"df\">lsdfjldkjsf</option>");
            sb.Append($"</select>");
            sb.Append($"<div class=\"invalid-feedback\">{validationMessage}</div>");

            return new HtmlString(sb.ToString());
        }






        public static IHtmlContent DeveloverDetailButton(this IHtmlHelper html)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-new btn-outline-success btn-sm mr-1\" title=\"Thêm mới (Z+N)\" href=\"javascript:void(0);\" data-hotkey=\"Z+N\">");
            sb.Append($"<i class=\"fal fa-file-plus\"></i>");
            sb.Append($"<span> Thêm mới</span>");
            sb.Append($"</a>");
            sb.Append($"<a class=\"btn btn-edit btn-outline-warning btn-sm mr-1\" title=\"Sửa (Z+E)\" href=\"javascript:void(0);\" data-hotkey=\"Z+E\">");
            sb.Append($"<i class=\"fal fa-file-edit\"></i>");
            sb.Append($"<span> Sửa</span>");
            sb.Append($"</a>");
            sb.Append($"<a class=\"btn btn-save btn-outline-success btn-sm mr-1\" title=\"Lưu (Z+S)\" href=\"javascript:void(0);\" data-hotkey=\"Z+S\" style=\"display: none\">");
            sb.Append($"<i class=\"fal fa-save\"></i>");
            sb.Append($"<span> Lưu</span>");
            sb.Append($"</a>");
            sb.Append($"<a class=\"btn btn-cancel btn-outline-secondary btn-sm mr-1\" title=\"Huỷ bỏ (Z+C)\" href=\"javascript:void(0);\" data-hotkey=\"Z+C\" style=\"display: none\">");
            sb.Append($"<i class=\"fal fa-ban\"></i>");
            sb.Append($"<span> Huỷ</span>");
            sb.Append($"</a>");
            sb.Append($"<a class=\"btn btn-delete btn-outline-danger btn-sm float-right\" title=\"Xoá (Z+D)\" href=\"javascript:void(0);\" data-hotkey=\"Z+D\">");
            sb.Append($"<i class=\"fal fa-times\"></i>");
            sb.Append($"<span> Xoá</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }
        public static IHtmlContent DeveloverNewButton(this IHtmlHelper html, string onClickCallBack)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-new btn-outline-success btn-sm\" title=\"Thêm mới (Z+N)\" href=\"javascript:void(0);\" data-hotkey=\"Z+N\" onclick=\"{onClickCallBack}\">");
            sb.Append($"<i class=\"fal fa-file-plus\"></i>");
            sb.Append($"<span> Thêm mới</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent DeveloverEditButton(this IHtmlHelper html, string onClickCallBack)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-edit btn-outline-warning btn-sm\" title=\"Sửa (Z+E)\" href=\"javascript:void(0);\" data-hotkey=\"Z+E\" onclick=\"{onClickCallBack}\">");
            sb.Append($"<i class=\"fal fa-file-edit\"></i>");
            sb.Append($"<span> Sửa</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent DeveloverSaveButton(this IHtmlHelper html, string onClickCallBack)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-save btn-outline-success btn-sm\" title=\"Lưu (Z+S)\" href=\"javascript:void(0);\" data-hotkey=\"Z+S\" onclick=\"{onClickCallBack}\" style=\"display: none\">");
            sb.Append($"<i class=\"fal fa-save\"></i>");
            sb.Append($"<span> Lưu</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent DeveloverCancelButton(this IHtmlHelper html, string onClickCallBack)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-cancel btn-outline-secondary btn-sm\" title=\"Huỷ bỏ (Z+C)\" href=\"javascript:void(0);\" data-hotkey=\"Z+C\" onclick=\"{onClickCallBack}\" style=\"display: none\">");
            sb.Append($"<i class=\"fal fa-ban\"></i>");
            sb.Append($"<span> Huỷ</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }

        public static IHtmlContent DeveloverDeleteButton(this IHtmlHelper html, string onClickCallBack)
        {
            if (html == null) throw new ArgumentNullException(nameof(html));

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a class=\"btn btn-delete btn-outline-danger btn-sm float-right\" title=\"Xoá (Z+D)\" href=\"javascript:void(0);\" data-hotkey=\"Z+D\" onclick=\"{onClickCallBack}\">");
            sb.Append($"<i class=\"fal fa-times\"></i>");
            sb.Append($"<span> Xoá</span>");
            sb.Append($"</a>");

            return new HtmlString(sb.ToString());
        }
    }
}
