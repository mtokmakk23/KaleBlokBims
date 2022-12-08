using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Dealer_SiparislerController : Controller
    {
        // GET: Dealer_Siparisler
        public ActionResult Index()
        {
          
            return View();
        }
        [HttpPost]
        public string Siparisler()
        {

            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.SiparisFisleri(Session["BayiKodu"].ToString());

        }

        [HttpPost]
        public string SiparisDetaylari(int orFicheRef)
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.SiparisFisiSatirlari(orFicheRef);
          
        }

        [HttpPost]
        public string IrsaliyeBilgisi(int orfLineRef)
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.SiparisFisiSatirIrsaliyeleri(orfLineRef);
         
        }
    }
}