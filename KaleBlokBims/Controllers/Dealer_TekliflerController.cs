using KaleBlokBims.Models.Classlar;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Dealer_TekliflerController : Controller
    {
        // GET: Dealer_Teklifler
        public ActionResult Teklifler()
        {
            return View();
        }
        [HttpPost]
        public string TumTeklifler()
        {
            try
            {
                string bayiKodu = Session["BayiKodu"].ToString();
                var db = new Models.IZOKALEPORTALEntities();
                var query = (from a in db.TeklifBasliklari.ToList()
                             where a.BayiKodu == bayiKodu && Convert.ToBoolean(a.OnaylandiMi) == true && Convert.ToBoolean(a.SilindiMi) != true
                             select new
                             {
                                 ReferansNo = a.BayiKodu + "/" + a.LOGICALREF,
                                 a.LOGICALREF,
                                 a.FiyatListesi,
                                 Adres = (Convert.ToBoolean(a.FabrikaTeslimMi)) ? "Fabrika Teslim" : a.Il + "/" + a.Ilce,
                                 a.TeklifSonGecerlilikTarihi,
                                 SipariseDonduMu = (Convert.ToBoolean(a.SipariseDonduMu)) ? "EVET" : "HAYIR",
                                 a.MailAdresi,
                                 Durum = (Convert.ToBoolean(a.SipariseDonduMu)) ? "SİPARİŞE DÖNDÜ" :
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
        public string TeklifAktar(string refNo, string ilgiliKisi, string ilgiliKisiTel, string detayliAdres, string odemeTipi, string siparisNotu)
        {
            RestResponse response = new RestResponse();
            var db = new Models.IZOKALEPORTALEntities();
            var trans = db.Database.BeginTransaction();
            try
            {
                var LOGICALREF = refNo.Split('/')[1];
                var teklif = db.TeklifBasliklari.Where(x => x.LOGICALREF.ToString() == LOGICALREF && x.OnaylandiMi == true && x.SilindiMi != true).FirstOrDefault();
                if (teklif != null)
                {
                    if (Convert.ToDateTime(teklif.TeklifSonGecerlilikTarihi).AddDays(1) > DateTime.Now)
                    {
                        if (!Convert.ToBoolean(teklif.FabrikaTeslimMi))
                        {
                            if (detayliAdres.Trim() == "")
                            {
                                trans.Commit();
                                response.IsSuccessStatusCode = false;
                                response.ErrorMessage = "Detaylı adres girilmelidir.";
                                return JsonConvert.SerializeObject(response);
                            }
                        }
                        if (ilgiliKisi.Trim() == "" || ilgiliKisiTel.Trim() == "" || odemeTipi == "Seçiniz")
                        {
                            trans.Commit();
                            response.IsSuccessStatusCode = false;
                            response.ErrorMessage = "İlgili kişi ve Ödeme tipi bilgileri girilmelidir.";
                            return JsonConvert.SerializeObject(response);
                        }
                        var baslik = new Models.SiparisBasliklari();
                        baslik.AdresBasligi = "";
                        baslik.BayiKodu = teklif.BayiKodu;
                        baslik.BayiAdi = teklif.BayiAdi;
                        baslik.EklenmeTarihi = DateTime.Now;
                        baslik.FabrikaTeslimMi = Convert.ToBoolean(teklif.FabrikaTeslimMi);
                        baslik.FiyatListesi = teklif.FiyatListesi;
                        baslik.OdemeTipi = teklif.OdemeTipi;
                        baslik.BaglantiLref = Convert.ToInt32(-1);
                        baslik.Il = teklif.Il;
                        baslik.Ilce = teklif.Ilce;
                        baslik.IlgiliKisi = ilgiliKisi;
                        baslik.IlgiliKisiTel = ilgiliKisiTel;
                        baslik.MailAdresi = Session["MailAdresi"].ToString();
                        baslik.OnaylandiMi = true;
                        baslik.OnaylanmaTarihi = DateTime.Now;
                        baslik.SevkAdresi = detayliAdres;
                        baslik.SilindiMi = false;
                        baslik.SiparisNotu = siparisNotu;
                        baslik.OdemeTipi = odemeTipi;
                        baslik.TigereAktarildiMi = true;
                        baslik.TigereAktarilmaTarihi = DateTime.Now;
                        baslik.TigereAktaranKisi = "Tigere Aktarılmadı Bayi Teklifi Onayladı Sadece";
                        baslik.FisiOlusturanAdminMi = (Session["AdminMi"].ToString() == "0") ? false : true;
                        db.SiparisBasliklari.Add(baslik);
                        db.SaveChanges();

                        foreach (var item in db.TeklifIcerikleri.Where(x => x.BaslikLREF == teklif.LOGICALREF && x.LINETYPE == 0))
                        {
                            var icerik = new Models.SiparisIcerikleri();
                            icerik.AltGrup = item.AltGrup;
                            icerik.AnaGrup = item.AnaGrup;
                            icerik.FiyatListesi = item.FiyatListesi;
                            icerik.LINETYPE = 0;
                            icerik.BaseDoviz = item.BaseDoviz;
                            icerik.BaseFiyat = Convert.ToDouble(item.BaseFiyat.ToString().Replace(".", ","));
                            icerik.BaslikLREF = baslik.LOGICALREF;
                            icerik.Birimi = item.Birimi;
                            icerik.EklenmeTarihi = DateTime.Now;
                            icerik.GuncelEUR = Convert.ToDouble(item.GuncelEUR.ToString().Replace(".", ","));
                            icerik.GuncelUSD = Convert.ToDouble(item.GuncelUSD.ToString().Replace(".", ","));
                            icerik.HesaplamaDetayliAciklama = item.HesaplamaDetayliAciklama;
                            icerik.HesaplanmisBirimFiyatiTL = Convert.ToDouble(item.HesaplanmisBirimFiyatiTL.ToString().Replace(".", ","));
                            icerik.MalzemeAdi = item.MalzemeAdi;
                            icerik.MalzemeKodu = item.MalzemeKodu;
                            icerik.Miktar = Convert.ToDouble(item.Miktar.ToString().Replace(".", ","));
                            icerik.NakliyeFiyatiTL = Convert.ToDouble(item.NakliyeFiyatiTL.ToString().Replace(".", ","));
                            icerik.SabitEUR = Convert.ToDouble(item.SabitEUR.ToString().Replace(".", ","));
                            icerik.SabitUSD = Convert.ToDouble(item.SabitUSD.ToString().Replace(".", ","));
                            icerik.Kdv = Convert.ToDouble(item.Kdv.ToString().Replace(".", ","));
                            icerik.Editable = false;
                            icerik.SistemKalemiMi = false;
                            db.SiparisIcerikleri.Add(icerik);
                            db.SaveChanges();
                            foreach (var item2 in db.TeklifIcerikleri.Where(x => x.IndiriminUygulanacagiLOGICALREF.ToString() == item.LOGICALREF.ToString()))
                            {
                                var indirim = new Models.SiparisIcerikleri();
                                indirim.BaslikLREF = baslik.LOGICALREF;
                                indirim.IndirimAciklamasi = item2.IndirimAciklamasi;
                                indirim.IndiriminUygulanacagiLOGICALREF = Convert.ToInt32(icerik.LOGICALREF);
                                indirim.IndirimTutari = Convert.ToDouble(item2.IndirimTutari.ToString().Replace(".", ","));
                                indirim.HesaplanmisBirimFiyatiTL = Convert.ToDouble(item2.HesaplanmisBirimFiyatiTL.ToString().Replace(".", ","));
                                indirim.LINETYPE = 2;
                                indirim.EklenmeTarihi = DateTime.Now;
                                indirim.Editable = false;
                                db.SiparisIcerikleri.Add(indirim);
                                db.SaveChanges();
                            }
                            foreach (var item2 in db.TeklifIcerikleri.Where(x => x.NakliyeninUygulanacagiLref.ToString() == item.LOGICALREF.ToString()))
                            {
                                var nakliye = new Models.SiparisIcerikleri();
                                nakliye.Editable = false;
                                nakliye.BaslikLREF = baslik.LOGICALREF;
                                nakliye.EklenmeTarihi = DateTime.Now;
                                nakliye.FiyatListesi = item2.FiyatListesi;
                                nakliye.LINETYPE = 4;
                                nakliye.Miktar = item2.Miktar;
                                nakliye.NakliyeAdi = item2.NakliyeAdi;
                                nakliye.NakliyeBirimSeti = item2.NakliyeBirimSeti;
                                nakliye.NakliyeFiyatiTL = Convert.ToDouble(item2.NakliyeFiyatiTL.ToString().Replace(".", ","));
                                nakliye.NakliyeKartiLref = Convert.ToInt32(item2.NakliyeKartiLref);
                                nakliye.NakliyeKodu = item2.NakliyeKodu;
                                nakliye.NakliyeninUygulanacagiLref = icerik.LOGICALREF;
                                db.SiparisIcerikleri.Add(nakliye);
                                db.SaveChanges();
                            }
                        }
                        teklif.SipariseDonduMu = true;
                        db.SaveChanges();
                        trans.Commit();
                        var firmaAdmini = "";
                        foreach (var item in db.BayiKullanicilari.Where(x => x.BayiKodu == baslik.BayiKodu && x.AdminMi == true))
                        {
                            firmaAdmini += item.MailAdresi + ",";
                        }
                        SiparisFormuOlustur form = new SiparisFormuOlustur();
                        var pdfByte = form.siparisFormu(Convert.ToInt32(baslik.LOGICALREF));
                        MailGonderme mail = new MailGonderme();
                        mail.EkliMailGonderme("", SabitTanimlar.SiparisFormuGonderilecekMailler(), baslik.MailAdresi + "," + firmaAdmini, teklif.BayiKodu + "-" + teklif.LOGICALREF+ " - Teklif Onay Formu" , teklif.BayiAdi + " Tarafından oluşturulan " + teklif.BayiKodu + "-" + teklif.LOGICALREF + " referans numaralı teklif onaylanmıştır. Teklife ait sipariş formu ekte yer almaktadır.", pdfByte, teklif.BayiKodu + "-" + teklif.LOGICALREF + ".pdf");
                        response.IsSuccessStatusCode = true;
                        String file = Convert.ToBase64String(pdfByte);
                        response.Content = file;

                    }
                    else
                    {
                        trans.Commit();
                        response.IsSuccessStatusCode = false;
                        response.ErrorMessage = "TEKLİFİN SÜRESİ DOLDU";
                    }

                }
                else
                {
                    trans.Commit();
                    response.IsSuccessStatusCode = false;
                    response.ErrorMessage = "TEKLİF BULUNAMADI";
                }

            }
            catch (Exception hata)
            {
                trans.Rollback();
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = hata.Message;
            }

            return JsonConvert.SerializeObject(response);

        }
        [HttpPost]
        public string DownloadPdf(string baslikLREF)
        {
            RestResponse response = new RestResponse();
            try
            {
                TeklifFormuOlustur form = new TeklifFormuOlustur();
                var pdfByte = form.teklifFormu(Convert.ToInt32(baslikLREF));
                response.IsSuccessStatusCode = true;
                String file = Convert.ToBase64String(pdfByte);
                response.Content = file;

            }
            catch (Exception hata)
            {
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = hata.Message;
            }
            return JsonConvert.SerializeObject(response);

        }
        [HttpPost]
        public string teklifiSil(string baslikLREF, string SilinmeSebebi)
        {
            try
            {
                if (SilinmeSebebi.Trim()=="")
                {
                    return "Teklif Silme Sebebini Yazmak Zorunludur.";

                }
                var db = new Models.IZOKALEPORTALEntities();
                var sip = db.TeklifBasliklari.Where(x => x.LOGICALREF.ToString() == baslikLREF).FirstOrDefault();
                sip.SilindiMi = true;
                sip.SilinmeSebebi = SilinmeSebebi + " ("+ ((Session["AdminMi"].ToString() == "0")?"Bayi":"Admin") + "= " + Session["MailAdresi"].ToString() + ")";
                sip.SilinmeTarihi = DateTime.Now;
                db.SaveChanges();
                return "ok";
            }
            catch (Exception hata)
            {
                return hata.Message;
            }
        }
    }
}