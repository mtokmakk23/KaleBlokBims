using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace KaleBlokBims.Controllers
{
    [AllowAnonymous]
    public class Admin_LoginController : Controller
    {
        // GET: Admin_Login
        public ActionResult Login()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            HttpContext.Request.Cookies.Clear();
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var md5 = new Models.Classlar.MD5();
            password = md5.MD5Sifrele(password);
            var db = new Models.IZOKALEPORTALEntities();
            var kullanici = db.AdminKullanicilari.Where(x => x.MailAdresi == (email) && x.Sifre == (password) && x.Statu == (true)).FirstOrDefault();
            if (kullanici != null)
            {
                Session["BayiKodu"] = "";
                Session["AdiSoyadi"] = kullanici.AdiSoyadi;
                Session["KullaniciId"] = kullanici.LOGICALREF;
                Session["MailAdresi"] = kullanici.MailAdresi;
                Session["AdminMi"] = "1";
                FormsAuthentication.SetAuthCookie(kullanici.LOGICALREF.ToString(), false);
                return RedirectToAction("Index", "Admin_Anasayfa");
            }
            else
            {
                ViewBag.Sonuc = "Kullanıcı Bilgileri Bulunamadı";
                return View();
            }

        }
    }
}