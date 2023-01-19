using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class LoggerController : Controller
    {
        // GET: Logger
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public string LogKayitlari(string ip, string sayfaLinki)
        {
            var db = new Models.IZOKALEPORTALEntities();
            var query = from a in db.Log
                        orderby a.Tarih descending
                        select new
                        {
                            AdminMi=(a.AdminMi==true)?"Admin":"Bayi",
                            a.BayiAdi,
                            a.GezindigiSayfa,
                            a.IpAdresi,
                            a.MailAdresi,
                            a.Tarih
                        };
            return JsonConvert.SerializeObject(query, new IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy HH:mm" });
        }
        [HttpPost]
        public void LogKayit(string ip, string sayfaLinki)
        {
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                var log = new Models.Log();
                log.AdminMi = Convert.ToBoolean((Session["AdminMi"].ToString() == "1" ? "true" : "false"));
                log.GezindigiSayfa = sayfaLinki;
                log.IpAdresi = ip;
                log.MailAdresi = Session["MailAdresi"].ToString();
                log.Tarih = DateTime.Now;
                log.BayiAdi = Session["BayiAdi"].ToString();
                db.Log.Add(log);
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}