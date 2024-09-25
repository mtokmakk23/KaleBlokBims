using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using pdf = iTextSharp.text;

namespace KaleBlokBims.Models.Classlar
{
    public class TeklifFormuOlustur
    {
        public byte[] teklifFormu(int LOGICALREF)
        {
            #region fields
            var db = new IZOKALEPORTALEntities();

            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 15f, 15f, 15f, 15f);
            MemoryStream memoryStream = new MemoryStream();
            BaseFont STF_Helvetica_Turkish = BaseFont.CreateFont("C:\\Windows\\Fonts\\Arial.ttf", "windows-1254", true);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            //PdfWriter writer=PdfWriter.GetInstance(document, new FileStream("C:/Deneme/deneme.pdf", FileMode.Create));
            //PdfWriter.GetInstance(document, new FileStream("C:\\Deneme\\CSharpPDF.pdf", FileMode.Create));
            PdfPCell cell;
            PdfPTable anaTablo = new PdfPTable(1);
            anaTablo.WidthPercentage = 99f;
            anaTablo.SetWidths(new float[] { 100f });
            #endregion


            #region utils
            var teklifBasligi = (from fb in db.TeklifBasliklari
                                 where fb.LOGICALREF == LOGICALREF
                                 select new
                                 {
                                     fb.FiyatListesi,
                                     fb.BayiKodu,
                                     fb.BayiAdi,
                                     fb.EklenmeTarihi,
                                     fb.Ilce,
                                     fb.TeklifSonGecerlilikTarihi,
                                     fb.Il,
                                     fb.FabrikaTeslimMi,
                                     fb.LOGICALREF,
                                     fb.MailAdresi,
                                     fb.OdemeTipi,
                                     fb.Aciklama
                                 }).FirstOrDefault();

            var teklifIcerigi = (from fb in db.TeklifBasliklari
                                 join fi in db.TeklifIcerikleri on fb.LOGICALREF equals fi.BaslikLREF
                                 where fi.BaslikLREF == LOGICALREF
                                 select new
                                 {
                                     fi.LOGICALREF,
                                     fi.IndirimAciklamasi,
                                     fi.IndiriminUygulanacagiLOGICALREF,
                                     fi.IndirimTutari,
                                     fi.LINETYPE,
                                     fi.GuncelEUR,
                                     fi.GuncelUSD,
                                     fi.Kdv,
                                     fi.HesaplamaDetayliAciklama,
                                     fi.FiyatListesi,
                                     fi.MalzemeKodu,
                                     fi.MalzemeAdi,
                                     fi.Miktar,
                                     fi.Birimi,
                                     fi.HesaplanmisBirimFiyatiTL,
                                     TOPLAM = "",

                                     fb.BayiKodu,
                                     fb.BayiAdi
                                 }).ToList();
            #endregion

            #region form adres ve tarih kısmı

            PdfPTable adresVeTarihTablo = new PdfPTable(3);
            adresVeTarihTablo.WidthPercentage = 99f;
            adresVeTarihTablo.SetWidths(new float[] { 45f, 22, 33f });

            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["Logo"].ToString()));
            img.ScaleToFit(100, 80);


            #region Satıcı Bilgileri
            PdfPTable saticiBil = new PdfPTable(1);
            saticiBil.WidthPercentage = 99f;

            cell = new PdfPCell(new Phrase(ConfigurationManager.AppSettings["Firma"].ToUpper(), new pdf.Font(STF_Helvetica_Turkish, 9, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = PdfPCell.NO_BORDER;
            saticiBil.AddCell(cell);

            cell = new PdfPCell(new Phrase(ConfigurationManager.AppSettings["FirmaAdres"].ToUpper(), new pdf.Font(STF_Helvetica_Turkish, 9, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = PdfPCell.NO_BORDER;
            saticiBil.AddCell(cell);

            cell = new PdfPCell(new Phrase(ConfigurationManager.AppSettings["FirmaIletisim"], new pdf.Font(STF_Helvetica_Turkish, 9, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = PdfPCell.NO_BORDER;
            saticiBil.AddCell(cell);
            #endregion

            #region alıcı bilgileri
            PdfPTable aliciBil = new PdfPTable(1);
            aliciBil.WidthPercentage = 100f;

            cell = new PdfPCell(new Phrase(teklifBasligi.BayiAdi, new pdf.Font(STF_Helvetica_Turkish, 9, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingTop = 10;
            cell.Border = PdfPCell.NO_BORDER;
            aliciBil.AddCell(cell);


            //Teslim Bilgileri
            cell = new PdfPCell(new Phrase("TESLİMAT ADRESİ:", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.ITALIC, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = PdfPCell.NO_BORDER;
            cell.PaddingTop = 10;
            aliciBil.AddCell(cell);

            cell = new PdfPCell(new Phrase((teklifBasligi.FabrikaTeslimMi == true ? "Fabrika Teslim" : teklifBasligi.Il + "/" + teklifBasligi.Ilce).ToUpper(), new pdf.Font(STF_Helvetica_Turkish, 9, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = PdfPCell.NO_BORDER;
            aliciBil.AddCell(cell);
            cell = new PdfPCell(new Phrase("", new pdf.Font(STF_Helvetica_Turkish, 9, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = PdfPCell.NO_BORDER;
            aliciBil.AddCell(cell);
            #endregion

            #region TeklifBilgileri
            PdfPTable teklifBil = new PdfPTable(2);
            teklifBil.WidthPercentage = 100f;
            teklifBil.SetWidths(new float[] { 30f, 70f });

            cell = new PdfPCell(new Phrase("TEKLİF BİLGİLERİ", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.BackgroundColor = cell.BackgroundColor = (new BaseColor(System.Drawing.ColorTranslator.FromHtml("#eee")));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            teklifBil.AddCell(cell);

            cell = new PdfPCell(new Phrase("Tarih:", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            teklifBil.AddCell(cell);
            cell = new PdfPCell(new Phrase(Convert.ToDateTime(teklifBasligi.EklenmeTarihi).ToString("dd.MM.yyyy"), new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            teklifBil.AddCell(cell);
            cell = new PdfPCell(new Phrase("Ref:", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            teklifBil.AddCell(cell);
            cell = new PdfPCell(new Phrase(teklifBasligi.BayiKodu + "-" + teklifBasligi.LOGICALREF, new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            teklifBil.AddCell(cell);
            cell = new PdfPCell(new Phrase("Opsiyon:", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            teklifBil.AddCell(cell);
            cell = new PdfPCell(new Phrase(Convert.ToDateTime(teklifBasligi.TeklifSonGecerlilikTarihi).ToString("dd.MM.yyyy"), new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            teklifBil.AddCell(cell);

            cell = new PdfPCell(new Phrase("Oluşturan:", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            teklifBil.AddCell(cell);
            cell = new PdfPCell(new Phrase(db.AdminKullanicilari.Where(x => x.MailAdresi == teklifBasligi.MailAdresi).FirstOrDefault().AdiSoyadi + " (Admin)", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            teklifBil.AddCell(cell);
            #endregion

            cell = new PdfPCell(saticiBil);
            cell.Border = PdfPCell.NO_BORDER;
            adresVeTarihTablo.AddCell(cell);

            cell = new PdfPCell(img);
            cell.Border = PdfPCell.NO_BORDER;
            //cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            adresVeTarihTablo.AddCell(cell);

            cell = new PdfPCell(teklifBil);
            cell.Border = PdfPCell.NO_BORDER;
            cell.Rowspan = 2;
            adresVeTarihTablo.AddCell(cell);

            cell = new PdfPCell(aliciBil);
            cell.Border = PdfPCell.NO_BORDER;
            adresVeTarihTablo.AddCell(cell);

            cell = new PdfPCell(new Phrase("TEKLİF FORMU", new pdf.Font(STF_Helvetica_Turkish, 12, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            adresVeTarihTablo.AddCell(cell);


            cell = new PdfPCell(adresVeTarihTablo);
            cell.Border = PdfPCell.NO_BORDER;
            anaTablo.AddCell(cell);
            #endregion

            cell = new PdfPCell(new Phrase("BOŞLUK", new pdf.Font(STF_Helvetica_Turkish, 10, pdf.Font.BOLD, BaseColor.WHITE)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.Padding = 10;
            anaTablo.AddCell(cell);


            #region from malzeme bilgileri

            PdfPTable malzemeBilgileri = new PdfPTable(6);
            malzemeBilgileri.WidthPercentage = 99f;
            malzemeBilgileri.SetWidths(new float[] { 10f, 40f, 10f, 10f, 10f, 20f });

            cell = new PdfPCell(new Phrase("Mlz.Kodu", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.BorderWidthBottom = 1;
            malzemeBilgileri.AddCell(cell);
            cell = new PdfPCell(new Phrase("Mlz.Adı", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.BorderWidthBottom = 1;
            malzemeBilgileri.AddCell(cell);
            cell = new PdfPCell(new Phrase("Miktar", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.BorderWidthBottom = 1;
            malzemeBilgileri.AddCell(cell);
            cell = new PdfPCell(new Phrase("Birim", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.BorderWidthBottom = 1;
            malzemeBilgileri.AddCell(cell);
            cell = new PdfPCell(new Phrase("Fiyat", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.BorderWidthBottom = 1;
            malzemeBilgileri.AddCell(cell);
            cell = new PdfPCell(new Phrase("Tutar", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.BorderWidthBottom = 1;
            malzemeBilgileri.AddCell(cell);
            double tutar;
            double toplamIndirim = 0;
            double toplamKdv = 0;
            double toplamTutar = 0;

            foreach (var item in teklifIcerigi.Where(x => x.LINETYPE == 0))
            {
                tutar = Math.Round(Convert.ToDouble(item.HesaplanmisBirimFiyatiTL * item.Miktar), 2);
                toplamTutar += tutar;
                toplamKdv += Convert.ToDouble(tutar * (item.Kdv / 100));
                cell = new PdfPCell(new Phrase(item.MalzemeKodu, new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
                cell.Border = PdfPCell.NO_BORDER;
                malzemeBilgileri.AddCell(cell);
                cell = new PdfPCell(new Phrase(item.MalzemeAdi, new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
                cell.Border = PdfPCell.NO_BORDER;
                malzemeBilgileri.AddCell(cell);
                cell = new PdfPCell(new Phrase(item.Miktar.ToString(), new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
                cell.Border = PdfPCell.NO_BORDER;
                malzemeBilgileri.AddCell(cell);
                cell = new PdfPCell(new Phrase(item.Birimi, new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
                cell.Border = PdfPCell.NO_BORDER;
                malzemeBilgileri.AddCell(cell);
                cell = new PdfPCell(new Phrase(Math.Round(Convert.ToDouble(item.HesaplanmisBirimFiyatiTL), 2) + " TL", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
                cell.Border = PdfPCell.NO_BORDER;
                malzemeBilgileri.AddCell(cell);
                cell = new PdfPCell(new Phrase(tutar + " TL", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
                cell.Border = PdfPCell.NO_BORDER;
                malzemeBilgileri.AddCell(cell);

                foreach (var item2 in teklifIcerigi.Where(x => x.IndiriminUygulanacagiLOGICALREF == item.LOGICALREF))
                {
                    toplamIndirim += Math.Round(Convert.ToDouble(item2.IndirimTutari) + (Convert.ToDouble(item2.IndirimTutari) * (Convert.ToDouble(item.Kdv) / 100)), 2);
                    cell = new PdfPCell(new Phrase("İndirim", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.ITALIC, BaseColor.BLACK)));
                    cell.Border = PdfPCell.NO_BORDER;
                    malzemeBilgileri.AddCell(cell);
                    cell = new PdfPCell(new Phrase(item2.IndirimAciklamasi, new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.ITALIC, BaseColor.BLACK)));
                    cell.Border = PdfPCell.NO_BORDER;
                    malzemeBilgileri.AddCell(cell);
                    cell = new PdfPCell(new Phrase("", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
                    cell.Border = PdfPCell.NO_BORDER;
                    malzemeBilgileri.AddCell(cell);
                    cell = new PdfPCell(new Phrase("", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
                    cell.Border = PdfPCell.NO_BORDER;
                    malzemeBilgileri.AddCell(cell);
                    cell = new PdfPCell(new Phrase(Math.Round(Convert.ToDouble(item2.IndirimTutari), 2) + " TL", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.ITALIC, BaseColor.BLACK)));
                    cell.Border = PdfPCell.NO_BORDER;
                    malzemeBilgileri.AddCell(cell);
                    cell = new PdfPCell(new Phrase(Math.Round(Convert.ToDouble(item2.IndirimTutari), 2) + " TL", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.ITALIC, BaseColor.BLACK)));
                    cell.Border = PdfPCell.NO_BORDER;
                    malzemeBilgileri.AddCell(cell);
                }
            }


            cell = new PdfPCell(new Phrase("BOŞLUK", new pdf.Font(STF_Helvetica_Turkish, 10, pdf.Font.BOLD, BaseColor.WHITE)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.Padding = 10;
            cell.Colspan = 6;
            malzemeBilgileri.AddCell(cell);

            cell = new PdfPCell(new Phrase("Toplam Tutar:", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 5;
            malzemeBilgileri.AddCell(cell);

            cell = new PdfPCell(new Phrase(Math.Round(toplamTutar, 2).ToString() + " TL", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            malzemeBilgileri.AddCell(cell);


            cell = new PdfPCell(new Phrase("Toplam İndirim:", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 5;
            malzemeBilgileri.AddCell(cell);

            cell = new PdfPCell(new Phrase(Math.Round(toplamIndirim, 2).ToString() + " TL", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            malzemeBilgileri.AddCell(cell);

            cell = new PdfPCell(new Phrase("Toplam Kdv:", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 5;
            malzemeBilgileri.AddCell(cell);

            cell = new PdfPCell(new Phrase(Math.Round(toplamKdv, 2).ToString() + " TL", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            malzemeBilgileri.AddCell(cell);



            cell = new PdfPCell(new Phrase("Yalnız:" + yaziyaCevir(Convert.ToDecimal(Math.Round((toplamKdv + toplamTutar - toplamIndirim), 2))), new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 3;
            malzemeBilgileri.AddCell(cell);

            cell = new PdfPCell(new Phrase("Genel Toplam:", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Colspan = 2;
            malzemeBilgileri.AddCell(cell);

            cell = new PdfPCell(new Phrase(Math.Round((toplamKdv + toplamTutar - toplamIndirim), 2).ToString() + " TL", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.BOLD, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            malzemeBilgileri.AddCell(cell);


            cell = new PdfPCell(malzemeBilgileri);
            cell.Border = PdfPCell.NO_BORDER;
            anaTablo.AddCell(cell);
            #endregion



            #region sipariş detayları
            PdfPTable detayBilgileri = new PdfPTable(1);
            detayBilgileri.WidthPercentage = 99f;
            cell = new PdfPCell(new Phrase("Ödeme Tipi: " + teklifBasligi.OdemeTipi, new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            detayBilgileri.AddCell(cell);

            cell = new PdfPCell(new Phrase(" ", new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            detayBilgileri.AddCell(cell);

            cell = new PdfPCell(new Phrase("Açıklama: " + teklifBasligi.Aciklama, new pdf.Font(STF_Helvetica_Turkish, 8, pdf.Font.NORMAL, BaseColor.BLACK)));
            cell.Border = PdfPCell.NO_BORDER;
            detayBilgileri.AddCell(cell);


            cell = new PdfPCell(detayBilgileri);
            cell.Border = PdfPCell.NO_BORDER;
            anaTablo.AddCell(cell);
            #endregion


            #region Pdf Oluştur
            document.Open();
            document.Add(anaTablo);
            document.Close();
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            return bytes;
            #endregion

        }




        public string yaziyaCevir(decimal tutar)
        {
            string sTutar = tutar.ToString("F2").Replace('.', ','); // Replace('.',',') ondalık ayracının . olma durumu için            
            string lira = sTutar.Substring(0, sTutar.IndexOf(',')); //tutarın tam kısmı
            string kurus = sTutar.Substring(sTutar.IndexOf(',') + 1, 2);
            string yazi = "";

            string[] birler = { "", "BİR", "İKİ", "ÜÇ", "DÖRT", "BEŞ", "ALTI", "YEDİ", "SEKİZ", "DOKUZ" };
            string[] onlar = { "", "ON", "YİRMİ", "OTUZ", "KIRK", "ELLİ", "ALTMIŞ", "YETMİŞ", "SEKSEN", "DOKSAN" };
            string[] binler = { "KATRİLYON", "TRİLYON", "MİLYAR", "MİLYON", "BİN", "" }; //KATRİLYON'un önüne ekleme yapılarak artırabilir.

            int grupSayisi = 6; //sayıdaki 3'lü grup sayısı. katrilyon içi 6. (1.234,00 daki grup sayısı 2'dir.)
                                //KATRİLYON'un başına ekleyeceğiniz her değer için grup sayısını artırınız.

            lira = lira.PadLeft(grupSayisi * 3, '0'); //sayının soluna '0' eklenerek sayı 'grup sayısı x 3' basakmaklı yapılıyor.            

            string grupDegeri;

            for (int i = 0; i < grupSayisi * 3; i += 3) //sayı 3'erli gruplar halinde ele alınıyor.
            {
                grupDegeri = "";

                if (lira.Substring(i, 1) != "0")
                    grupDegeri += birler[Convert.ToInt32(lira.Substring(i, 1))] + "YÜZ"; //yüzler                

                if (grupDegeri == "BİRYÜZ") //biryüz düzeltiliyor.
                    grupDegeri = "YÜZ";

                grupDegeri += onlar[Convert.ToInt32(lira.Substring(i + 1, 1))]; //onlar

                grupDegeri += birler[Convert.ToInt32(lira.Substring(i + 2, 1))]; //birler                

                if (grupDegeri != "") //binler
                    grupDegeri += " " + binler[i / 3] + " ";

                if (grupDegeri == "BİRBİN") //birbin düzeltiliyor.
                    grupDegeri = "BİN";

                yazi += grupDegeri;
            }

            if (yazi != "")
                yazi += "TL ";

            int yaziUzunlugu = yazi.Length;

            if (kurus.Substring(0, 1) != "0") //kuruş onlar
                yazi += onlar[Convert.ToInt32(kurus.Substring(0, 1))];

            if (kurus.Substring(1, 1) != "0") //kuruş birler
                yazi += birler[Convert.ToInt32(kurus.Substring(1, 1))];

            if (yazi.Length > yaziUzunlugu)
                yazi += " KR";
            else
                yazi += "SIFIR KR";

            return yazi;
        }

        private static bool EmailKontrol(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }


        }
    }
}