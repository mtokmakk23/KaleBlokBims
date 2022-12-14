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
    public class Admin_BekleyenSiparislerController : Controller
    {
        // GET: Admin_BekleyenSiparisler
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string bekleyenSiparisler()
        {
            var db = new Models.IZOKALEPORTALEntities();
            //var query = db.SiparisBasliklari.Where(x=>x.OnaylandiMi==true && x.SilindiMi!=true).ToList();

            var query = from a in db.SiparisBasliklari
                        where a.OnaylandiMi == true && a.SilindiMi != true
                        select new
                        {
                            a.LOGICALREF,
                            a.AdresBasligi,
                            a.BayiAdi,
                            a.BayiKodu,
                            a.EklenmeTarihi,
                            a.FabrikaTeslimMi,
                            a.FiyatListesi,
                            a.Il,
                            a.Ilce,
                            a.IlgiliKisi,
                            a.IlgiliKisiTel,
                            a.MailAdresi,
                            a.OdemeTipi,
                            a.OnaylandiMi,
                            a.OnaylanmaTarihi,
                            a.SevkAdresi,
                            a.SilenKisi,
                            a.SilindiMi,
                            a.SilinmeSebebi,
                            a.SilinmeTarihi,
                            a.SiparisNotu,
                            a.TigereAktaranKisi,
                            a.TigereAktarildiMi,
                            a.TigereAktarilmaTarihi,
                            ReferansNo = a.BayiKodu + "/" + a.LOGICALREF
                        };
            return JsonConvert.SerializeObject(query, new IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });

        }
        [HttpPost]
        public string CarileriGetir(string bayiKodu)
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.MusteriyeBagliCariBilgileri(bayiKodu);
        }
        [HttpPost]
        public string OdemeleriGetir()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.WCFOdemePlanlari();
        }
        [HttpPost]
        public string SatisElemanlarini()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.WCFSatisElemanlari();
        }

        [HttpPost]
        public string IsyeriBilgileri()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.IsyeriBilgileri();
        }
        [HttpPost]
        public string FabrikaBilgileri()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.FabrikaBilgileri();
        }

        [HttpPost]
        public string BolumBilgileri()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.BolumBilgileri();
        }

        [HttpPost]
        public string AmbarBilgileri()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.AmbarBilgileri();
        }

        [HttpPost]
        public string Malzemeler(string LOGICALREF)
        {
            var db = new Models.IZOKALEPORTALEntities();
            var query = from a in db.SiparisIcerikleri
                        where a.BaslikLREF.ToString() == LOGICALREF && a.LINETYPE == 0
                        select new
                        {
                            a.BaslikLREF,
                            a.Birimi,
                            a.FiyatListesi,
                            a.HesaplamaDetayliAciklama,
                            a.HesaplanmisBirimFiyatiTL,
                            a.IndirimAciklamasi,
                            a.IndiriminUygulanacagiLOGICALREF,
                            a.IndirimTutari,
                            a.Kdv,
                            a.LINETYPE,
                            a.LOGICALREF,
                            a.MalzemeAdi,
                            a.MalzemeKodu,
                            a.Miktar,
                            a.Editable,
                            a.NakliyeAdi,
                            a.NakliyeBirimSeti,
                            a.NakliyeFiyatiTL,
                            a.NakliyeKodu,                            
                            Turu = (a.LINETYPE == 0) ? "Malzeme" :
                                 (a.LINETYPE == 1) ? "Promosyon" :
                                 (a.LINETYPE == 2) ? "İndirim" :
                                 (a.LINETYPE == 3) ? "Ek Ücret" :
                                 (a.LINETYPE == 4) ? "Hizmet" :
                                 (a.LINETYPE == 5) ? "Depozito" :
                                 (a.LINETYPE == 6) ? "Karma Kasa" :
                                 (a.LINETYPE == 7) ? "Karma Kasa Hattı" :
                                 (a.LINETYPE == 8) ? "Sabit Kıymet" :
                                 (a.LINETYPE == 9) ? "Opsiyonel Malzeme" :
                                 (a.LINETYPE == 10) ? "Malzeme Sınıfı" :
                                 (a.LINETYPE == 11) ? "Taşeronluk" : "",
                            Indirimler = db.SiparisIcerikleri.Where(x => x.IndiriminUygulanacagiLOGICALREF.ToString() == a.LOGICALREF.ToString()),
                            Nakliyeler= db.SiparisIcerikleri.Where(x => x.NakliyeninUygulanacagiLref.ToString() == a.LOGICALREF.ToString()),

                        };
            return JsonConvert.SerializeObject(query, new IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });

        }
        [HttpPost]
        public string deleteRow(string itemRef)
        {
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                db.Database.ExecuteSqlCommand("delete from SiparisIcerikleri where LOGICALREF=" + itemRef);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception hata)
            {
                return hata.Message;
            }
        }
        [HttpPost]
        public string siparisiSil(string baslikLREF, string SilinmeSebebi)
        {
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                var sip = db.SiparisBasliklari.Where(x => x.LOGICALREF.ToString() == baslikLREF).FirstOrDefault();
                sip.SilenKisi = "Admin= " + Session["MailAdresi"].ToString();
                sip.SilindiMi = true;
                sip.SilinmeSebebi = SilinmeSebebi;
                sip.SilinmeTarihi = DateTime.Now;
                db.SaveChanges();
                return "ok";
            }
            catch (Exception hata)
            {
                return hata.Message;
            }
        }
        [HttpPost]
        public string indirimiKaydet(string itemREF, string lineType, string indirimAciklamasi, string indirimTutari, string HesaplanmisBirimFiyatiTL)
        {
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                var indirim = new SiparisIcerikleri();
                indirim.BaslikLREF = db.SiparisIcerikleri.Where(x => x.LOGICALREF.ToString() == itemREF).FirstOrDefault().BaslikLREF;
                indirim.IndirimAciklamasi = indirimAciklamasi;
                indirim.IndiriminUygulanacagiLOGICALREF = Convert.ToInt32(itemREF);
                indirim.IndirimTutari = Convert.ToDouble(indirimTutari.Replace(".", ","));
                indirim.HesaplanmisBirimFiyatiTL = Convert.ToDouble(HesaplanmisBirimFiyatiTL.Replace(".", ","));
                indirim.LINETYPE = 2;
                indirim.EklenmeTarihi = DateTime.Now;
                indirim.Editable = true;
                db.SiparisIcerikleri.Add(indirim);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception hata)
            {
                return hata.Message;
            }

        }

        [HttpPost]
        public string siparisAktar(string tarih, string belgeNo, string referansNo, string cari, string odemeler, string satisElemani, string siparisNotu, string isYeri, string bolum, string fabrika, string ambar)
        {
            //return "";
            M2BWebService.M2BWCFBaslik baslik = new M2BWebService.M2BWCFBaslik();
            M2BWebService.CariSevkAdresi sevkAdresi = new M2BWebService.CariSevkAdresi();
            List<M2BWebService.M2BWCFTransaction> transactions = new List<M2BWebService.M2BWCFTransaction>();
            var db = new Models.IZOKALEPORTALEntities();
            var LOGICALREF = referansNo.Split('/')[1];
            var siparisBasligi = db.SiparisBasliklari.Where(x => x.LOGICALREF.ToString() == LOGICALREF).FirstOrDefault();
            var siparisIcerigi = db.SiparisIcerikleri.Where(x => x.BaslikLREF.ToString() == LOGICALREF && x.LINETYPE != 2).ToList();

            try
            {
                if (Convert.ToBoolean(siparisBasligi.FabrikaTeslimMi))
                {
                    baslik.Aciklama1 = "Fabrika Teslimi";
                }
                else
                {
                    baslik.Aciklama1 = siparisBasligi.Ilce + "/" + siparisBasligi.Il;
                }

                if (siparisBasligi.SiparisNotu.Length > 50)
                {
                    baslik.Aciklama2 = siparisBasligi.SiparisNotu.Substring(0, 50);
                    baslik.Aciklama3 = siparisBasligi.SiparisNotu.Substring(50, siparisBasligi.SiparisNotu.Length - 50);
                }
                else
                {
                    baslik.Aciklama2 = siparisBasligi.SiparisNotu;
                }
                baslik.Ambar = Convert.ToInt32(ambar).ToString();
                baslik.BelgeNo = belgeNo;
                baslik.Bolum = Convert.ToInt32(bolum).ToString();
                baslik.CariLREF = cari;
                baslik.DokumanIzlemeNumarasi = "";
                baslik.Fabrika = Convert.ToInt32(fabrika).ToString();
                baslik.Isyeri = Convert.ToInt32(isYeri).ToString();
                baslik.MusteriSiparisNo = siparisBasligi.BayiKodu + "/" + siparisBasligi.LOGICALREF;
                baslik.OdemeTipiKodu = odemeler;
                baslik.OzelKod = "";
                baslik.SatisElemaniKodu = satisElemani;
                baslik.Tarih = tarih;

                sevkAdresi.Aciklama1 = siparisBasligi.BayiAdi;
                sevkAdresi.Adres1 = siparisBasligi.SevkAdresi;
                sevkAdresi.Adres2 = siparisBasligi.IlgiliKisi + " - " + siparisBasligi.IlgiliKisiTel;
                sevkAdresi.AdresBasligi = siparisBasligi.AdresBasligi;
                if (Convert.ToBoolean(siparisBasligi.FabrikaTeslimMi))
                {
                    sevkAdresi.Il = "ERZURUM";
                    sevkAdresi.Ilce = "OLTU";
                }
                else
                {
                    sevkAdresi.Il = siparisBasligi.Il;
                    sevkAdresi.Ilce = siparisBasligi.Ilce;
                }

                foreach (var item in siparisIcerigi.Where(x => x.LINETYPE == 0))
                {
                    transactions.Add(new M2BWebService.M2BWCFTransaction()
                    {
                        Birim = item.Birimi,
                        BirimFiyat = Convert.ToDouble(item.HesaplanmisBirimFiyatiTL),
                        HareketOzelKodu = "",
                        //IskontoOrani=0,
                        Kdv = Convert.ToDouble(item.Kdv),
                        KdvHaricmi0 = 0,
                        MalzemeKodu = item.MalzemeKodu,
                        Miktar = Convert.ToDouble(item.Miktar),
                        SatirTipi = 0,
                        SatirAciklamasi = (item.HesaplamaDetayliAciklama.Length > 250) ? item.HesaplamaDetayliAciklama.Substring(0, 249) : item.HesaplamaDetayliAciklama,
                        Toplam = Convert.ToDouble(item.HesaplanmisBirimFiyatiTL) * Convert.ToDouble(item.Miktar)


                    });
                    var temp = item.LOGICALREF;
                    var queryIndirim = siparisIcerigi.Where(x => (x.IndiriminUygulanacagiLOGICALREF == (temp)) && (x.LINETYPE.Equals(2))).ToList();
                    foreach (var item2 in queryIndirim)
                    {
                        transactions.Add(new M2BWebService.M2BWCFTransaction()
                        {
                            Miktar = 0,
                            SatirTipi = 2,
                            SatirAciklamasi = (item2.IndirimAciklamasi.Length > 250) ? item2.IndirimAciklamasi.Substring(0, 249) : item2.IndirimAciklamasi,
                            Toplam = Convert.ToDouble(item2.IndirimTutari)



                        });
                    }
                    var queryHizmet = siparisIcerigi.Where(x => (x.NakliyeninUygulanacagiLref == (temp)) && (x.LINETYPE.Equals(4))).ToList();
                    foreach (var item2 in queryHizmet)
                    {
                        transactions.Add(new M2BWebService.M2BWCFTransaction()
                        {
                            Birim = item.NakliyeBirimSeti,
                            BirimFiyat = Convert.ToDouble(item.NakliyeFiyatiTL),
                            HareketOzelKodu = "",
                            //IskontoOrani=0,
                            Kdv = Convert.ToDouble(item.Kdv),
                            KdvHaricmi0 = 0,
                            MalzemeKodu = item.NakliyeKodu,
                            Miktar = Convert.ToDouble(item.Miktar),
                            SatirTipi = 4,
                            SatirAciklamasi = "",
                            Toplam = Convert.ToDouble(item.NakliyeFiyatiTL) * Convert.ToDouble(item.Miktar)

                        });
                    }
                }
                var servis = new M2BWebService.ZOKALEAPISoapClient();
                var tigerDonenCevap = servis.M2BSiparisOlustur(Convert.ToInt32(ambar).ToString(), baslik, transactions.ToArray(), sevkAdresi);
                if (tigerDonenCevap.Substring(0, 3) == "OK ")
                {
                    siparisBasligi.TigereAktaranKisi = Session["MailAdresi"].ToString();
                    siparisBasligi.TigereAktarildiMi = true;
                    siparisBasligi.TigereAktarilmaTarihi = DateTime.Now;
                    db.SaveChanges();
                    return "ok";
                }
                else
                {
                    return tigerDonenCevap;
                }

            }
            catch (Exception hata)
            {
                return hata.Message;
            }

        }

    }
}