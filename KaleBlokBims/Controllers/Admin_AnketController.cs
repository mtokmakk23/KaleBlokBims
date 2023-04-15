using KaleBlokBims.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Admin_AnketController : Controller
    {
        // GET: Admin_Anket
        public ActionResult AnketOlustur()
        {
            return View();
        }

        [HttpPost]
        public string _AnketiOlustur(string anketTipi, string baslik, string baslangicTarihi, string bitisTarihi, string status, string sorular, string cevaplar)
        {
            try
            {
                string mailAdresi = Session["MailAdresi"].ToString();
                if (status == "Kapatıldı")
                {
                    return "[{\"success\":\"0\",\"message\":\"Durumu Kapalı Olan Anket Oluşturulamaz!\"}]";
                }
                var db = new Models.IZOKALEPORTALEntities();
                if (baslik.Trim() == "")
                {
                    return "[{\"success\":\"0\",\"message\":\"Anket Başlığı Boş Geçilemez!\"}]";
                }

                if (baslangicTarihi.Trim() == "" || bitisTarihi.Trim() == "")
                {
                    return "[{\"success\":\"0\",\"message\":\"Anket Başlangıç Ve Bitiş Tarihi Boş Geçilemez!\"}]";
                }
                dynamic jsonSorular = JsonConvert.DeserializeObject(sorular);
                dynamic jsonCevaplar = JsonConvert.DeserializeObject(cevaplar);
                for (int i = 0; i < jsonSorular.Sorular.Count; i++)
                {
                    if (jsonSorular.Sorular[i].soruMetni == "")
                    {
                        return "[{\"success\":\"0\",\"message\":\"Soru Alanı Boş Geçilemez\"}]";
                    }
                    else if (jsonSorular.Sorular[i].status == "Sadece Bir Cevap" || jsonSorular.Sorular[i].status == "Birden Çok Cevap")
                    {
                        int cevapEklenmis = 0;
                        for (int j = 0; j < jsonCevaplar.Cevaplar.Count; j++)
                        {
                            if (jsonCevaplar.Cevaplar[j].soruID == jsonSorular.Sorular[i].soruID && jsonCevaplar.Cevaplar[j].cevapMetni != "")
                            {
                                cevapEklenmis++;
                            }
                        }
                        if (cevapEklenmis < 2)
                        {
                            return "[{\"success\":\"0\",\"message\":\"" + jsonSorular.Sorular[i].soruMetni + " Sorusuna Cevap Ekleyiniz\"}]";
                        }
                    }

                }
                if (jsonSorular.Sorular.Count == 0)
                {
                    return "[{\"success\":\"0\",\"message\":\"Ankete Soru Eklemelisiniz\"}]";
                }


                ANKT_Master eklenecekUrun = new ANKT_Master()
                {
                    AnketTipi = anketTipi,
                    BaslangicTarihi = Convert.ToDateTime(baslangicTarihi),
                    Baslik = baslik,
                    BitisTarihi = Convert.ToDateTime(bitisTarihi + " 23:59:59"),
                    EklemeTarihi = DateTime.Now,
                    Ekleyen = mailAdresi,
                    Status = status,
                };
                db.ANKT_Master.Add(eklenecekUrun); // Eklenecekler listesine yeni bir ürün eklendi
                db.SaveChanges();

                int anketLGRF = 0;
                var query = db.ANKT_Master.Where(x => x.Ekleyen.Equals(mailAdresi)).OrderByDescending(x => x.EklemeTarihi).Take(1);
                foreach (var item in query)
                {
                    anketLGRF = Convert.ToInt32(item.LOGICALREF);
                }


                int sira = 1;
                for (int i = 0; i < jsonSorular.Sorular.Count; i++)
                {
                    ANKT_Sorular eklenecekSoru = new ANKT_Sorular()
                    {
                        CevapTipi = jsonSorular.Sorular[i].status,
                        MasterRef = anketLGRF,
                        Metin = jsonSorular.Sorular[i].soruMetni,
                        SiraNo = sira
                    };
                    db.ANKT_Sorular.Add(eklenecekSoru); // Eklenecekler listesine yeni bir ürün eklendi
                    db.SaveChanges();

                    var soruLREF = 0;
                    var queryy = db.ANKT_Sorular.Where(x => (x.MasterRef.ToString() == anketLGRF.ToString()) && (x.SiraNo.ToString() == sira.ToString())).Take(1);
                    foreach (var item in queryy)
                    {
                        soruLREF = Convert.ToInt32(item.LOGICALREF);
                    }

                    for (int j = 0; j < jsonCevaplar.Cevaplar.Count; j++)
                    {
                        if (jsonCevaplar.Cevaplar[j].soruID == jsonSorular.Sorular[i].soruID && jsonCevaplar.Cevaplar[j].cevapMetni != "")
                        {
                            ANKT_SabitCevaplar eklenecekCevap = new ANKT_SabitCevaplar()
                            {
                                Label = jsonCevaplar.Cevaplar[j].cevapMetni,
                                SoruRef = soruLREF
                            };
                            db.ANKT_SabitCevaplar.Add(eklenecekCevap); // Eklenecekler listesine yeni bir ürün eklendi
                            db.SaveChanges();
                        }
                    }
                    sira++;
                }
                return "[{\"success\":\"1\",\"message\":\"Anket Başarıyla Oluşturulmuştur.\"}]";
            }
            catch (Exception hata)
            {
                return "[{\"success\":\"0\",\"message\":\"" + hata.Message + "\"}]";
            }






        }


        public ActionResult TumAnketler()
        {
            var db = new Models.IZOKALEPORTALEntities();
            db.Database.ExecuteSqlCommand("update ANKT_Master set Status='Kapatıldı' where BitisTarihi<GETDATE() and Status='Yayında'");
            return View();
        }
        [HttpPost]
        public string _anketler()
        {
            var db = new Models.IZOKALEPORTALEntities();
            var query = from a in db.ANKT_Master
                        where a.Status != "Sil"
                        orderby a.EklemeTarihi descending
                        select new
                        {
                            a.BaslangicTarihi,
                            a.Baslik,
                            a.BitisTarihi,
                            a.EklemeTarihi,
                            a.Ekleyen,
                            a.Status,
                            a.LOGICALREF,
                            buttonClass = (a.Status == "Hazırlanıyor") ? "warning" :
                                           (a.Status == "Yayında") ? "success" :
                                             (a.Status == "Kapatıldı") ? "danger" : "default",
                            cevaplayanKisiSayisi = (from b in db.ANKT_Sonuclar where b.MasterRef == a.LOGICALREF select new { b.BayiKullaniciMail }).Distinct().Count()
                        };
            return JsonConvert.SerializeObject(query, new IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy HH:mm" });
        }
        [HttpPost]
        public string _statuDegistir(string statu, string LREF)
        {
            try
            {

                var db = new Models.IZOKALEPORTALEntities();
                var query = db.ANKT_Sonuclar.Where(x => x.MasterRef.ToString() == LREF.ToString());
                if (query.Count() > 0 && statu == "Hazırlanıyor")
                {
                    return "Cevaplanan Anket Hazırlanıyor'a Çekilemez!";
                }
                else
                {
                    ANKT_Master guncellenecekUrun = db.ANKT_Master.FirstOrDefault(u => u.LOGICALREF.ToString() == LREF.ToString());
                    guncellenecekUrun.Status = statu;
                    db.SaveChanges();
                    return "true";
                }

            }
            catch (Exception hata)
            {
                return hata.Message;
            }

        }


        public ActionResult AnketIslemleri(string actionName, string LOGICALREF)
        {
            if (actionName.Trim() == "" || LOGICALREF.Trim() == "")
            {
                return TumAnketler();
            }
            else
            if (actionName == "edit")
            {
                string mailAdresi = Session["MailAdresi"].ToString();
                var db = new Models.IZOKALEPORTALEntities();
                var query = db.ANKT_Master.Where(x =>
                (x.LOGICALREF.ToString() == LOGICALREF) &&
                (x.Ekleyen == mailAdresi) &&
                (x.Status.Equals("Hazırlanıyor")));
                if (query.Count() != 1)
                {
                    return TumAnketler();
                }
            }
            ViewBag.actionName = actionName;
            ViewBag.LOGICALREF = LOGICALREF;
            return View();
        }

        [HttpPost]
        public string _verileriYerlestir(string LREF)
        {
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                var anketlist = new List<AnketIcerikleri>();

                var queryAnketBilgileri = (from a in db.ANKT_Master
                                           where a.Status != "Sil" && a.LOGICALREF.ToString() == LREF.ToString()
                                           select a).ToList();

                var querySoruBilgileri = (from a in db.ANKT_Master
                                          join b in db.ANKT_Sorular on a.LOGICALREF.ToString() equals b.MasterRef.ToString()
                                          where a.Status != "Sil" && a.LOGICALREF.ToString() == LREF.ToString()
                                          select b).ToList();

                var queryCevapBilgileri = (from a in db.ANKT_Master
                                           join b in db.ANKT_Sorular on a.LOGICALREF.ToString() equals b.MasterRef.ToString()
                                           join c in db.ANKT_SabitCevaplar on b.LOGICALREF.ToString() equals c.SoruRef.ToString()
                                           where a.Status != "Sil" && a.LOGICALREF.ToString() == LREF.ToString()
                                           select c).ToList();

                anketlist.Add(new AnketIcerikleri()
                {
                    AnketBilgileri = JsonConvert.SerializeObject(queryAnketBilgileri, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" }),
                    AnketCevaplari = JsonConvert.SerializeObject(queryCevapBilgileri),
                    AnketSorulari = JsonConvert.SerializeObject(querySoruBilgileri),
                });
                return JsonConvert.SerializeObject(anketlist);
            }
            catch (Exception hata)
            {

                throw;
            }
            
        }

        [HttpPost]
        public string _anketiOlusturIslem(string anketTipi, string baslik, string baslangicTarihi, string bitisTarihi, string status, string sorular, string cevaplar, string action, string LREF)
        {
            if (action == "copy")
            {


                return _AnketiOlustur(anketTipi, baslik, baslangicTarihi, bitisTarihi, status, sorular, cevaplar);


            }
            else if (action == "edit")
            {
                var db = new Models.IZOKALEPORTALEntities();
                if (baslik.Trim() == "")
                {
                    return "[{\"success\":\"0\",\"message\":\"Anket Başlığı Boş Geçilemez!\"}]";
                }

                if (baslangicTarihi.Trim() == "" || bitisTarihi.Trim() == "")
                {
                    return "[{\"success\":\"0\",\"message\":\"Anket Başlangıç Ve Bitiş Tarihi Boş Geçilemez!\"}]";
                }
                dynamic jsonSorular = JsonConvert.DeserializeObject(sorular);
                dynamic jsonCevaplar = JsonConvert.DeserializeObject(cevaplar);
                for (int i = 0; i < jsonSorular.Sorular.Count; i++)
                {
                    if (jsonSorular.Sorular[i].soruMetni == "")
                    {
                        return "[{\"success\":\"0\",\"message\":\"Soru Alanı Boş Geçilemez\"}]";
                    }
                    else if (jsonSorular.Sorular[i].status == "Sadece Bir Cevap" || jsonSorular.Sorular[i].status == "Birden Çok Cevap")
                    {
                        int cevapEklenmis = 0;
                        for (int j = 0; j < jsonCevaplar.Cevaplar.Count; j++)
                        {
                            if (jsonCevaplar.Cevaplar[j].soruID == jsonSorular.Sorular[i].soruID && jsonCevaplar.Cevaplar[j].cevapMetni != "")
                            {
                                cevapEklenmis++;
                            }
                        }
                        if (cevapEklenmis < 2)
                        {
                            return "[{\"success\":\"0\",\"message\":\"" + jsonSorular.Sorular[i].soruMetni + " Sorusuna Cevap Ekleyiniz\"}]";
                        }
                    }

                }
                if (jsonSorular.Sorular.Count == 0)
                {
                    return "[{\"success\":\"0\",\"message\":\"Ankete Soru Eklemelisiniz\"}]";
                }


                ANKT_Master guncellenecekUrun = db.ANKT_Master.FirstOrDefault(u => u.LOGICALREF.ToString() == LREF.ToString());
                guncellenecekUrun.BaslangicTarihi = Convert.ToDateTime(baslangicTarihi);
                guncellenecekUrun.AnketTipi = anketTipi;
                guncellenecekUrun.Baslik = baslik;
                guncellenecekUrun.BitisTarihi = Convert.ToDateTime(bitisTarihi + " 23:59:59");
                guncellenecekUrun.Status = status;
                db.SaveChanges();

                var anketLGRF = Convert.ToInt32(LREF);
                db.Database.ExecuteSqlCommand(" delete FROM ANKT_SabitCevaplar where SoruRef in (select LOGICALREF from ANKT_Sorular where MasterRef={0})", anketLGRF);
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("DELETE from ANKT_Sorular WHERE MasterRef = {0}", anketLGRF);
                db.SaveChanges();
                //db.ExecuteCommand("UPDATE Products SET QuantityPerUnit = {0} WHERE ProductID = {1}", "24 boxes", 5);
                int sira = 1;
                for (int i = 0; i < jsonSorular.Sorular.Count; i++)
                {
                    ANKT_Sorular eklenecekSoru = new ANKT_Sorular()
                    {
                        CevapTipi = jsonSorular.Sorular[i].status,
                        MasterRef = anketLGRF,
                        Metin = jsonSorular.Sorular[i].soruMetni,
                        SiraNo = sira
                    };
                    db.ANKT_Sorular.Add(eklenecekSoru); // Eklenecekler listesine yeni bir ürün eklendi
                    db.SaveChanges();

                    var soruLREF = 0;
                    var queryy = db.ANKT_Sorular.Where(x => (x.MasterRef.ToString().Equals(anketLGRF.ToString())) && (x.SiraNo.ToString().Equals(sira.ToString()))).Take(1);
                    foreach (var item in queryy)
                    {
                        soruLREF = Convert.ToInt32(item.LOGICALREF);
                    }

                    for (int j = 0; j < jsonCevaplar.Cevaplar.Count; j++)
                    {
                        if (jsonCevaplar.Cevaplar[j].soruID == jsonSorular.Sorular[i].soruID && jsonCevaplar.Cevaplar[j].cevapMetni != "")
                        {
                            ANKT_SabitCevaplar eklenecekCevap = new ANKT_SabitCevaplar()
                            {
                                Label = jsonCevaplar.Cevaplar[j].cevapMetni,
                                SoruRef = soruLREF
                            };
                            db.ANKT_SabitCevaplar.Add(eklenecekCevap); // Eklenecekler listesine yeni bir ürün eklendi
                            db.SaveChanges();
                        }
                    }
                    sira++;
                }
                return "[{\"success\":\"1\",\"message\":\"Anket Başarıyla Oluşturulmuştur.\"}]";

            }
            else
            {
                return "";
            }
        }


        private class AnketIcerikleri
        {
            public string AnketBilgileri { set; get; }
            public string AnketSorulari { set; get; }
            public string AnketCevaplari { set; get; }
        }
    }
}