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

            return View();
        }
        [HttpPost]
        public string cariyiBayidenAyir(string LOGICALREF)
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.CariyiBayidenAyir(LOGICALREF);
        }

        public string tumCariler(string term, string _type, string q)
        {
            term = Encoding.UTF8.GetString(Encoding.GetEncoding("iso-8859-1").GetBytes(term));
            if (term == null || term.Trim() == "")
            {
                return "";
            }
            else
            {
                var servis = new M2BWebService.ZOKALEAPISoapClient();
                return servis.CariHesaplar(term);
            }


        }
        [HttpPost]
        public string bayiyeCariBagla(string MusteriLREF, string CariLREF)
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.BayiyeCariBagla(MusteriLREF, CariLREF);
        }

        [HttpPost]
        public string bayiler()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return JsonConvert.SerializeObject(servis.BayiVeBagliCariKodlari());
        }
        [HttpPost]
        public string yeniBayi(string bayiAdi)
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.YeniBayi(bayiAdi);
        }

        [HttpPost]
        public string bayiKullanicilari()
        {
            var db = new Models.IZOKALEPORTALEntities();
            return JsonConvert.SerializeObject(db.BayiKullanicilari.OrderByDescending(x=>x.LOGICALREF));
        }
        [HttpPost]
        public void kullaniciSil(string LREF)
        {
            var db = new Models.IZOKALEPORTALEntities();
            db.Database.ExecuteSqlCommand("delete from BayiKullanicilari where LOGICALREF="+LREF);
        }
        [HttpPost]
        public string kullaniciDuzenle(string cbayi,string cname,string cemail,string cphone,string cpassword,bool cAktif = false)
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            var tumBayiler = servis.BayiVeBagliCariKodlari();
            MD5 md5 = new MD5();
            if (cbayi==null || cbayi.ToString().Trim()=="" || cbayi.ToString().Trim() == "-1")
            {
                return "Bayi Seçmelisiniz";
            }
            var db = new Models.IZOKALEPORTALEntities();
            var bayiKullanicisi = db.BayiKullanicilari.Where(x => x.MailAdresi == cemail).FirstOrDefault();
            if (bayiKullanicisi==null)
            {
                bayiKullanicisi = new Models.BayiKullanicilari();
                bayiKullanicisi.AdiSoyadi = cname;
                bayiKullanicisi.AdminMi = true;
                bayiKullanicisi.BayiKodu = cbayi;
                bayiKullanicisi.BayiAdi = tumBayiler.Where(x=>x.BayiKodu.Equals(cbayi)).FirstOrDefault().MusteriAdi;
                bayiKullanicisi.MailAdresi = cemail;
                bayiKullanicisi.Sifre = md5.MD5Sifrele(cpassword);
                bayiKullanicisi.Status = cAktif;
                bayiKullanicisi.GSM = cphone;
                db.BayiKullanicilari.Add(bayiKullanicisi);
                db.SaveChanges();
            }
            else
            {
                bayiKullanicisi.AdiSoyadi = cname;
                bayiKullanicisi.AdminMi = true;
                bayiKullanicisi.BayiKodu = cbayi;
                bayiKullanicisi.BayiAdi = tumBayiler.Where(x => x.BayiKodu.Equals(cbayi)).FirstOrDefault().MusteriAdi;
                bayiKullanicisi.MailAdresi = cemail;
                if (cpassword.Length==4)
                {
                    bayiKullanicisi.Sifre = md5.MD5Sifrele(cpassword);
                }
                bayiKullanicisi.Status = cAktif;
                bayiKullanicisi.GSM = cphone;
                db.SaveChanges();
            }
            return "ok";
           
        }
    }
}