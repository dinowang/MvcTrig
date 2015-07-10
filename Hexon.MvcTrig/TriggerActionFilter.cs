using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hexon.MvcTrig
{
    public class TriggerActionFilter : ActionFilterAttribute
    {
        private bool _flush = false;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _flush = !filterContext.IsChildAction;

            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (_flush && filterContext.ParentActionViewContext == null && TriggerHelper.HasTrigger)
            {
                var httpContext = filterContext.RequestContext.HttpContext;
                var response = httpContext.Response;

                if (response.StatusCode == 302)
                {
                    // HTTP Redirect
                    httpContext.Session["TriggerHelper"] = TriggerHelper.Current;
                }
                else
                {
                    var routeData = filterContext.RouteData;

                    TriggerHelper.Current.Tag = string.Concat(routeData.Values["controller"], ".", routeData.Values["action"]);
                    TriggerHelper.Current.Flush();
                }
            }
        }
    }
}