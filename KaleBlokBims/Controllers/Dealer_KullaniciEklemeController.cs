using KaleBlokBims.Models.Classlar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Dealer_KullaniciEklemeController : Controller
    {
        // GET: Dealer_KullaniciEkleme
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string bayiKullanicilari()
        {

            var db = new Models.IZOKALEPORTALEntities();
            var bayiKodu = Session["BayiKodu"].ToString();
            return JsonConvert.SerializeObject(db.BayiKullanicilari.Where(x => x.BayiKodu == bayiKodu).OrderByDescending(x => x.LOGICALREF));
        }
        [HttpPost]
        public string kullaniciYetkileri(string kullaniciId)
        {
            var db = new Models.IZOKALEPORTALEntities();
            var yetkiler = db.BayiKullaniciYetkileri.Where(x => x.KullaniciID.ToString() == kullaniciId);
            if (yetkiler.Count() > 0)
            {
                return JsonConvert.SerializeObject(yetkiler);
            }
            else
            {
                return JsonConvert.SerializeObject(db.BayiKullaniciYetkileri.Where(x => x.KullaniciID.ToString() == "0"));
            }

        }
        [HttpPost]
        public string kullaniciKaydet(string cAktif,string cname, string cemail,string cphone,string cpassword,string yetkiler)
        {
            RestSharp.RestResponse response = new RestSharp.RestResponse();
            try
            {
                MD5 md5 = new MD5();
                var bayiKodu = Session["BayiKodu"].ToString();
                var db = new Models.IZOKALEPORTALEntities();
                var kullanici = db.BayiKullanicilari.Where(x => x.BayiKodu == bayiKodu && x.MailAdresi == cemail).FirstOrDefault();
                bool yeniMi = false;
                if (kullanici == null)
                {
                    yeniMi = true;
                    kullanici = new Models.BayiKullanicilari();
                    kullanici.MailAdresi = cemail;
                    kullanici.AdminMi = false;
                }
                kullanici.AdiSoyadi = cname;
                kullanici.BayiAdi = Session["BayiAdi"].ToString();
                kullanici.BayiKodu = Session["BayiKodu"].ToString();
                kullanici.GSM = cphone;
                if (cpassword.Length == 4)
                {
                    kullanici.Sifre = md5.MD5Sifrele(cpassword);
                    kullanici.SifreDegistirmeTarihi = DateTime.Now.AddYears(-1);
                }

                kullanici.Status = Convert.ToBoolean(cAktif);

                if (yeniMi)
                {
                    db.BayiKullanicilari.Add(kullanici);

                }
                db.SaveChanges();

                var yetki = db.BayiKullaniciYetkileri.Where(x => x.KullaniciID.ToString() == kullanici.LOGICALREF.ToString()).FirstOrDefault();
                if (yetki == null)
                {
                    yetki = new Models.BayiKullaniciYetkileri();
                    yetki.FirmaAdminiMi = false;
                    yetki.KullaniciID = Convert.ToInt32(kullanici.LOGICALREF);
                    db.BayiKullaniciYetkileri.Add(yetki);
                    db.SaveChanges();
                }

                db.Database.ExecuteSqlCommand("update BayiKullaniciYetkileri set " + yetkiler.Substring(0, yetkiler.Length - 1) + " where KullaniciID=" + kullanici.LOGICALREF);
                response.IsSuccessStatusCode = true;
                response.Content = "Kayıt Başarılı";
            }
            catch (Exception hata)
            {
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = hata.Message;
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}