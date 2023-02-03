using KaleBlokBims.Models.Classlar;
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
            var yetkiler = Yetkiler.AdminKullaniciYetkisi();
            if (!Convert.ToBoolean(yetkiler.RaporlariGorme))
            {
                return RedirectToAction("Index", "Admin_Anasayfa");
            }
            else
            {
                return View();
            }

        }
        [HttpPost]
        public string _BaglantiBakiyeRaporu()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.TumBayilerinBaglantiBakiyeOzeti();
        }

        public ActionResult YoneticiRaporlari()
        {
            var yetkiler = Yetkiler.AdminKullaniciYetkisi();
            if (!Convert.ToBoolean(yetkiler.YoneticiRaporlariniGorme))
            {
                return RedirectToAction("Index", "Admin_Anasayfa");
            }
            else
            {
                return View();
            }
           

           
        }
        [HttpPost]
        public string YoneticiRaporlariServis(string raporAdi)
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.YonetimRaporlari(raporAdi);
        }
    }
}