using System;
using System.Web.WebPages;

namespace Hexon.MvcTrig
{
    public static class WebPageBaseExtensions
    {
        public static TriggerHelper Trig(this WebPageBase controller, Func<TriggerHelper, TriggerHelper> invocation = null)
        {
            var trig = TriggerHelper.Current;

            if (invocation != null)
            {
                return invocation(trig);
            }

            return trig;
        }
    }
}