using System;
using System.Web.WebPages;

namespace Hexon.MvcTrig
{
    public static class WebPageBaseExtensions
    {
        public static TriggerContext Trig(this WebPageBase controller, Func<TriggerContext, TriggerContext> invocation = null)
        {
            var trig = TriggerContext.Current;

            if (invocation != null)
            {
                return invocation(trig);
            }

            return trig;
        }
    }
}