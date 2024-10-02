using KaleBlokBims.Models;
using KaleBlokBims.Models.Classlar;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Admin_TeklifController : Controller
    {
        // GET: Admin_Teklif
        public ActionResult TumTeklifler()
        {
            return View();
        }


        [HttpPost]
        public string _TumTeklifler()
        {
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                var query = (from a in db.TeklifBasliklari.ToList()
                             where Convert.ToBoolean(a.OnaylandiMi) == true
                             orderby a.EklenmeTarihi descending
                             select new
                             {
                                 ReferansNo = a.BayiKodu + "/" + a.LOGICALREF,
                                 a.BayiAdi,
                                 a.LOGICALREF,
                                 a.FiyatListesi,
                                 Adres = (Convert.ToBoolean(a.FabrikaTeslimMi)) ? "Fabrika Teslim" : a.Il + "/" + a.Ilce,
                                 a.TeklifSonGecerlilikTarihi,
                                 a.Aciklama,
                                 SipariseDonduMu = (Convert.ToBoolean(a.SipariseDonduMu)) ? "EVET" : "HAYIR",
                                 a.MailAdresi,
                                 a.SilinmeSebebi,
                                 Durum = (Convert.ToBoolean(a.SilindiMi)) ? "SİLİNDİ" : 
                                 (Convert.ToBoolean(a.SipariseDonduMu)) ? "SİPARİŞE DÖNDÜ" :
                                 Convert.ToDateTime(a.TeklifSonGecerlilikTarihi).AddDays(1) > DateTime.Now ?
                                 "AÇIK" :
                                 "SÜRESİ DOLDU"
                             }).ToList();


                return JsonConvert.SerializeObject(query, new IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });
            }
            catch (Exception hata)
            {

                throw;
            }

        }

        [HttpPost]
        public string teklifiSil(string baslikLREF, string SilinmeSebebi)
        {
            try
            {
                if (SilinmeSebebi.Trim() == "")
                {
                    return "Teklif Silme Sebebini Yazmak Zorunludur.";

                }
                var db = new Models.IZOKALEPORTALEntities();
                var sip = db.TeklifBasliklari.Where(x => x.LOGICALREF.ToString() == baslikLREF).FirstOrDefault();
                sip.SilindiMi = true;
                sip.SilinmeSebebi = SilinmeSebebi + " (Admin= " + Session["MailAdresi"].ToString() + ")";
                sip.SilinmeTarihi = DateTime.Now;
                db.SaveChanges();
                return "ok";
            }
            catch (Exception hata)
            {
                return hata.Message;
            }
        }











        public ActionResult TeklifOlustur()
        {
            return View();
        }

        [HttpPost]
        public string FiyatListeleri(string bayiKodu)
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            return JsonConvert.SerializeObject(servis.SecilebilirFiyatListeleri(bayiKodu).Where(x=>x.baglantiLREF=="-1"));
        }

        [HttpPost]
        public string MalzemeFiyatlariniGetir(string bayiKodu, string FiyatListesiKodu, string baglantiLREF, string SPECODE1, string SPECODE2, string Il, string Ilce, string fabrikaTeslimMi, string GuncelUSD, string GuncelEUR,string PayplanRef)
        {
            if (SPECODE1 == "-1") SPECODE1 = "";
            if (SPECODE2 == "-1") SPECODE2 = "";


            var servis = new M2BWebService.ZOKALEAPISoapClient();
            var list = JsonConvert.DeserializeObject<List<Malzeme>>(servis.MalzemeListesi(bayiKodu, FiyatListesiKodu, baglantiLREF, SPECODE1, SPECODE2, Il, Ilce, Convert.ToBoolean(fabrikaTeslimMi), Convert.ToDouble(GuncelUSD.ToString().Replace(".", ",")), Convert.ToDouble(GuncelEUR.ToString().Replace(".", ",")), PayplanRef));
            foreach (var item in list)
            {
                item.ResimUrl = UrunResimleri(item.MalzemeKodu, item.SPECODE1);
            }
            return JsonConvert.SerializeObject(list);

        }
        public string UrunResimleri(string malzemeKodu, string anaGrup)
        {
            string path = "/Files/ProductImages/";
            string[] dizindekiDosyalar = Directory.GetFiles(Server.MapPath(path));
            foreach (string dosya in dizindekiDosyalar)
            {
                FileInfo fileInfo = new FileInfo(dosya);
                string dosyaAdi = fileInfo.Name;
                if (dosyaAdi.IndexOf(malzemeKodu) != -1)
                {
                    return path + dosyaAdi;

                }
                else
                {

                    if (dosyaAdi.IndexOf(anaGrup) != -1)
                    {
                        return path + dosyaAdi;

                    }



                }
            }
            return path + "gorselHazirlanıyor.png";
        }


        [HttpPost]
        public string SepeteEkle(
          string bayiKodu,
          string bayiAdi,
          string fiyatListesi,
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
          string PayplanRef,
          bool SistemKalemiMi = false)
        {
            RestResponse response = new RestResponse();
            var db = new Models.IZOKALEPORTALEntities();
            var trans = db.Database.BeginTransaction();
            try
            {

                var mailAdresi = Session["MailAdresi"].ToString();
                var baslik = db.TeklifBasliklari.Where(x => x.MailAdresi == mailAdresi && x.OnaylandiMi == false && x.SilindiMi ==false).FirstOrDefault();
                bool yeniBaslikMi = false;
                if (baslik == null)
                {
                    yeniBaslikMi = true;
                    baslik = new Models.TeklifBasliklari();

                }
                baslik.BayiKodu = bayiKodu;
                baslik.BayiAdi = bayiAdi;
                baslik.EklenmeTarihi = DateTime.Now;
                baslik.FabrikaTeslimMi = Convert.ToBoolean(FabrikaTeslimMi);
                baslik.FiyatListesi = fiyatListesi;
                baslik.BaglantiLref = Convert.ToInt32(baglantiLREF);
                baslik.Il = Il;
                baslik.Ilce = Ilce;
                baslik.MailAdresi = mailAdresi;
                baslik.OnaylandiMi = false;
                baslik.SilindiMi = false;
                baslik.SipariseDonduMu = false;
                baslik.PayplanRef = PayplanRef;
                if (yeniBaslikMi)
                {
                    db.TeklifBasliklari.Add(baslik);
                }
                db.SaveChanges();

                var icerik = new Models.TeklifIcerikleri();
                icerik.AltGrup = AltGrup;
                icerik.AnaGrup = AnaGrup;
                icerik.FiyatListesi = fiyatListesi;
                icerik.LINETYPE = 0;
                icerik.BaseDoviz = BaseDoviz;
                icerik.BaseFiyat = Convert.ToDouble(BaseFiyat.Replace(".", ","));
                icerik.BaslikLREF = baslik.LOGICALREF;
                icerik.Birimi = Birim;
                icerik.EklenmeTarihi = DateTime.Now;
                icerik.GuncelEUR = Convert.ToDouble(GuncelEUR.ToString().Replace(".", ","));
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
                db.TeklifIcerikleri.Add(icerik);
                db.SaveChanges();

                if (icerik.NakliyeFiyatiTL > 0)
                {
                    //Nakliye bilgileri kalem olarak ekleniyor
                    var nakliye = new Models.TeklifIcerikleri();
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
                    db.TeklifIcerikleri.Add(nakliye);
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

        [HttpPost]
        public string acikTeklifler()
        {
            RestResponse response = new RestResponse();
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                var mailAdresi = Session["MailAdresi"].ToString();
                var baslik = db.TeklifBasliklari.Where(x => x.MailAdresi == mailAdresi  && x.OnaylandiMi == false && x.SilindiMi ==false).ToList();
                if (baslik.Count > 0)
                {
                    var LOGICALREF = baslik[0].LOGICALREF.ToString();
                    var query = from a in db.TeklifIcerikleri
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
                                    Indirimler = db.TeklifIcerikleri.Where(x => x.IndiriminUygulanacagiLOGICALREF.ToString() == a.LOGICALREF.ToString()),
                                    Nakliyeler = db.TeklifIcerikleri.Where(x => x.NakliyeninUygulanacagiLref.ToString() == a.LOGICALREF.ToString()),

                                };
                    //return JsonConvert.SerializeObject(query, new IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });
                    var donusturucu = new LINQResultToDataTable();
                    var baslikDT = donusturucu.LINQResultToDataTables(baslik);
                    baslikDT.Columns.Add("Malzemeler");
                    var temp = baslik[0].LOGICALREF.ToString();
                    var icerik = query.ToList();
                    baslikDT.Rows[0]["Malzemeler"] = JsonConvert.SerializeObject(icerik);
                    response.Content = JsonConvert.SerializeObject(baslikDT);
                    response.IsSuccessStatusCode = true;
                    return JsonConvert.SerializeObject(response);
                }
                else
                {
                    response.Content = "Açık Sipariş Yok";
                    response.IsSuccessStatusCode = true;
                    return JsonConvert.SerializeObject(response);
                }
            }
            catch (Exception hata)
            {
                response.Content = hata.Message;
                response.IsSuccessStatusCode = false;
                return JsonConvert.SerializeObject(response);
            }
        }


      

        [HttpPost]
        public string indirimiKaydet(string itemREF, string lineType, string indirimAciklamasi, string indirimTutari, string HesaplanmisBirimFiyatiTL)
        {
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                var indirim = new TeklifIcerikleri();
                indirim.BaslikLREF = db.TeklifIcerikleri.Where(x => x.LOGICALREF.ToString() == itemREF).FirstOrDefault().BaslikLREF;
                indirim.IndirimAciklamasi = indirimAciklamasi;
                indirim.IndiriminUygulanacagiLOGICALREF = Convert.ToInt32(itemREF);
                indirim.IndirimTutari = Convert.ToDouble(indirimTutari.Replace(".", ","));
                indirim.HesaplanmisBirimFiyatiTL = Convert.ToDouble(HesaplanmisBirimFiyatiTL.Replace(".", ","));
                indirim.LINETYPE = 2;
                indirim.EklenmeTarihi = DateTime.Now;
                indirim.Editable = true;
                db.TeklifIcerikleri.Add(indirim);
                db.SaveChanges();
                return "ok";
            }
            catch (Exception hata)
            {
                return hata.Message;
            }

        }

        [HttpPost]
        public string deleteRow(string itemRef)
        {
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                var urun = db.TeklifIcerikleri.Where(x => x.LOGICALREF.ToString() == itemRef).FirstOrDefault();
                db.Database.ExecuteSqlCommand("delete from TeklifIcerikleri where LOGICALREF=" + itemRef + " or IndiriminUygulanacagiLOGICALREF=" + itemRef + " or NakliyeninUygulanacagiLref=" + itemRef);
                db.SaveChanges();
                var kalanUrunToplamlari = db.TeklifIcerikleri.Where(x => x.BaslikLREF == urun.BaslikLREF).ToList();
                if (kalanUrunToplamlari.Count == 0)
                {
                    db.Database.ExecuteSqlCommand("delete from TeklifBasliklari where LOGICALREF=" + urun.BaslikLREF);
                    db.SaveChanges();
                }
                db.SaveChanges();
                return "ok";
            }
            catch (Exception hata)
            {
                return hata.Message;
            }
        }

       
        [HttpPost]
        public string TeklifiKaydet(string gecerlilikTarihi,string odemeTipi,string aciklama)
        {
            if (odemeTipi== "Seçiniz")
            {
                return "Ödeme Tipi Seçilmelidir.";
            }
            if (gecerlilikTarihi=="")
            {
                return "Geçerlilik Tarihi Girilmelidir.";
            }
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                var mailAdresi = Session["MailAdresi"].ToString();
                var baslik = db.TeklifBasliklari.Where(x => x.MailAdresi == mailAdresi && x.OnaylandiMi == false && x.SilindiMi == false).FirstOrDefault();
                if (baslik!=null)
                {
                    baslik.TeklifSonGecerlilikTarihi = Convert.ToDateTime(gecerlilikTarihi);
                    baslik.OnaylandiMi = true;
                    baslik.OnaylanmaTarihi = DateTime.Now;
                    baslik.SipariseDonduMu = false;
                    baslik.OdemeTipi = odemeTipi;
                    baslik.Aciklama = aciklama;
                    db.SaveChanges();
                    TeklifFormuOlustur form = new TeklifFormuOlustur();
                    //form.teklifFormu(baslik.LOGICALREF);
                    var firmaAdmini = "";
                    var bayiKullanicilari = from a in db.BayiKullanicilari
                                            join
                  b in db.BayiKullaniciYetkileri on a.LOGICALREF.ToString() equals b.KullaniciID.ToString()
                                            where a.Status == true && a.BayiKodu==baslik.BayiKodu && (b.FirmaAdminiMi == true || b.TeklifleriGorme == true)
                                            select new
                                            {
                                                a.MailAdresi
                                            };

                    foreach (var item in bayiKullanicilari)
                    {
                        firmaAdmini += item.MailAdresi + ",";
                    }
                    var pdfByte = form.teklifFormu(Convert.ToInt32(baslik.LOGICALREF));
                    MailGonderme mail = new MailGonderme();
                    mail.EkliMailGonderme("", SabitTanimlar.SiparisFormuGonderilecekMailler(), baslik.MailAdresi + "," + firmaAdmini, baslik.BayiKodu + "-" + baslik.LOGICALREF+" - Teklif Formu", baslik.MailAdresi + " Tarafından oluşturulan " + baslik.BayiKodu + "-" + baslik.LOGICALREF + " referans numaralı teklif formu ekte yer almaktadır.", pdfByte, baslik.BayiKodu + "-" + baslik.LOGICALREF + ".pdf");
                    return "ok";
                }
                else
                {
                    return "Bekleyen Teklif Yok";
                }
                  
            }
            catch (Exception hata)
            {
                return hata.Message;
            }
        }
    }
}