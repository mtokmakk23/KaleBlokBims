using KaleBlokBims.Models;
using KaleBlokBims.Models.Classlar;
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
    public class Admin_SikayetController : Controller
    {
        // GET: Admin_Sikayet
        public ActionResult TumSikayetler()
        {
            return View();
        }

        [HttpPost]
        public string _TumSikayetler()
        {
            var db = new Models.IZOKALEPORTALEntities();
            return JsonConvert.SerializeObject(db.Sikayet.Where(x => x.Aktif == true).OrderByDescending(x => x.FormTarih), new IsoDateTimeConverter { DateTimeFormat = "dd.MM.yyyy" });
        }


        public ActionResult SikayetInceleme(string MASLGRF)
        {
            try
            {
                ViewBag.MASLGRF = MASLGRF;
                return View();
            }
            catch (Exception)
            {
                return TumSikayetler();
            }


        }


        [HttpPost]
        public string SikayetAyrinti(string logicalref)
        {
            var db = new Models.IZOKALEPORTALEntities();
            return JsonConvert.SerializeObject(db.Sikayet.Where(x => x.LOGICALREF.ToString().Equals(logicalref) && x.Aktif == true).OrderByDescending(x => x.LOGICALREF), new IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });
        }
        [HttpPost]
        public string ResimleriGetir(string logicalref, string bayiKodu)
        {
            try
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
            catch (Exception)
            {
                return "[]";
            }


        }

        [HttpPost]
        public string OnIncelemeKaydet(string logicalref, string onIncelemeSekli, string onIncelemeSorumlusu, string onIncelemeSonucu, string yapilanIslemSorumlusu, string yapilanIslemler, string musteriyeYapilanGeriDonus, string durum, string sikayetMaliyeti, string sorumluDepartman, string kokNedenAnalizi, string mevcutDurum)
        {
            var db = new Models.IZOKALEPORTALEntities();
            if ((yapilanIslemler.Trim() == "" || yapilanIslemSorumlusu.Trim() == "" || musteriyeYapilanGeriDonus.Trim() == "") && (durum == "KAPATILDI" || durum == "KAPATILDI (RED)"))
            {
                return "Yapılan İşlemleri,Yapılan İşlemler Sorumlusu ve Müşteriye Geri Dönüş Yapılmadan Şikayet Kapatılamaz!";
            }
            else
            {
                try
                {
                    var guncellenecekUrun = db.Sikayet.Where(x => x.LOGICALREF.ToString().Equals(logicalref)).FirstOrDefault();              
                    guncellenecekUrun.OnIncelemeSekli = onIncelemeSekli; // Seçilen ürünün ProductName özelliği değiştirildi
                    guncellenecekUrun.OnIncelemeSonucu = onIncelemeSonucu;
                    guncellenecekUrun.OnIncelemeSorumlusu = onIncelemeSorumlusu;
                    guncellenecekUrun.OnIncelemeYapilanIslemSorumlusu = yapilanIslemSorumlusu;
                    guncellenecekUrun.OnIncelemeYapilanIsler = yapilanIslemler;
                    guncellenecekUrun.OnIncelemeIslemTarihi = DateTime.Now;
                    guncellenecekUrun.OnIncelemeIslemYapan = Session["MailAdresi"].ToString();
                    guncellenecekUrun.FirmaCevabi = musteriyeYapilanGeriDonus; // Seçilen ürünün ProductName özelliği değiştirildi
                    guncellenecekUrun.Durum = durum;
                    guncellenecekUrun.SikayetMaliyeti = sikayetMaliyeti;
                    guncellenecekUrun.SorumluDepartman = sorumluDepartman;
                    guncellenecekUrun.KokNedenAnalizi = kokNedenAnalizi;
                    db.SaveChanges(); // Yukarıda yapılan güncellemeler veritabanına gönderildi



                    if (mevcutDurum != durum && (durum == "KAPATILDI" || durum == "KAPATILDI (RED)"))
                    {
                        
                            var alici = "";
                            if (guncellenecekUrun.LoginOlunanMail.Trim() != "")
                                alici = (guncellenecekUrun.LoginOlunanMail);
                            var icerik = "" +
                            "<table style='border:1px solid #eee;border-radius: 5px;padding: 10px;font-size: 13px;width:100%'>" +
                               "<tr><td><img src='http://88.250.15.240:8090/assets/images/logom.png' style='height: 55px' alt='KALEBLOK'></td><td style='text-align:center;font-weight:bold;'>ŞİKAYET CEVAP FORMU</td></tr>" +
                               "<tr><td style='font-weight:bold'>Form No:</td><td>" + guncellenecekUrun.FormNo + "</td></tr>" +
                               "<tr><td style='font-weight:bold'>Tarih:</td><td>" + Convert.ToDateTime(guncellenecekUrun.FormTarih).ToString("dd.MM.yyyy") + "</td></tr>" +
                               "<tr><td style='font-weight:bold'>Bayi:</td><td>" + guncellenecekUrun.BayiAdi + "</td></tr>" +
                               "<tr><td style='font-weight:bold'>Şantiye:</td><td>" + guncellenecekUrun.SantiyeAdi + "</td></tr>" +
                               "<tr><td style='font-weight:bold'>Ürün Grubu:</td><td>" + guncellenecekUrun.UrunGrubu.Replace("#", ",") + "</td></tr>" +
                               "<tr><td style='font-weight:bold'>Şikayet Başlığı:</td><td>" + guncellenecekUrun.BildirimSebebi.Replace("#", ",") + "</td></tr>" +
                               "<tr><td style='font-weight:bold'>Ürün Açıklaması:</td><td>" + guncellenecekUrun.UrunAciklama.Replace("#", ",") + "</td></tr>" +
                               "<tr><td style='font-weight:bold'>Detaylı Açıklama:</td><td>" + guncellenecekUrun.DetayliAciklama + "</td></tr>" +
                               "<tr><td style='font-weight:bold'>Şikayet Cevabı:</td><td>" + guncellenecekUrun.FirmaCevabi + "</td></tr>" +
                               "</table>";
                            string mailGonderilecekler = SabitTanimlar.SikayetBildirimiYapilacakEmailler();
                            MailGonderme mailGonder = new MailGonderme();
                            mailGonder.EksizMailGonderme("", mailGonderilecekler, alici, guncellenecekUrun.FormNo + " FORM NUMARALI ŞİKAYET CEVABI", icerik);

                        
                    }
                    return "true";
                }
                catch (Exception hata)
                {

                    return hata.Message;
                }
            }



        }


    }
}