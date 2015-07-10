using System;
using System.Web.Mvc;

namespace Hexon.MvcTrig
{
    public static class ControllerExtensions
    {
        public static TriggerHelper Trig(this Controller controller, Func<TriggerHelper, TriggerHelper> invocation = null)
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