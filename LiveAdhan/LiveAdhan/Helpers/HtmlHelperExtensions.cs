using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString EnumDropdownListFor<T, TProperty>(
            this HtmlHelper<T> htmlHelper,
            Expression<Func<T, TProperty>> expression,
            object htmlAttributes)
        {
            var selectedItem = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            var dropdownList = Enum.GetValues(typeof(TProperty)).Cast<TProperty>().Select(e => new SelectListItem
            {
                Text = e.ToString(),
                Value = Convert.ToInt32(e).ToString(),
                Selected = e.Equals(selectedItem)
            });
            return htmlHelper.DropDownListFor(expression, dropdownList, htmlAttributes);
        }

        public static MvcHtmlString EnumDropdownListFor<T, TProperty>(
            this HtmlHelper<T> htmlHelper,
            Expression<Func<T, TProperty>> expression)
        {
            return EnumDropdownListFor(htmlHelper, expression, null);
        }

        public static Dictionary<String, EnumRadioButton> EnumRadioButtonsFor<T, TProperty>(
            this HtmlHelper<T> htmlHelper,
            Expression<Func<T, TProperty>> expression)
        {
            var result = new Dictionary<String, EnumRadioButton>();

            foreach (var enumValue in Enum.GetValues(typeof(TProperty)).Cast<TProperty>())
            {
                var id = typeof(TProperty).Name + "_" + Convert.ToInt32(enumValue).ToString();
                var html = htmlHelper.RadioButtonFor(expression, enumValue.ToString(), new { id = id });
                result[id] = new EnumRadioButton
                {
                    Html = html,
                    EnumLabel = enumValue.ToString()
                };
            }
            return result;
        }
    }
    public class EnumRadioButton
    {
        public String EnumLabel { get; set; }
        public MvcHtmlString Html { get; set; }
    }
}
