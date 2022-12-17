using KaleBlokBims.Models.Classlar;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace KaleBlokBims.Controllers
{
    [AllowAnonymous]
    public class Dealer_LoginController : Controller
    {
        
        // GET: Dealer_Login
        public ActionResult Login()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            HttpContext.Request.Cookies.Clear();
            ViewBag.Sonuc = "";
            
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email,string password)
        {
            var md5 = new Models.Classlar.MD5();
            password = md5.MD5Sifrele(password);
            var db = new Models.IZOKALEPORTALEntities();
            var kullanici = db.BayiKullanicilari.Where(x=>x.MailAdresi==(email) && x.Sifre==(password) && x.Status==(true)).FirstOrDefault();
            if (kullanici!=null)
            {
                Session["BayiKodu"] = kullanici.BayiKodu;
                Session["BayiAdi"] = kullanici.BayiAdi;
                Session["AdiSoyadi"] = kullanici.AdiSoyadi;
                Session["KullaniciId"] = kullanici.LOGICALREF;
                Session["MailAdresi"] = kullanici.MailAdresi;
                Session["AdminMi"] = "0";
                FormsAuthentication.SetAuthCookie(kullanici.LOGICALREF.ToString(), false);
                return RedirectToAction("Index", "Dealer_Anasayfa");
            }
            else
            {
                ViewBag.Sonuc = "Kullanıcı Bilgileri Bulunamadı";
                return View();
            }
           
        }
    }
}