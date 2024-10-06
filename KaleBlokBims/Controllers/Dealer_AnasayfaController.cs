using HtmlAgilityPack;
using KaleBlokBims.Models.Classlar;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
            if (Session["AdminMi"].ToString() == "0")
            {
                var db = new Models.IZOKALEPORTALEntities();
                var mailAdresi = Session["MailAdresi"].ToString();
                db.BayiKullanicilari.Where(x => x.MailAdresi == mailAdresi).FirstOrDefault().GeciciSifre = "";
            }

            return View();
        }
        [HttpPost]
        public string Duyurular()
        {
            var db = new Models.IZOKALEPORTALEntities();
            var query = db.Duyurular.Where(x => x.BayiDuyurusuMu == true && x.BaslangicTarihi < DateTime.Now && x.BitisTarihi > DateTime.Now);
            return JsonConvert.SerializeObject(query);
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
        public string PaletHesapla()
        {
            var servis = new M2BWebService.ZOKALEAPISoapClient();
            RestResponse response = new RestResponse();
            var db = new Models.IZOKALEPORTALEntities();
            var mailAdresi = Session["MailAdresi"].ToString();
            var BayiKodu = Session["BayiKodu"].ToString();
            try
            {
                var paletBilgileri = servis.PaletFiyatiniAl();
                if (paletBilgileri == null)
                {
                    response.IsSuccessStatusCode = false;
                    response.ErrorMessage = "Palet Fiyatı Alınamadı.";
                    return JsonConvert.SerializeObject(response);
                }
                var baslik = db.SiparisBasliklari.Where(x => x.MailAdresi == mailAdresi && x.BayiKodu == BayiKodu && x.OnaylandiMi == false && x.SilindiMi != true).FirstOrDefault();
                if (baslik == null)
                {
                    response.IsSuccessStatusCode = false;
                    response.ErrorMessage = "Açık Siparişiniz Bulunmuyor";
                    return JsonConvert.SerializeObject(response);
                }

                var malzemeler = db.SiparisIcerikleri.Where(x => x.BaslikLREF == baslik.LOGICALREF).ToList();
                int toplamPalet = 0;
                double varOlanPaletMiktari = 0;
                int index = 1;
                foreach (var item in malzemeler)
                {
                    if (item.MalzemeKodu == paletBilgileri.Rows[0]["CODE"].ToString())
                    {
                        varOlanPaletMiktari += Convert.ToDouble(item.Miktar);
                    }
                    else
                    {
                        if (item.Miktar % item.PALET_KT == 0)
                        {
                            toplamPalet += Convert.ToInt32(item.Miktar / item.PALET_KT);
                        }
                        else
                        {
                            response.IsSuccessStatusCode = false;
                            response.ErrorMessage =index+ ". Satırdaki Malzemenin Miktarı "+item.PALET_KT+" "+item.Birimi+" veya Katları Olmalıdır.";
                            return JsonConvert.SerializeObject(response);
                        }

                    }
                    index++;
                }
                if (varOlanPaletMiktari>0)
                {
                    db.Database.ExecuteSqlCommand("delete from SiparisIcerikleri where BaslikLREF="+ baslik.LOGICALREF + " AND MalzemeKodu='"+ paletBilgileri.Rows[0]["CODE"].ToString() + "'");
                    db.SaveChanges();
                }

                var icerik = new Models.SiparisIcerikleri();
                icerik.AltGrup = "";
                icerik.AnaGrup = "";
                icerik.FiyatListesi = baslik.FiyatListesi;
                icerik.LINETYPE = 0;
                icerik.BaseDoviz = "TL";
                icerik.BaseFiyat = Convert.ToDouble(paletBilgileri.Rows[0]["PRICE"].ToString().Replace(".", ","));
                icerik.PALET_KT = 1;
                icerik.BaslikLREF = baslik.LOGICALREF;
                icerik.Birimi = paletBilgileri.Rows[0]["UNIT"].ToString();
                icerik.EklenmeTarihi = DateTime.Now;
                icerik.GuncelEUR = 1;
                icerik.GuncelUSD = 1;
                icerik.HesaplamaDetayliAciklama = "";
                icerik.HesaplanmisBirimFiyatiTL = Convert.ToDouble(paletBilgileri.Rows[0]["PRICE"].ToString().Replace(".", ","));
                icerik.MalzemeAdi = paletBilgileri.Rows[0]["NAME"].ToString();
                icerik.MalzemeKodu = paletBilgileri.Rows[0]["CODE"].ToString();
                icerik.Miktar = Convert.ToDouble(toplamPalet.ToString().Replace(".", ","));
                icerik.NakliyeFiyatiTL =0;
                icerik.SabitEUR =1;
                icerik.SabitUSD = 1;
                icerik.Kdv = Convert.ToDouble(paletBilgileri.Rows[0]["vat"].ToString().Replace(".", ","));
                icerik.Editable = false;
                icerik.SistemKalemiMi = false;
                db.SiparisIcerikleri.Add(icerik);
                db.SaveChanges();
                response.IsSuccessStatusCode = true;
                response.Content = "Palet Eklendi. Sepet Fiyatını Kontrol Ediniz.";
                return JsonConvert.SerializeObject(response);
            }
            catch (Exception ex)
            {
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = ex.Message;
                return JsonConvert.SerializeObject(response);
            }
        }

        [HttpPost]
        public string SepetiOnayla(string sepetOnaylaOdemeTipi, string sepetOnaylaSiparisNotu)
        {
            RestResponse response = new RestResponse();
            try
            {

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
                    if (baslik.BaglantiLref != -1)
                    {
                        var servis = new M2BWebService.ZOKALEAPISoapClient();
                        var liste = servis.SecilebilirFiyatListeleri(baslik.BayiKodu).Where(x => x.baglantiLREF.ToString() == baslik.BaglantiLref.ToString()).FirstOrDefault();
                        var malzemeler = db.SiparisIcerikleri.Where(x => x.BaslikLREF == baslik.LOGICALREF).ToList();
                        double toplamTutar = 0;
                        foreach (var item in malzemeler)
                        {
                            if (item.LINETYPE == 0)
                            {
                                toplamTutar += Convert.ToDouble(item.HesaplanmisBirimFiyatiTL) * Convert.ToDouble(item.Miktar) * (Convert.ToDouble(100 + item.Kdv) / 100);
                            }
                            if (item.LINETYPE == 2)
                            {
                                toplamTutar -= Convert.ToDouble(item.IndirimTutari);

                            }
                        }

                        if (Convert.ToDouble(liste.bakiye) < toplamTutar)
                        {
                            response.IsSuccessStatusCode = false;
                            response.ErrorMessage = "En Fazla " + liste.bakiye + " TL Tutarında Sipariş Girebilirsiniz.";
                            return JsonConvert.SerializeObject(response);
                        }
                    }
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
                    System.IO.File.WriteAllBytes(Server.MapPath("/Files/SiparisFormlari/" + baslik.BayiAdi + " - " + baslik.LOGICALREF + ".pdf"), pdfByte);

                    MailGonderme mail = new MailGonderme();
                    mail.EkliMailGonderme("", SabitTanimlar.SiparisFormuGonderilecekMailler(), baslik.MailAdresi + "," + firmaAdmini, baslik.BayiKodu + "-" + baslik.LOGICALREF + " - Sipariş Formu", baslik.BayiAdi + " Tarafından oluşturulan " + baslik.BayiKodu + "-" + baslik.LOGICALREF + " referans numaralı sipariş formu ekte yer almaktadır.", pdfByte, baslik.BayiKodu + "-" + baslik.LOGICALREF + ".pdf");


                    response.IsSuccessStatusCode = true;
                    String file = Convert.ToBase64String(pdfByte);
                    response.Content = file;
                }
                return JsonConvert.SerializeObject(response);
            }
            catch (Exception hata)
            {
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = hata.Message;
                return hata.Message;
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
                var baslik = db.SiparisBasliklari.Where(x => x.MailAdresi == mailAdresi && x.BayiKodu == BayiKodu && x.OnaylandiMi == false && x.SilindiMi != true).ToList();
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
                //%4 ileve
                dolarsatisfyt = (Math.Round(Convert.ToDouble(dolarsatisfyt) * (1.04), 4)).ToString();
                eurosatisfyt = (Math.Round(Convert.ToDouble(eurosatisfyt) * (1.04), 4)).ToString();
                //
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