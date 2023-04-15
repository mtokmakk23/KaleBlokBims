using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    [AllowAnonymous]
    public class PublicDataController : Controller
    {
        // GET: PublicData
        public ActionResult SikayetDetaylari(string KEYLINK)
        {
            string OnlukKey = "";
            try
            {
                KEYLINK = Request.QueryString["KEYLINK"];
                var a = KEYLINK.Length;
                if (KEYLINK == null || (KEYLINK.Length < 27 && KEYLINK.Length > 33))
                {
                    ViewBag.LOGICALREF = "";
                  
                }
                else
                {
                    for (int i = 0; i < KEYLINK.Length; i++)
                    {
                        OnlukKey += to10(KEYLINK.Substring(i, 1));
                    }
                    DateTime tarih = new DateTime();
                    string log = "";
                    var mod = Convert.ToInt32(OnlukKey.Substring(1, 1));
                    if (mod == 1)
                    {
                        var gun = Convert.ToInt32(OnlukKey.Substring(19, 1) + OnlukKey.Substring(9, 1));
                        var ay = Convert.ToInt32(OnlukKey.Substring(21, 1) + OnlukKey.Substring(3, 1));
                        var yil = Convert.ToInt32(OnlukKey.Substring(12, 1) + OnlukKey.Substring(24, 1) + OnlukKey.Substring(7, 1) + OnlukKey.Substring(16, 1));
                        tarih = new DateTime(yil, ay, gun);
                        log = OnlukKey.Substring(OnlukKey.Length - Convert.ToInt32(OnlukKey.Substring(OnlukKey.Length - 1, 1)) - 1, Convert.ToInt32(OnlukKey.Substring(OnlukKey.Length - 1, 1)));
                    }
                    else
                    if (mod == 2)
                    {
                        var gun = Convert.ToInt32(OnlukKey.Substring(16, 1) + OnlukKey.Substring(7, 1));
                        var ay = Convert.ToInt32(OnlukKey.Substring(21, 1) + OnlukKey.Substring(9, 1));
                        var yil = Convert.ToInt32(OnlukKey.Substring(24, 1) + OnlukKey.Substring(19, 1) + OnlukKey.Substring(12, 1) + OnlukKey.Substring(3, 1));
                        tarih = new DateTime(yil, ay, gun);
                        log = OnlukKey.Substring(OnlukKey.Length - Convert.ToInt32(OnlukKey.Substring(OnlukKey.Length - 1, 1)) - 1, Convert.ToInt32(OnlukKey.Substring(OnlukKey.Length - 1, 1)));

                    }
                    else
                    {
                        ViewBag.LOGICALREF = "";
                    }
                    ViewBag.LOGICALREF = log;
                    TimeSpan Sonuc = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) - tarih;
                    if (Convert.ToDouble(Sonuc.TotalDays.ToString()) >= 0 && Convert.ToDouble(Sonuc.TotalDays.ToString()) < 2)
                    {

                    }
                    else
                    {
                        ViewBag.LOGICALREF = "";
                    }
                }
            }
            catch (Exception hata)
            {
                ViewBag.LOGICALREF = "";
            }
            return View();
        }

        public string to10(string x)
        {
            return (Convert.ToInt32(x, 16) - 6).ToString();
        }
        [HttpPost]
        public string SikayetiGetir(string LOGICALREF)
        {
            var db = new Models.IZOKALEPORTALEntities();
            return JsonConvert.SerializeObject(db.Sikayet.Where(x =>x.Aktif == true && x.LOGICALREF.ToString()==LOGICALREF).OrderByDescending(x => x.LOGICALREF), new IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });
        }
        [HttpPost]
        public string ResimleriGetir(string logicalref, string bayiKodu)
        {

            var resimUzantilariListesi = new List<string>();
            //var folderPath = "~/images/sikayet/" + logicalref + "/Gelen";
            string uzanti = "/Files/BayiSikayetleri/" + bayiKodu + "/" + logicalref;
            string dizin = Server.MapPath("~") + uzanti;

            string[] dizindekiDosyalar = Directory.GetFiles(dizin);
            foreach (string dosya in dizindekiDosyalar)
            {
                FileInfo fileInfo = new FileInfo(dosya);
                string dosyaAdi = fileInfo.Name;
                resimUzantilariListesi.Add(uzanti + "/" + dosyaAdi);
            }

            return JsonConvert.SerializeObject(resimUzantilariListesi);
        }
    }
}