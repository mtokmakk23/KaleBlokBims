using HtmlAgilityPack;
using KaleBlokBims.Models.Classlar;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Dealer_AnasayfaController : Controller
    {
        // GET: Dealer_Anasayfa
        public ActionResult Index()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            ViewBag.HesapOzeti = servis.HesapOzeti(Session["BayiKodu"].ToString());
            ViewBag.BaglantiOzeti = servis.BaglantiBakiyeOzeti(Session["BayiKodu"].ToString());
            var tumBayiler = servis.Bayiler();
            ViewBag.BayiBilgileri = JsonConvert.SerializeObject(tumBayiler.Where(x => x.BayiKodu == Session["BayiKodu"].ToString()));
            return View();
        }
        [HttpPost]
        public string UrunSepettenCikar(string LOGICALREF)
        {
            RestResponse response = new RestResponse();
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                var urun = db.SiparisIcerikleri.Where(x => x.LOGICALREF.ToString() == LOGICALREF).FirstOrDefault();
                db.Database.ExecuteSqlCommand("delete from SiparisIcerikleri where LOGICALREF=" + LOGICALREF + " or IndiriminUygulanacagiLOGICALREF=" + LOGICALREF + " or NakliyeninUygulanacagiLref=" + LOGICALREF);
                db.SaveChanges();
                var kalanUrunToplamlari = db.SiparisIcerikleri.Where(x => x.BaslikLREF == urun.BaslikLREF).ToList();
                if (kalanUrunToplamlari.Count == 0)
                {
                    db.Database.ExecuteSqlCommand("delete from SiparisBasliklari where LOGICALREF=" + urun.BaslikLREF);
                    db.SaveChanges();
                }
                response.IsSuccessStatusCode = true;
                response.Content = "Ürün Sepetten Silindi";
            }
            catch (Exception hata)
            {
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = hata.Message;
            }
            return JsonConvert.SerializeObject(response);
        }
        [HttpPost]
        public string SepetiOnayla(string sepetOnaylaOdemeTipi, string sepetOnaylaSiparisNotu)
        {
            try
            {
                RestResponse response = new RestResponse();
                if (sepetOnaylaOdemeTipi == "Seçiniz")
                {
                    response.IsSuccessStatusCode = false;
                    response.ErrorMessage = "Ödeme Tipi Seçmelisiniz";
                    return JsonConvert.SerializeObject(response);
                }

                var db = new Models.IZOKALEPORTALEntities();
                var mailAdresi = Session["MailAdresi"].ToString();
                var BayiKodu = Session["BayiKodu"].ToString();
                var baslik = db.SiparisBasliklari.Where(x => x.MailAdresi == mailAdresi && x.BayiKodu == BayiKodu && x.OnaylandiMi == false && x.SilindiMi != true).FirstOrDefault();
                if (baslik == null)
                {
                    response.IsSuccessStatusCode = false;
                    response.ErrorMessage = "Açık Siparişiniz Bulunmuyor";
                }
                else
                {

                    baslik.OnaylandiMi = true;
                    baslik.OnaylanmaTarihi = DateTime.Now;
                    baslik.SiparisNotu = sepetOnaylaSiparisNotu;
                    baslik.OdemeTipi = sepetOnaylaOdemeTipi;
                    db.SaveChanges();
                    var firmaAdmini = "";
                    foreach (var item in db.BayiKullanicilari.Where(x => x.BayiKodu == baslik.BayiKodu && x.AdminMi == true))
                    {
                        firmaAdmini += item.MailAdresi + ",";
                    }
                    SiparisFormuOlustur form = new SiparisFormuOlustur();
                    var pdfByte = form.siparisFormu(Convert.ToInt32(baslik.LOGICALREF));
                    MailGonderme mail = new MailGonderme();
                    mail.EkliMailGonderme("", SabitTanimlar.SiparisFormuGonderilecekMailler(), baslik.MailAdresi + "," + firmaAdmini, "Sipariş Formu", baslik.BayiAdi + " Tarafından oluşturulan " + baslik.BayiKodu + "-" + baslik.LOGICALREF + " referans numaralı sipariş formu ekte yer almaktadır.", pdfByte, baslik.BayiKodu + "-" + baslik.LOGICALREF + ".pdf");
                    response.IsSuccessStatusCode = true;
                    String file = Convert.ToBase64String(pdfByte);
                    response.Content = file;
                }
                return JsonConvert.SerializeObject(response);
            }
            catch (Exception hata)
            {
                return hata.Message;
            }
            
        }


        public void sistemKalemleriEkle()
        {
            var db = new Models.IZOKALEPORTALEntities();
            var mailAdresi = Session["MailAdresi"].ToString();
            var BayiKodu = Session["BayiKodu"].ToString();
            var baslik = db.SiparisBasliklari.Where(x => x.MailAdresi == mailAdresi && x.BayiKodu == BayiKodu && x.OnaylandiMi == false && x.SilindiMi == false).FirstOrDefault();
            if (baslik != null)
            {
                var temp = baslik.LOGICALREF.ToString();
                var icerik = db.SiparisIcerikleri.Where(x => x.BaslikLREF.ToString() == temp && x.LINETYPE != 4 && x.LINETYPE != 2 && (x.AnaGrup == "KB_EPS" || x.AnaGrup == "KB_TASYUNU" || x.AnaGrup == "KB_XPS") && x.SistemKalemiMi != true).ToList();
                db.Database.ExecuteSqlCommand("delete from SiparisIcerikleri where BaslikLREF="+temp+ " and SistemKalemiMi=1");
                var servis = new M2BWebService.ZOKALEAPISoapClient();
                foreach (var item in icerik)
                {
                    var sistemKalemleriListesi = servis.SistemKalemleriBilgileriAl(item.AnaGrup);
                    var sistemKalemleriMalzemeKodlari = "";
                    foreach (var item2 in sistemKalemleriListesi)
                    {
                        sistemKalemleriMalzemeKodlari += "'"+item2.Kodu+"',";
                    }
                    sistemKalemleriMalzemeKodlari = sistemKalemleriMalzemeKodlari.Substring(0, sistemKalemleriMalzemeKodlari.Length - 1);
                    var list = servis.SistemKalemleriMalzemeListesi(Session["BayiKodu"].ToString(), baslik.FiyatListesi, baslik.BaglantiLref.ToString(), baslik.Il, baslik.Ilce, Convert.ToBoolean(baslik.FabrikaTeslimMi), Convert.ToDouble(item.GuncelUSD.ToString().Replace(".", ",")), Convert.ToDouble(item.GuncelEUR.ToString().Replace(".", ",")), sistemKalemleriMalzemeKodlari);
                    foreach (var item2 in list)
                    {
                        var aaa = item.Miktar * sistemKalemleriListesi.Where(x => x.Kodu == item2.MalzemeKodu).FirstOrDefault().BirBirimeKullanilacakMiktar;

                        Dealer_SiparisOlusturController so = new Dealer_SiparisOlusturController();
                        so.SepeteEkle(baslik.FiyatListesi,
                            baslik.BaglantiLref.ToString(),
                            baslik.AdresBasligi,
                            baslik.IlgiliKisi,
                            baslik.IlgiliKisiTel,
                            baslik.SevkAdresi,
                            baslik.Il,
                            baslik.Ilce,
                            baslik.FabrikaTeslimMi.ToString(),
                            item2.MalzemeKodu,
                            item2.MalzemeAdi,
                            item2.Birim,
                            item2.SPECODE1,
                            item2.SPECODE2,
                            "1",
                            item2.HesaplanmisBirimFiyatiTL.ToString(),
                            item2.BaseFiyat.ToString(),
                            item2.BaseDoviz,
                            item2.NakliyeMasrafiTL.ToString(),
                            item2.GuncelEUR.ToString(),
                            item2.GuncelUSD.ToString(),
                            item2.sozlesmeUSD.ToString(),
                            item2.sozlesmeEUR.ToString(),
                            item2.HesaplamaDetayliAciklama,
                            item2.Kdv.ToString(),
                            item2.NakliyeKartiLref.ToString(),
                            item2.NakliyeKodu,
                            item2.NakliyeAdi,
                            item2.NakliyeBirimSeti,
                            true);
                    }
                }





            }
        }        
        [HttpPost]
        public string acikSiparisler()
        {
            RestResponse response = new RestResponse();
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                var mailAdresi = Session["MailAdresi"].ToString();
                var BayiKodu = Session["BayiKodu"].ToString();
                var baslik = db.SiparisBasliklari.Where(x => x.MailAdresi == mailAdresi && x.BayiKodu == BayiKodu && x.OnaylandiMi == false && x.SilindiMi!=true ).ToList();
                if (baslik.Count > 0)
                {
                    var donusturucu = new LINQResultToDataTable();
                    var baslikDT = donusturucu.LINQResultToDataTables(baslik);
                    baslikDT.Columns.Add("Malzemeler");
                    var temp = baslik[0].LOGICALREF.ToString();
                    var icerik = db.SiparisIcerikleri.Where(x => x.BaslikLREF.ToString() == temp && x.LINETYPE != 4).ToList();
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
        public string DovizCek()
        {
            var dt = new DataTable();
            dt.Columns.Add("USD");
            dt.Columns.Add("EUR");
            dt.Columns.Add("guncellemeSaati");
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                IWebProxy theProxy = WebRequest.DefaultWebProxy;
                if (theProxy is object)
                {
                    theProxy.Credentials = CredentialCache.DefaultCredentials;
                }
                var web = new HtmlWeb();


                var document = web.Load("https://www.bloomberght.com/doviz");
                var page = document.DocumentNode;
                var fiyatlar = page.SelectNodes("//*[@id=\"HeaderMarkets\"]/ul/li");
                if (fiyatlar == null)
                {
                    return null;
                }
                var dolarsatisfyt = "";
                var eurosatisfyt = "";
                for (int i = 1; i < fiyatlar.Count; i++)
                {

                    var kurIsmi = (fiyatlar[i].SelectSingleNode(".//small[contains(@class, 'title')]")).InnerText.Trim();
                    if (kurIsmi.Contains("USD"))
                    {
                        dolarsatisfyt = (fiyatlar[i].SelectSingleNode(".//small[contains(@class, 'value')]")).InnerText.Trim();
                    }
                    if (kurIsmi.Contains("EUR"))
                    {
                        eurosatisfyt = (fiyatlar[i].SelectSingleNode(".//small[contains(@class, 'value')]")).InnerText.Trim();
                    }
                    if (dolarsatisfyt != "" && eurosatisfyt != "")
                    {
                        break;
                    }
                }


                dt.Rows.Add(dolarsatisfyt, eurosatisfyt, DateTime.Now.ToString("HH:mm"));

                return JsonConvert.SerializeObject(dt);

            }
            catch (Exception)
            {
                dt.Rows.Add(0, 0, DateTime.Now.ToString("HH:mm"));
                return JsonConvert.SerializeObject(dt);
            }






        }



    }
}