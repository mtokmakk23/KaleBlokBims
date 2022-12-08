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
            var db = new Models.IZOKALEPORTALEntities();
            var AdminBilgisi = db.AdminKullanicilari.ToList().Where(x=>x.MailAdresi== Session["MailAdresi"].ToString());
            ViewBag.AdminBilgileri = JsonConvert.SerializeObject(AdminBilgisi);

            return View();
        }
    }
}