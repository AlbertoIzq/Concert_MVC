using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concert.Utility
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Set currency symbol to euro
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(
            ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var cultureInfo = (CultureInfo)CultureInfo.GetCultureInfo("es").Clone();
            cultureInfo.NumberFormat.CurrencySymbol = "€";
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
}
