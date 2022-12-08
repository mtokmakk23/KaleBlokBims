using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Dealer_IrsaliyelerController : Controller
    {
        // GET: Dealer_Irsaliyeler
        public ActionResult Index()
        {
          
            return View();
        }

        [HttpPost]
        public string Irsaliyeler()
        {

            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.IrsaliyeFisleri(Session["BayiKodu"].ToString());

        }
        [HttpPost]
        public string IrsaliyeDetaylari(int stFicheRef)
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.IrsaliyeFisiSatirlari(stFicheRef);
            
        }
    }
}