using System.Web.Mvc;
using Hexon.MvcTrig.Fancybox;

namespace Hexon.MvcTrig.Sample.Controllers
{
    public class FancyboxController : Controller
    {
        public ActionResult Greeting()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            TriggerContext.Current.Parent(x => x.FancyClose());

            return View();
        }

        public ActionResult Restore()
        {
            TriggerContext.Current.Parent(x => x.FancyClose());

            return View("Edit");
        }

    }
}