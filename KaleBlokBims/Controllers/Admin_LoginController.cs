using KaleBlokBims.Models.Classlar;
using Newtonsoft.Json;
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
        public string geciciSifreOlustur(string mailAdresi)
        {
            RestSharp.RestResponse response = new RestSharp.RestResponse();
            var db = new Models.IZOKALEPORTALEntities();
            var query = db.AdminKullanicilari.Where(x => x.MailAdresi == mailAdresi && x.Statu == true).FirstOrDefault();
            if (query != null)
            {
                Random rnd = new Random();
                MD5 md5 = new MD5();
                var geciciSifre = rnd.Next(1000, 9999);
                query.GeciciSifre = md5.MD5Sifrele(geciciSifre.ToString());
                SmsGonderme sms = new SmsGonderme();
                sms.smsGonder(geciciSifre + " Geçici Şifreniz İle Portala Giriş Yapabilirsiniz", query.GSM);
                db.SaveChanges();
                response.IsSuccessStatusCode = true;
                response.Content = "Geçici Şifreniz 05........" + query.GSM.Substring(query.GSM.Length - 4, 4) + " Telefon Numarasına Sms Olarak Gönderilmiştir.";
            }
            else
            {
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = "Kullanıcı Bilgisi Bulunamadı";
            }
            return JsonConvert.SerializeObject(response);
        }
        [HttpPost]
        public ActionResult Login(string email, string password,string smsKodu)
        {
            var md5 = new Models.Classlar.MD5();
            password = md5.MD5Sifrele(password);
            var db = new Models.IZOKALEPORTALEntities();
            var kullanici = db.AdminKullanicilari.Where(x => x.MailAdresi == (email) && x.Sifre == (password) && x.Statu == (true)).FirstOrDefault();
            if (kullanici != null)
            {
                Session["BayiKodu"] = "";
                Session["BayiAdi"] = "";
                Session["AdiSoyadi"] = kullanici.AdiSoyadi;
                Session["KullaniciId"] = kullanici.LOGICALREF;
                Session["MailAdresi"] = kullanici.MailAdresi;
                Session["AdminMi"] = "1";
                FormsAuthentication.SetAuthCookie(kullanici.LOGICALREF.ToString(), false);
                if (Convert.ToDateTime(kullanici.SifreDegistirmeTarihi).AddDays(60) < DateTime.Now)
                {
                    return RedirectToAction("Index", "Password");

                }
                else
                {
                    return RedirectToAction("Index", "Admin_Anasayfa");
                }
            }
            else
            {
                ViewBag.Sonuc = "Kullanıcı Bilgileri Bulunamadı";
                return View();
            }

        }
    }
}