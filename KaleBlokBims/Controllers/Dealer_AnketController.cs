using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Dealer_AnketController : Controller
    {
        private class AnketIcerikleri
        {
            public string AnketBilgileri { set; get; }
            public string AnketSorulari { set; get; }
            public string AnketCevaplari { set; get; }
        }
        private class CevapSetting
        {
            public string SoruLRef { set; get; }
            public string CevapLREF { set; get; }
            public string Cevap { set; get; }
        }
        // GET: Dealer_Anket
        public ActionResult TumAnketler()
        {
            var db = new Models.IZOKALEPORTALEntities();
            db.Database.ExecuteSqlCommand("update ANKT_Master set Status='Kapatıldı' where BitisTarihi<GETDATE() and Status='Yayında'");
            return View();
        }
        
        [HttpPost]
        public string _anketleriGetir()
        {
            
                string mailAdresi = Session["MailAdresi"].ToString();
                var db = new Models.IZOKALEPORTALEntities();
                var query = from m in db.ANKT_Master
                            where m.AnketTipi == "Bayiler İçin" && m.Status != "Sil" && m.BaslangicTarihi < DateTime.Now && m.Status != "Hazırlanıyor"
                            orderby m.EklemeTarihi descending
                            select new
                            {
                                m.LOGICALREF,
                                m.Baslik,
                                m.Status,
                                m.BaslangicTarihi,
                                m.BitisTarihi,
                                textStyle = (m.Status == "Hazırlanıyor") ? "warning" :
                                                (m.Status == "Yayında") ? "success" :
                                                  (m.Status == "Kapatıldı") ? "danger" : "default",
                                yuzde = (((from o in db.ANKT_Sonuclar where o.MasterRef.ToString().Equals(m.LOGICALREF.ToString()) && o.BayiKullaniciMail.Equals(mailAdresi) select new { o.SoruRef }).Distinct().Count()) * 100) / (db.ANKT_Sorular.Where(x => x.MasterRef.ToString().Equals(m.LOGICALREF.ToString())).Count()),
                                cevaplananSoruSayisi = ((from o in db.ANKT_Sonuclar where o.MasterRef.ToString().Equals(m.LOGICALREF.ToString()) && o.BayiKullaniciMail.Equals(mailAdresi) select new { o.SoruRef }).Distinct().Count()),
                                toplamSoruSayisi = (db.ANKT_Sorular.Where(x => x.MasterRef.ToString().Equals(m.LOGICALREF.ToString())).Count())
                                // yüzde =(100*cevaplanan soru sayısı) /toplam soru sayısı
                            };
                return JsonConvert.SerializeObject(query, new IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });
            
            
        }

        public ActionResult AnketCevapla(string actionName, string LOGICALREF)
        {
            if (actionName.Trim() == "" || LOGICALREF.Trim() == "")
            {
                return TumAnketler();
            }
            else
           if (actionName == "answer")
            {
                string mailAdresi = Session["MailAdresi"].ToString();
                var db = new Models.IZOKALEPORTALEntities();
                var query = db.ANKT_Master.Where(x =>
                (x.LOGICALREF.ToString() == LOGICALREF) &&
                (x.Status.Equals("Yayında")));
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
        public string _anketiDoldur(string LREF)
        {
            var db = new Models.IZOKALEPORTALEntities();
            var anketlist = new List<AnketIcerikleri>();

            var queryAnketBilgileri = from a in db.ANKT_Master
                                      where a.Status != "Sil" && a.LOGICALREF.ToString() == LREF.ToString()
                                      select a;

            var querySoruBilgileri = from a in db.ANKT_Master
                                     join b in db.ANKT_Sorular on a.LOGICALREF.ToString() equals b.MasterRef.ToString()
                                     where a.Status != "Sil" && a.LOGICALREF.ToString() == LREF.ToString()
                                     select b;

            var queryCevapBilgileri = from a in db.ANKT_Master
                                      join b in db.ANKT_Sorular on a.LOGICALREF.ToString() equals b.MasterRef.ToString()
                                      join c in db.ANKT_SabitCevaplar on b.LOGICALREF.ToString() equals c.SoruRef.ToString()
                                      where a.Status != "Sil" && a.LOGICALREF.ToString() == LREF.ToString()
                                      select c;

            anketlist.Add(new AnketIcerikleri()
            {
                AnketBilgileri = JsonConvert.SerializeObject(queryAnketBilgileri, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" }),
                AnketCevaplari = JsonConvert.SerializeObject(queryCevapBilgileri),
                AnketSorulari = JsonConvert.SerializeObject(querySoruBilgileri),
            });
            return JsonConvert.SerializeObject(anketlist);
        }

        [HttpPost]
        public string _cevaplariYukle(string LREF)
        {
            try
            {
                //select s.SoruLRef,ISNULL(CAST(c.LOGICALREF AS nvarchar),'MEGA'),s.Cevap from ANKT_Sonuclari s left join ANKT_SabitCevaplar c on s.Cevap=c.Label

                var db = new Models.IZOKALEPORTALEntities();

                var anketlist = new List<CevapSetting>();
                var aa = db.Database.SqlQuery<string>("select CAST(s.SoruRef AS nvarchar)+ '#' +ISNULL(CAST(c.LOGICALREF AS nvarchar),'KALE')+ '#' +s.Cevap from ANKT_Sonuclar s left join ANKT_SabitCevaplar c on s.Cevap=c.Label where s.MasterRef=" + LREF + " and s.BayiKullaniciMail='" + Session["MailAdresi"].ToString() + "'").AsEnumerable();
                foreach (var item in aa)
                {
                    anketlist.Add(new CevapSetting()
                    {
                        Cevap = item.Split('#')[2],
                        CevapLREF = item.Split('#')[1],
                        SoruLRef = item.Split('#')[0],
                    });
                }
                // var query = db.ANKT_Sonuclaris.Where(x=>(x.MasterLRef.Equals(LREF)) && (x.BayiKullaniciMail.Equals(HttpContext.Current.Session["BayiKullaniciMail"].ToString())));
                return JsonConvert.SerializeObject(anketlist);
            }
            catch (Exception hata)
            {
                return hata.Message;
            }

        }

        [HttpPost]
        public string _anketiCevapla(string Sonuclar)
        {

            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                dynamic jsonCevaplar = JsonConvert.DeserializeObject(Sonuclar);
                for (int i = 0; i < jsonCevaplar.Sonuclar.Count; i++)
                {
                    db.Database.ExecuteSqlCommand("DELETE from ANKT_Sonuclar WHERE MasterRef = {0} AND BayiKullaniciMail={1}", Convert.ToInt32(jsonCevaplar.Sonuclar[i].anketID), Session["MailAdresi"].ToString());
                    break;
                }
                for (int i = 0; i < jsonCevaplar.Sonuclar.Count; i++)
                {
                    if ((jsonCevaplar.Sonuclar[i].cevap).ToString() != "")
                    {
                        db.Database.ExecuteSqlCommand("insert into ANKT_Sonuclar (MasterRef,SoruRef,Cevap,BayiKullaniciMail,EklemeTarihi,BayiKodu,BayiAdi) values({0},{1},{2},{3},{4},{5},{6})", Convert.ToInt32(jsonCevaplar.Sonuclar[i].anketID), Convert.ToInt32(jsonCevaplar.Sonuclar[i].soruID), (jsonCevaplar.Sonuclar[i].cevap).ToString(), Session["MailAdresi"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm"), Session["BayiKodu"].ToString(), Session["BayiAdi"].ToString());
                
                    }



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