using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Admin_AnasayfaController : Controller
    {
        // GET: Admin_Anasayfa
        public ActionResult Index()
        {
            Session["BayiKodu"] = "";
            Session["BayiAdi"] = "";
            var db = new Models.IZOKALEPORTALEntities();
            var AdminBilgisi = db.AdminKullanicilari.ToList().Where(x=>x.MailAdresi== Session["MailAdresi"].ToString());
            ViewBag.AdminBilgileri = JsonConvert.SerializeObject(AdminBilgisi);
           
            return View();
        }

        [HttpPost]
        public string bayiAdinaSistemeGir(string LOGICALREF)
        {
            RestSharp.RestResponse response = new RestSharp.RestResponse();
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            var Bayi = (servis.Bayiler()).Where(x=>x.LOGICALREF.ToString().Equals(LOGICALREF)).FirstOrDefault();
            if (Bayi!=null)
            {
                Session["BayiKodu"] = Bayi.BayiKodu;
                Session["BayiAdi"] = Bayi.BayiAdi;
                response.IsSuccessStatusCode = true;
            }
            else
            {
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = "Bayi Bilgileri Bulunamadı.";
            }
            return JsonConvert.SerializeObject(response);
        }
    }
}