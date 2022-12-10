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
        public string bayiler()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return JsonConvert.SerializeObject(servis.Bayiler());
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
            string cbayii = cbayi;
            string cemaill = cemail;
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            var tumBayiler = servis.Bayiler();
            MD5 md5 = new MD5();
            if (cbayii == null || cbayii.ToString().Trim()=="" || cbayii.ToString().Trim() == "-1")
            {
                return "Bayi Seçmelisiniz";
            }
            var db = new Models.IZOKALEPORTALEntities();
            var bayiKullanicisi = db.BayiKullanicilari.Where(x => x.MailAdresi == cemaill).FirstOrDefault();
            if (bayiKullanicisi==null)
            {
                bayiKullanicisi = new Models.BayiKullanicilari();
                bayiKullanicisi.AdiSoyadi = cname;
                bayiKullanicisi.AdminMi = true;
                bayiKullanicisi.BayiKodu = cbayii;
                bayiKullanicisi.BayiAdi = tumBayiler.Where(x=>x.BayiKodu.Equals(cbayii)).FirstOrDefault().BayiAdi;
                bayiKullanicisi.MailAdresi = cemaill;
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
                bayiKullanicisi.BayiKodu = cbayii;
                bayiKullanicisi.BayiAdi = tumBayiler.Where(x => x.BayiKodu.Equals(cbayii)).FirstOrDefault().BayiAdi;
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