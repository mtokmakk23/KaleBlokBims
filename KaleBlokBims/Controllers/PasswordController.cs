using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class PasswordController : Controller
    {
        // GET: Password
        public ActionResult Index()
        {
            try
            {
                var aaa = Session["AdminMi"].ToString();
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Dealer_Login");
            }
            return View();
        }

        [HttpPost]
        public string SifreDegistir(string mevcutSifre, string yeniSifre, string yeniSifreTekrar)
        {
            RestSharp.RestResponse response = new RestSharp.RestResponse();
            try
            {

                if (yeniSifre.Length < 6 || yeniSifre.Length > 10)
                {
                    response.IsSuccessStatusCode = false;
                    response.ErrorMessage = "Yeni Şifre 6 İle 10 Karakter Arasında Olmalıdır";
                    return JsonConvert.SerializeObject(response);
                }
                if (yeniSifre != yeniSifreTekrar)
                {
                    response.IsSuccessStatusCode = false;
                    response.ErrorMessage = "Yeni Şifre İle Tekrar Aynı Değil!";
                    return JsonConvert.SerializeObject(response);
                }
                if (mevcutSifre == yeniSifre)
                {
                    response.IsSuccessStatusCode = false;
                    response.ErrorMessage = "Mevcut Şifre İle Yeni Şifre Aynı Olamaz.";
                    return JsonConvert.SerializeObject(response);
                }
                var db = new Models.IZOKALEPORTALEntities();
                var md5 = new Models.Classlar.MD5();
                string mailAdresi = Session["MailAdresi"].ToString();
                mevcutSifre = md5.MD5Sifrele(mevcutSifre);
                if (Session["AdminMi"].ToString() == "1")
                {
                    var query = db.AdminKullanicilari.Where(x => x.MailAdresi == mailAdresi && x.Statu == true && (x.Sifre == mevcutSifre || x.GeciciSifre == mevcutSifre)).FirstOrDefault();
                    if (query != null)
                    {
                        query.Sifre = md5.MD5Sifrele(yeniSifre);
                        query.SifreDegistirmeTarihi = DateTime.Now;
                        db.SaveChanges();
                        response.IsSuccessStatusCode = true;
                        response.Content = "/Admin_Anasayfa/Index";
                        return JsonConvert.SerializeObject(response);
                    }
                    else
                    {
                        response.IsSuccessStatusCode = false;
                        response.ErrorMessage = "Mevcut Şifre Hatalı Lütfen Kontrol Ediniz.";
                        return JsonConvert.SerializeObject(response);
                    }
                }
                else
                {
                    var query = db.BayiKullanicilari.Where(x => x.MailAdresi == mailAdresi && x.Status == true && (x.Sifre == mevcutSifre || x.GeciciSifre == mevcutSifre)).FirstOrDefault();
                    if (query != null)
                    {
                        query.Sifre = md5.MD5Sifrele(yeniSifre);
                        query.SifreDegistirmeTarihi = DateTime.Now;
                        db.SaveChanges();
                        response.IsSuccessStatusCode = true;
                        response.Content = "/Dealer_Anasayfa/Index";
                        return JsonConvert.SerializeObject(response);
                    }
                    else
                    {
                        response.IsSuccessStatusCode = false;
                        response.ErrorMessage = "Mevcut Şifre Hatalı Lütfen Kontrol Ediniz.";
                        return JsonConvert.SerializeObject(response);
                    }
                }
            }
            catch (Exception hata)
            {
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = hata.Message;
                return JsonConvert.SerializeObject(response);
            }

        }
    }
}