using System.Web.Mvc;
using Hexon.MvcTrig.Bootstrap;

namespace Hexon.MvcTrig.Sample.Controllers
{
    public class BootstrapController : Controller
    {
        public ActionResult Modal()
        {
            this.Trig(x => x.ModalOpen());

            return PartialView();
        }
    }
}