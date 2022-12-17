using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Dealer_FiyatListesiController : Controller
    {
        // GET: Dealer_FiyatListesi
        public ActionResult Index()
        {
          
            return View();
        }

        [HttpPost]
        public string fiyatListeleri()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return JsonConvert.SerializeObject(servis.SecilebilirFiyatListeleri(Session["BayiKodu"].ToString()));
        }
         [HttpPost]
        public string Gruplar()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.AnaGruplar("","","");
        }
         [HttpPost]
        public string Iller()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.IlveIlceleriGetir();
        }


        [HttpPost]
        public string MalzemeFiyatlariniGetir(string FiyatListesiKodu,string baglantiLREF, string SPECODE1, string SPECODE2, string Il, string Ilce, string fabrikaTeslimMi, string GuncelUSD, string GuncelEUR)
        {
            if (SPECODE1=="-1") SPECODE1 = "";
            if (SPECODE2=="-1") SPECODE2 = "";
           
            
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            var list = servis.MalzemeListesi(Session["BayiKodu"].ToString(), FiyatListesiKodu, baglantiLREF, SPECODE1, SPECODE2, Il, Ilce, Convert.ToBoolean(fabrikaTeslimMi), Convert.ToDouble(GuncelUSD.ToString().Replace(".",",")), Convert.ToDouble(GuncelEUR.ToString().Replace(".", ",")));
            return list;

        }
    }
}