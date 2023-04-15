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
    public class Dealer_SikayetController : Controller
    {
        // GET: Dealer_Sikayet
        public ActionResult SikayetOlustur()
        {
            return View();
        }
        [HttpPost]
        public string SikayetOlustur(string santiyeAdi, string irtibatKurulacakKisi, string irtibatNo, string urunGrubu, string sikayetBasligi, string urunAciklamasi, string detayliAciklama, string partiNo, string uretimTarihi, string urunMiktari, string teslimTarihi)
        {
            var response = new RestSharp.RestResponse();
            try
            {
                string bayiKodu = Session["BayiKodu"].ToString();
                string bayiAdi = Session["BayiAdi"].ToString();
                var db = new Models.IZOKALEPORTALEntities();
                string LOGICALREF = "";


                Sikayet query;

                query = new Sikayet();
                query.BayiKodu = bayiKodu;
                query.BayiAdi = bayiAdi;
                query.BildirimSebebi = sikayetBasligi;
                query.DetayliAciklama = detayliAciklama;
                query.FormTarih = Convert.ToDateTime(DateTime.Now);
                query.IrtibatKurulacakKisi = irtibatKurulacakKisi;
                query.IrtibatNo = irtibatNo;
                query.PartiNo = partiNo;
                query.SantiyeAdi = santiyeAdi;
                query.SikayetOlusturan = Session["MailAdresi"].ToString();
                query.UrunAciklama = urunAciklamasi;
                query.UrunGrubu = urunGrubu;
                query.UrunMiktari = urunMiktari;
                if (uretimTarihi != "")
                {
                    query.UretimTarihi = Convert.ToDateTime(uretimTarihi);
                }
                if (teslimTarihi != "")
                {
                    query.TeslimTarihi = Convert.ToDateTime(teslimTarihi);
                }
                query.Durum = "AÇIK (YENİ KAYIT)";
                query.DegistirmeTarihi = DateTime.Now.ToString();
                query.LoginOlunanMail = Session["MailAdresi"].ToString();
                query.Aktif = true;

                db.Sikayet.Add(query);
                db.SaveChanges();
                LOGICALREF = query.LOGICALREF.ToString();
                db.Database.ExecuteSqlCommand("update Sikayet set FormNo='" + bayiKodu + "-(" + LOGICALREF + ")" + "' where LOGICALREF=" + LOGICALREF);

                response.IsSuccessStatusCode = true;
                response.Content = LOGICALREF;

            }
            catch (Exception hata)
            {
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = hata.Message;
            }
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public string BelgeleriKaydet()
        {
            var response = new RestSharp.RestResponse();
            var db = new Models.IZOKALEPORTALEntities();
            string LOGICALREF = Request["LOGICALREF"].ToString();
            try
            {


                var sikayet = db.Sikayet.Where(x => x.LOGICALREF.ToString() == LOGICALREF).FirstOrDefault();

                string folderPath = "/Files/BayiSikayetleri/" + sikayet.BayiKodu + "/";
                if (!Directory.Exists(Server.MapPath("~") + folderPath))
                {
                    Directory.CreateDirectory(Server.MapPath("~") + folderPath);
                }
                folderPath += LOGICALREF + "/";
                if (!Directory.Exists(Server.MapPath("~") + folderPath))
                {
                    Directory.CreateDirectory(Server.MapPath("~") + folderPath);
                }
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  

                    HttpPostedFileBase file = files[i];
                    string fname;

                    // Checking for Internet Explorer  
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = file.FileName;
                    }

                    //// Get the complete folder path and store the file inside it.  
                    //fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                    //file.SaveAs(fname);

                    string nameAndLocation = folderPath + file.FileName;
                    file.SaveAs(Server.MapPath(nameAndLocation));
                }

                response.IsSuccessStatusCode = true;
                response.ErrorMessage = "Şikayet Oluşturuldu.";
                Models.Classlar.SmsGonderme sms = new Models.Classlar.SmsGonderme();
                int len = 0;
                if (sikayet.BayiAdi.ToString().Length >= 10) len = 10;
                else len = sikayet.BayiAdi.ToString().Length;
                sms.smsGonder("Bayi Şikayeti " + sikayet.BayiAdi.ToString().Substring(0, len) + "..\n " + OnConfirm(LOGICALREF)+"\n",Models.Classlar.SabitTanimlar.SikayetBildirimiYapilacakNumaralar());

                MailGonderme mail = new MailGonderme();
                mail.EksizMailGonderme("","",SabitTanimlar.SikayetBildirimiYapilacakEmailler(), sikayet.BayiKodu + "-" + LOGICALREF+" - Bayi Şikayeti", sikayet.BayiAdi.ToString() + "..\n<br/> " + OnConfirm(LOGICALREF) + "\n");

            }
            catch (Exception hata)
            {
                db.Database.ExecuteSqlCommand("delete from Sikayet where LOGICALREF=" + LOGICALREF);
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = hata.Message;
            }

            return JsonConvert.SerializeObject(response);

        }



        public ActionResult SikayetListesi()
        {
            return View();
        }

        [HttpPost]
        public string TumSikayetler()
        {
            var bayiKodu = Session["BayiKodu"].ToString();
            var db = new Models.IZOKALEPORTALEntities();
            return JsonConvert.SerializeObject(db.Sikayet.Where(x => x.BayiKodu.Equals(bayiKodu) && x.Aktif == true).OrderByDescending(x => x.LOGICALREF), new IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });
        }
        [HttpPost]
        public string ResimleriGetir(string logicalref,string bayiKodu)
        {

            var resimUzantilariListesi = new List<string>();
            //var folderPath = "~/images/sikayet/" + logicalref + "/Gelen";
            string uzanti = "/Files/BayiSikayetleri/" + bayiKodu + "/"+logicalref;
            string dizin = Server.MapPath("~") + uzanti;

            string[] dizindekiDosyalar = Directory.GetFiles(dizin);
            foreach (string dosya in dizindekiDosyalar)
            {
                FileInfo fileInfo = new FileInfo(dosya);
                string dosyaAdi = fileInfo.Name;
                resimUzantilariListesi.Add(uzanti+"/" + dosyaAdi );
            }

            return JsonConvert.SerializeObject(resimUzantilariListesi);
        }

        [HttpPost]
        public void SikayetSil(string logicalref)
        {
            var db = new Models.IZOKALEPORTALEntities();
            Sikayet guncellenecekUrun = db.Sikayet.First(u => u.LOGICALREF.ToString() == logicalref);
            guncellenecekUrun.Aktif = false;
            db.SaveChanges(); // Yukarıda yapılan güncellemeler veritabanına gönderildi

        }

        static string to16(int x)
        {
            if (x > 15) // taban 2'den Küçük Olamayacağı için direk -1 degerini döndürüyoruz. Eğer taban degeri 2 den küçük girilirse.
                return "F";

            return x.ToString("X");


        }
        public static string OnConfirm(string log)
        {
            Random rnd = new Random();
            int gun1 = Convert.ToInt32(DateTime.Now.Day.ToString().Substring(0, 1));
            int gun2;
            try
            {
                gun2 = Convert.ToInt32(DateTime.Now.Day.ToString().Substring(1, 1));
            }
            catch (Exception)
            {

                gun1 = 0;
                gun2 = Convert.ToInt32(DateTime.Now.Day.ToString().Substring(0, 1));
            }

            int ay1 = Convert.ToInt32(DateTime.Now.Month.ToString().Substring(0, 1));
            int ay2;

            try
            {
                ay2 = Convert.ToInt32(DateTime.Now.Month.ToString().Substring(1, 1));
            }
            catch (Exception)
            {

                ay1 = 0;
                ay2 = Convert.ToInt32(DateTime.Now.Month.ToString().Substring(0, 1));
            }

            int yil1 = Convert.ToInt32(DateTime.Now.Year.ToString().Substring(0, 1));
            int yil2 = Convert.ToInt32(DateTime.Now.Year.ToString().Substring(1, 1));
            int yil3 = Convert.ToInt32(DateTime.Now.Year.ToString().Substring(2, 1));
            int yil4 = Convert.ToInt32(DateTime.Now.Year.ToString().Substring(3, 1));

            int mod = rnd.Next(1, 3);
            string sayi = "";
            string logHex = "";
            for (int i = 0; i < log.Length; i++)
            {
                logHex += to16(Convert.ToInt32(log.Substring(i, 1)) + 6);
            }
            if (mod == 1)
            {

                sayi = to16(rnd.Next(1, 10) + 6)//0
                           + to16(mod + 6)//1
                           + to16(rnd.Next(10) + 6)//2
                           + to16(ay2 + 6)//3
                           + to16(rnd.Next(10) + 6)//4
                           + to16(rnd.Next(10) + 6)//5
                           + to16(rnd.Next(10) + 6)//6
                           + to16(yil3 + 6)//7
                           + to16(rnd.Next(10) + 6)//8
                           + to16(gun2 + 6)//9
                           + to16(rnd.Next(10) + 6)//10
                           + to16(rnd.Next(10) + 6)//11
                           + to16(yil1 + 6)//12
                           + to16(rnd.Next(10) + 6)//13
                           + to16(rnd.Next(10) + 6)//14
                           + to16(rnd.Next(10) + 6)//15
                           + to16(yil4 + 6)//16
                           + to16(rnd.Next(10) + 6)//17
                           + to16(rnd.Next(10) + 6)//18
                           + to16(gun1 + 6)//19
                           + to16(rnd.Next(10) + 6)//20
                           + to16(ay1 + 6)//21
                           + to16(rnd.Next(10) + 6)//22
                           + to16(rnd.Next(10) + 6)//23
                           + to16(yil2 + 6)//24
                           + to16(rnd.Next(10) + 6)//25
                           + to16(rnd.Next(10) + 6)//26
                           + logHex//27
                           + to16(log.Length + 6);//28

            }
            else
            {

                sayi = to16(rnd.Next(1, 10) + 6)//0
                         + to16(mod + 6)//1
                         + to16(rnd.Next(10) + 6)//2
                         + to16(yil4 + 6)//3
                         + to16(rnd.Next(10) + 6)//4
                         + to16(rnd.Next(10) + 6)//5
                         + to16(rnd.Next(10) + 6)//6
                         + to16(gun2 + 6)//7
                         + to16(rnd.Next(10) + 6)//8
                         + to16(ay2 + 6)//9
                         + to16(rnd.Next(10) + 6)//10
                         + to16(rnd.Next(10) + 6)//11
                         + to16(yil3 + 6)//12
                         + to16(rnd.Next(10) + 6)//13
                         + to16(rnd.Next(10) + 6)//14
                         + to16(rnd.Next(10) + 6)//15
                         + to16(gun1 + 6)//16
                         + to16(rnd.Next(10) + 6)//17
                         + to16(rnd.Next(10) + 6)//18
                         + to16(yil2 + 6)//19
                         + to16(rnd.Next(10) + 6)//20
                         + to16(ay1 + 6)//21
                         + to16(rnd.Next(10) + 6)//22
                         + to16(rnd.Next(10) + 6)//23
                         + to16(yil1 + 6)//24
                         + to16(rnd.Next(10) + 6)//25
                         + to16(rnd.Next(10) + 6)//26
                         + logHex//27
                         + to16(log.Length + 6);//28
            }

            return "http://88.250.15.240:8090/PublicData/SikayetDetaylari?KEYLINK=" + sayi;

        }
    }
}