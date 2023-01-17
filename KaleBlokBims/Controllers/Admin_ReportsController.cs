using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Admin_ReportsController : Controller
    {
        // GET: Admin_Reports
        public ActionResult BayiBaglantiBakiyeRaporu()
        {
            return View();
        }
        [HttpPost]
        public string _BaglantiBakiyeRaporu()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.TumBayilerinBaglantiBakiyeOzeti();
        }
    }
}