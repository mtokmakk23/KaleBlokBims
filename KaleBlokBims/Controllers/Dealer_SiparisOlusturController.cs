using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Dealer_SiparisOlusturController : Controller
    {
        // GET: Dealer_SiparisOlustur
        public ActionResult Index()
        {
            string mailAdresi = Session["MailAdresi"].ToString();
            var db = new Models.IZOKALEPORTALEntities();
            var query = from m in db.ANKT_Master
                        where m.AnketTipi == "Bayiler İçin" && m.Status == "Yayında"
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

            foreach (var item in query)
            {
                if (item.cevaplananSoruSayisi<item.toplamSoruSayisi)
                {
                    return Content("<script language='javascript' type='text/javascript'>alert('Anket Doldurulmadan Sipariş Oluşturulamaz!');window.location='/Dealer_Anket/TumAnketler';</script>"); ;
                }
            }
            return View();
        }
        [HttpPost]
        public string FiyatListeleri()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return JsonConvert.SerializeObject(servis.SecilebilirFiyatListeleri(Session["BayiKodu"].ToString()));
        }
         [HttpPost]
        public string Iller()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.IlveIlceleriGetir();
        }
         [HttpPost]
        public string KayitliAdresler()
        {
            var db = new Models.IZOKALEPORTALEntities();
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return JsonConvert.SerializeObject(db.KayitliAdresler.ToList().Where(x => x.BayiKodu == Session["BayiKodu"].ToString()));
        }


        [HttpPost]
        public string Gruplar(string fiyatListesi)
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return servis.AnaGruplar("","","");
        }
        [HttpPost]
        public string adresKontrol(string adresBasligi, string ilgiliKisi, string ilgiliKisiTel, string detayliAdres, string il, string ilce, string fabrikaTeslimMi)
        {
            RestResponse response = new RestResponse();
            if (adresBasligi == "" || ilgiliKisi == "" || ilgiliKisiTel == "" )
            {
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = "Lütfen Tüm Bilgileri Doldurunuz";
                return JsonConvert.SerializeObject(response);
            }
            else
            {
                if (Convert.ToBoolean(fabrikaTeslimMi) == false && (il == "Seçiniz" || ilce == "Seçiniz" || detayliAdres == ""))
                {
                    response.IsSuccessStatusCode = false;
                    response.ErrorMessage = "Lütfen Tüm Adres Alanlarını Doldurunuz";
                    return JsonConvert.SerializeObject(response);
                }
                string bayiKodu = Session["BayiKodu"].ToString();
                var db = new Models.IZOKALEPORTALEntities();
                var query = db.KayitliAdresler.Where(x => x.AdresBasligi == adresBasligi && x.BayiKodu == bayiKodu).FirstOrDefault();
                if (query == null)
                {
                    query = new Models.KayitliAdresler();
                    query.AdresBasligi = adresBasligi;
                    query.BayiKodu = bayiKodu;
                    query.DetayliAdres = detayliAdres;
                    query.FabrikaTeslimMi = Convert.ToBoolean(fabrikaTeslimMi);
                    query.Il = il;
                    query.Ilce = ilce;
                    query.IlgiliKisi = ilgiliKisi;
                    query.IlgiliKisiTel = ilgiliKisiTel;
                    db.KayitliAdresler.Add(query);
                    db.SaveChanges();
                }
                else
                {
                    
                    query.DetayliAdres = detayliAdres;
                    query.FabrikaTeslimMi = Convert.ToBoolean(fabrikaTeslimMi);
                    query.Il = il;
                    query.Ilce = ilce;
                    query.IlgiliKisi = ilgiliKisi;
                    query.IlgiliKisiTel = ilgiliKisiTel;
                    db.SaveChanges();

                }
                response.IsSuccessStatusCode = true;
                response.Content = "Adres Başarıyla Kaydedildi";
                return JsonConvert.SerializeObject(response);

            }


        }

        [HttpPost]
        public string SepeteEkle(string fiyatListesi,
            string baglantiLREF,
            string adresBasligi,
            string IlgiliKisi,
            string IlgiliKisiTel,
            string SevkAdresi,
            string Il,
            string Ilce,
            string FabrikaTeslimMi,
            string MalzemeKodu,
            string MalzemeAdi,
            string Birim,
            string AnaGrup,
            string AltGrup,
            string Miktar,
            string HesaplanmisBirimFiyatiTL,
            string BaseFiyat,
            string BaseDoviz,
            string NakliyeFiyatiTL,
            string GuncelEUR,
            string GuncelUSD,
            string SabitUSD,
            string SabitEUR,
            string HesaplamaDetayliAciklama,
            string Kdv,
            string NakliyeKartiLref,
            string NakliyeKodu,
            string NakliyeAdi,
            string NakliyeBirimSeti,
            bool SistemKalemiMi=false)
        {
            RestResponse response = new RestResponse();
            var db = new Models.IZOKALEPORTALEntities();
            var trans = db.Database.BeginTransaction();
            try
            {
               
                var mailAdresi = Session["MailAdresi"].ToString();
                var BayiKodu = Session["BayiKodu"].ToString();
                var BayiAdi = Session["BayiAdi"].ToString();
                var baslik = db.SiparisBasliklari.Where(x => x.MailAdresi == mailAdresi && x.BayiKodu == BayiKodu && x.OnaylandiMi == false && x.SilindiMi != true).FirstOrDefault();
                bool yeniBaslikMi = false;
                if (baslik == null)
                {
                    yeniBaslikMi = true;
                    baslik = new Models.SiparisBasliklari();

                }
                baslik.AdresBasligi = adresBasligi;
                baslik.BayiKodu = BayiKodu;
                baslik.BayiAdi = BayiAdi;
                baslik.EklenmeTarihi = DateTime.Now;
                baslik.FabrikaTeslimMi = Convert.ToBoolean(FabrikaTeslimMi);
                baslik.FiyatListesi = fiyatListesi;
                baslik.BaglantiLref =Convert.ToInt32(baglantiLREF);
                baslik.Il = Il;
                baslik.Ilce = Ilce;
                baslik.IlgiliKisi = IlgiliKisi;
                baslik.IlgiliKisiTel = IlgiliKisiTel;
                baslik.MailAdresi = mailAdresi;
                baslik.OnaylandiMi = false;
                baslik.SevkAdresi = SevkAdresi;
                baslik.SilindiMi = false;
                baslik.FisiOlusturanAdminMi = (Session["AdminMi"].ToString() == "0") ? false : true;
                if (yeniBaslikMi)
                {
                    db.SiparisBasliklari.Add(baslik);
                }
                db.SaveChanges();

                var icerik = new Models.SiparisIcerikleri();
                icerik.AltGrup = AltGrup;
                icerik.AnaGrup = AnaGrup;
                icerik.FiyatListesi = fiyatListesi;
                icerik.LINETYPE = 0;
                icerik.BaseDoviz = BaseDoviz;
                icerik.BaseFiyat = Convert.ToDouble(BaseFiyat.Replace(".",","));
                icerik.BaslikLREF = baslik.LOGICALREF;
                icerik.Birimi = Birim;
                icerik.EklenmeTarihi = DateTime.Now;
                icerik.GuncelEUR = Convert.ToDouble(GuncelEUR.ToString().Replace(".",","));
                icerik.GuncelUSD = Convert.ToDouble(GuncelUSD.ToString().Replace(".", ","));
                icerik.HesaplamaDetayliAciklama = HesaplamaDetayliAciklama;
                icerik.HesaplanmisBirimFiyatiTL = Convert.ToDouble(HesaplanmisBirimFiyatiTL.ToString().Replace(".", ","));
                icerik.MalzemeAdi = MalzemeAdi;
                icerik.MalzemeKodu = MalzemeKodu;
                icerik.Miktar = Convert.ToDouble(Miktar.ToString().Replace(".", ","));
                icerik.NakliyeFiyatiTL = Convert.ToDouble(NakliyeFiyatiTL.ToString().Replace(".", ","));
                icerik.SabitEUR = Convert.ToDouble(SabitEUR.ToString().Replace(".", ","));
                icerik.SabitUSD = Convert.ToDouble(SabitUSD.ToString().Replace(".", ","));
                icerik.Kdv = Convert.ToDouble(Kdv.ToString().Replace(".", ","));
                icerik.Editable = false;
                icerik.SistemKalemiMi = SistemKalemiMi;
                db.SiparisIcerikleri.Add(icerik);
                db.SaveChanges();

                if (icerik.NakliyeFiyatiTL>0)
                {
                    //Nakliye bilgileri kalem olarak ekleniyor
                    var nakliye = new Models.SiparisIcerikleri();
                    nakliye.Editable = false;
                    nakliye.BaslikLREF = baslik.LOGICALREF;
                    nakliye.EklenmeTarihi = DateTime.Now;
                    nakliye.FiyatListesi = icerik.FiyatListesi;
                    nakliye.LINETYPE = 4;
                    nakliye.Miktar = icerik.Miktar;
                    nakliye.NakliyeAdi = NakliyeAdi;
                    nakliye.NakliyeBirimSeti = NakliyeBirimSeti;
                    nakliye.NakliyeFiyatiTL = Convert.ToDouble(NakliyeFiyatiTL.ToString().Replace(".", ","));
                    nakliye.NakliyeKartiLref = Convert.ToInt32(NakliyeKartiLref);
                    nakliye.NakliyeKodu = NakliyeKodu;
                    nakliye.NakliyeninUygulanacagiLref = icerik.LOGICALREF;
                    db.SiparisIcerikleri.Add(nakliye);
                    db.SaveChanges();

                }

                trans.Commit();
                response.IsSuccessStatusCode = true;
                response.Content = "Sepete Eklendi";
                return JsonConvert.SerializeObject(response);
            }
            catch (Exception hata)
            {
                trans.Rollback();
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = hata.Message;
                return JsonConvert.SerializeObject(response);
            }


        }

    }
}