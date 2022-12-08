using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Dealer_HesapEkstresiController : Controller
    {
        // GET: Dealer_HesapEkstresi
        public ActionResult Index()
        {
           
            return View();
        }

        [HttpPost]
        public string HesapEkstresi()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.HesapEkstresi(Session["BayiKodu"].ToString());

        }
    }
}