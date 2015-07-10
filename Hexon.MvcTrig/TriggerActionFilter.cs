using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hexon.MvcTrig
{
    public class TriggerActionFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (! filterContext.Controller.ControllerContext.IsChildAction && TriggerHelper.HasTrigger)
            {
                var httpContext = filterContext.RequestContext.HttpContext;
                var response = httpContext.Response;

                if (response.StatusCode == 302)
                {
                    httpContext.Session["TriggerHelper"] = TriggerHelper.Current;
                }
                else
                {
                    TriggerHelper.Current.Flush();
                }
            }
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }
    }
}