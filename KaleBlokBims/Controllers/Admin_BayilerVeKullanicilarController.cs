using KaleBlokBims.Models.Classlar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Admin_BayilerVeKullanicilarController : Controller
    {


        // GET: Admin_BayilerVeKullanicilar
        public ActionResult Index()
        {

            var yetkiler = Yetkiler.AdminKullaniciYetkisi();
            if (!Convert.ToBoolean(yetkiler.KullaniciTanimlama))
            {
                return RedirectToAction("Index", "Admin_Anasayfa");
            }
            else
            {
                return View();
            }

        }


        [HttpPost]
        public string bayiler()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return JsonConvert.SerializeObject(servis.Bayiler());
        }

        [HttpPost]
        public string adminKullaniciYetkileri(string kullaniciId)
        {
            var db = new Models.IZOKALEPORTALEntities();
            var yetkiler = db.AdminKullaniciYetkisi.Where(x => x.KullaniciID.ToString() == kullaniciId);
            if (yetkiler.Count() > 0)
            {
                return JsonConvert.SerializeObject(yetkiler);
            }
            else
            {
                return JsonConvert.SerializeObject(db.AdminKullaniciYetkisi.Where(x => x.KullaniciID.ToString() == "0"));
            }

        }
        [HttpPost]
        public string bayiKullanicilari()
        {

            var db = new Models.IZOKALEPORTALEntities();

            return JsonConvert.SerializeObject(db.BayiKullanicilari.OrderByDescending(x => x.LOGICALREF));
        }
        [HttpPost]
        public string adminKullanicilari()
        {

            var db = new Models.IZOKALEPORTALEntities();

            return JsonConvert.SerializeObject(db.AdminKullanicilari.OrderByDescending(x => x.LOGICALREF));
        }
        [HttpPost]
        public void kullaniciSil(string LREF, string adminMi)
        {
            var db = new Models.IZOKALEPORTALEntities();

            if (adminMi == "0")
            {
                db.Database.ExecuteSqlCommand("delete from BayiKullanicilari where LOGICALREF=" + LREF);
            }
            if (adminMi == "1")
            {
                db.Database.ExecuteSqlCommand("delete from AdminKullanicilari where LOGICALREF=" + LREF);
            }

        }
        [HttpPost]
        public string bayikullaniciDuzenle(string cbayi, string cname, string cemail, string cphone, string cpassword, bool cAktif = false)
        {
            string cbayii = cbayi;
            string cemaill = cemail;
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            var tumBayiler = servis.Bayiler();
            MD5 md5 = new MD5();
            if (cbayii == null || cbayii.ToString().Trim() == "" || cbayii.ToString().Trim() == "-1")
            {
                return "Bayi Seçmelisiniz";
            }
            var db = new Models.IZOKALEPORTALEntities();
            var bayiKullanicisi = db.BayiKullanicilari.Where(x => x.MailAdresi == cemaill).FirstOrDefault();
            if (bayiKullanicisi == null)
            {
                bayiKullanicisi = new Models.BayiKullanicilari();
                bayiKullanicisi.AdiSoyadi = cname;
                bayiKullanicisi.AdminMi = true;
                bayiKullanicisi.BayiKodu = cbayii;
                bayiKullanicisi.BayiAdi = tumBayiler.Where(x => x.BayiKodu.Equals(cbayii)).FirstOrDefault().BayiAdi;
                bayiKullanicisi.MailAdresi = cemaill;
                bayiKullanicisi.Sifre = md5.MD5Sifrele(cpassword);
                bayiKullanicisi.Status = cAktif;
                bayiKullanicisi.GSM = cphone;
                bayiKullanicisi.SifreDegistirmeTarihi = DateTime.Now.AddDays(-360);
                db.BayiKullanicilari.Add(bayiKullanicisi);
                db.SaveChanges();

                var yetki = db.BayiKullaniciYetkileri.Where(x => x.KullaniciID.ToString() == bayiKullanicisi.LOGICALREF.ToString()).FirstOrDefault();
                if (yetki == null)
                {
                    yetki = new Models.BayiKullaniciYetkileri();
                    yetki.FirmaAdminiMi = true;
                    yetki.KullaniciID = Convert.ToInt32(bayiKullanicisi.LOGICALREF);
                    db.BayiKullaniciYetkileri.Add(yetki);
                    db.SaveChanges();
                }

            }
            else
            {
                bayiKullanicisi.AdiSoyadi = cname;
                bayiKullanicisi.AdminMi = true;
                bayiKullanicisi.BayiKodu = cbayii;
                bayiKullanicisi.BayiAdi = tumBayiler.Where(x => x.BayiKodu.Equals(cbayii)).FirstOrDefault().BayiAdi;
                if (cpassword.Length == 4)
                {
                    bayiKullanicisi.Sifre = md5.MD5Sifrele(cpassword);
                    bayiKullanicisi.SifreDegistirmeTarihi = DateTime.Now.AddDays(-360);

                }
                bayiKullanicisi.Status = cAktif;
                bayiKullanicisi.GSM = cphone;
                db.SaveChanges();
            }
            return "#ok";

        }
        [HttpPost]
        public void adminYetkileriKaydet(string LREF, string yetkiler)
        {
            var db = new Models.IZOKALEPORTALEntities();
            db.Database.ExecuteSqlCommand("update AdminKullaniciYetkisi set " + yetkiler.Substring(0, yetkiler.Length - 1) + " where KullaniciID=" + LREF);

        }

        [HttpPost]
        public string adminkullaniciDuzenle(string ccname, string ccemail, string ccphone, string ccpassword, bool ccAktif = false)
        {
            string cemaill = ccemail;
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            var tumBayiler = servis.Bayiler();
            MD5 md5 = new MD5();
            var db = new Models.IZOKALEPORTALEntities();
            var adminKullanicisi = db.AdminKullanicilari.Where(x => x.MailAdresi == cemaill).FirstOrDefault();
            if (adminKullanicisi == null)
            {
                adminKullanicisi = new Models.AdminKullanicilari();
                adminKullanicisi.AdiSoyadi = ccname;
                adminKullanicisi.MailAdresi = cemaill;
                adminKullanicisi.Sifre = md5.MD5Sifrele(ccpassword);
                adminKullanicisi.Statu = ccAktif;
                adminKullanicisi.GSM = ccphone;
                adminKullanicisi.SifreDegistirmeTarihi = DateTime.Now.AddDays(-360);
                db.AdminKullanicilari.Add(adminKullanicisi);
                db.SaveChanges();

                var yetki = new Models.AdminKullaniciYetkisi();
                yetki.KullaniciID =Convert.ToInt32(adminKullanicisi.LOGICALREF);
                db.AdminKullaniciYetkisi.Add(yetki);
                db.SaveChanges();
            }
            else
            {
                adminKullanicisi.AdiSoyadi = ccname;
                if (ccpassword.Length == 4)
                {
                    adminKullanicisi.SifreDegistirmeTarihi = DateTime.Now.AddDays(-360);
                    adminKullanicisi.Sifre = md5.MD5Sifrele(ccpassword);
                }
                adminKullanicisi.Statu = ccAktif;
                adminKullanicisi.GSM = ccphone;
                db.SaveChanges();
            }
            return "#ok " + adminKullanicisi.LOGICALREF;

        }
    }
}