using KaleBlokBims.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Dealer_EvraklarController : Controller
    {
        // GET: Dealer_Evraklar
        public ActionResult Evraklar()
        {
            return View();
        }

        [HttpPost]
        public string EvrakKaydet(string docType, HttpPostedFileBase file)
        {
            try
            {
                string bayiKodu = Session["BayiKodu"].ToString();
                string bayiAdi = Session["BayiAdi"].ToString();
                var db = new Models.IZOKALEPORTALEntities();
                string LOGICALREF = "";
                try
                {
                    string sqlString = "SELECT LOGICALREF from BayiEvraklari where BayiKodu='" + bayiKodu + "' and " + docType + " is null";
                    var id = db.Database.SqlQuery<int>(sqlString).Single();
                    LOGICALREF = id.ToString();
                }
                catch (Exception)
                {
                }

                BayiEvraklari query;
                if (LOGICALREF == "")
                {
                    query = new BayiEvraklari();
                    query.BayiKodu = bayiKodu;
                    query.BayiAdi = bayiAdi;
                    db.BayiEvraklari.Add(query);
                    db.SaveChanges();
                    LOGICALREF = query.LOGICALREF.ToString();
                }



                string folderPath = "/Files/BayiEvraklari/" + bayiKodu + "/";
                if (!Directory.Exists(Server.MapPath("~") + folderPath))
                {
                    Directory.CreateDirectory(Server.MapPath("~") + folderPath);
                }
                var fileNameArray = file.FileName.Split('.');
                string nameAndLocation = folderPath + docType + "_" + LOGICALREF + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")+"."+ fileNameArray[fileNameArray.Length-1];
                file.SaveAs(Server.MapPath(nameAndLocation));

                db.Database.ExecuteSqlCommand("update BayiEvraklari set " + docType + "='" + (nameAndLocation) + "' where LOGICALREF=" + LOGICALREF);
                return "ok";
            }
            catch (Exception hata)
            {
                return hata.Message;
            }



        }

        [HttpPost]
        public string TumEvraklar()
        {
            string bayiKodu = Session["BayiKodu"].ToString();
            var db = new Models.IZOKALEPORTALEntities();
            var query = db.BayiEvraklari.Where(x => x.BayiKodu == bayiKodu).ToList();
            return JsonConvert.SerializeObject(query);
        }

        [HttpPost]
        public string EvrakSil(string LOGICALREF, string docType)
        {
            try
            {
                string bayiKodu = Session["BayiKodu"].ToString();

                var db = new Models.IZOKALEPORTALEntities();
                db.Database.ExecuteSqlCommand("update BayiEvraklari set " + docType + "=NULL where BayiKodu='"+ bayiKodu + "' and LOGICALREF=" + LOGICALREF);
                db.Database.ExecuteSqlCommand("delete FROM [IZOKALEPORTAL].[dbo].[BayiEvraklari] where [BayilikSozlesmesi] is null and [BayiPortalSozlesmesi] is null and [CariHesapSozlesmesi] is null and [VergiLevhasi] is null and [ImzaSirkusu] is null and [TicaretSicilGazetesi] is null and [NufusCuzdaniFotokopisi] is null and [Diger] is null ");
                return "ok";
            }
            catch (Exception hata)
            {
                return hata.Message;
            }
        }
    }
}