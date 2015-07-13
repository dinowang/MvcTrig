using System;
using System.Web.Mvc;

namespace Hexon.MvcTrig
{
    public static class ControllerExtensions
    {
        public static TriggerContext Trig(this Controller controller, Func<TriggerContext, TriggerContext> invocation = null)
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