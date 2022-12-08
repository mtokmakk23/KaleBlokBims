using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace KaleBlokBims.Models.Classlar
{
    public class SiparisFormuOlustur
    {
        //public byte[] siparisFormu(int LOGICALREF, bool AdminMi)
        //{
        //    try
        //    {
        //        var db = new Models.IZOKALEPORTALEntities();
        //        var fisIcerikleri = db.SiparisIcerikleri.Where(x=>x.BaslikLREF==LOGICALREF).ToList();
        //        iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 15f, 15f, 15f, 15f);
        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            BaseFont STF_Helvetica_Turkish = BaseFont.CreateFont("C:\\Windows\\Fonts\\Trebuc.ttf", "windows-1254", true);

        //            //BaseFont STF_Helvetica_Turkish = BaseFont.CreateFont("Helvetica", "CP1254", BaseFont.NOT_EMBEDDED);
        //            Font icerik = new Font(STF_Helvetica_Turkish, 8, Font.NORMAL, BaseColor.BLACK);
        //            Font kolonBasligi = new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK);
        //            Font adresDetayi = new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK);
        //            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        //            PdfPCell cell;

        //            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("/images/Logolar/MEGA_Slider.png"));
        //            img.ScaleToFit(100, 80);

        //            /***********************************************************************/

        //            #region 
        //            //fiş içerikleri
        //            PdfPTable table = new PdfPTable(4);
        //            table.WidthPercentage = 99f;
        //            float[] tablewidths = new float[] { 10f, 55f, 20f, 15f };
        //            table.SetWidths(tablewidths);


        //            //fiyat listesi ve güncel döviz tablosu
        //            PdfPTable fiyatDoviz = new PdfPTable(2);
        //            fiyatDoviz.WidthPercentage = 99f;
        //            float[] fiyatDovizWidth = new float[] { 20f, 80f };
        //            fiyatDoviz.SetWidths(fiyatDovizWidth);

        //            double dipToplam = 0;
        //            double KdvToplam = 0;
        //            foreach (var item in fisIcerikleri)
        //            {
        //                if (item.LINETYPE == 0)
        //                {
        //                    cell = new PdfPCell(new Phrase("FİYAT LİSTESİ: ", new Font(STF_Helvetica_Turkish, 8, Font.BOLD, BaseColor.BLACK)));
        //                    cell.BorderWidthLeft = 0;
        //                    cell.BorderWidthRight = 0;
        //                    cell.BorderWidthTop = 0;
        //                    cell.BorderWidthBottom = 0;
        //                    fiyatDoviz.AddCell(cell);

        //                    cell = new PdfPCell(new Phrase(item.FiyatListesi, new Font(STF_Helvetica_Turkish, 9, Font.NORMAL, BaseColor.BLACK)));
        //                    cell.BorderWidthLeft = 0;
        //                    cell.BorderWidthRight = 0;
        //                    cell.BorderWidthTop = 0;
        //                    cell.BorderWidthBottom = 0;
        //                    fiyatDoviz.AddCell(cell);

                           
        //                        cell = new PdfPCell(new Phrase("GÜNCEL DÖVİZ BİLGİLERİ: ", new Font(STF_Helvetica_Turkish, 8, Font.BOLD, BaseColor.BLACK)));
        //                        cell.BorderWidthLeft = 0;
        //                        cell.BorderWidthRight = 0;
        //                        cell.BorderWidthTop = 0;
        //                        cell.BorderWidthBottom = 0;
        //                        fiyatDoviz.AddCell(cell);

        //                        cell = new PdfPCell(new Phrase("USD:" + item.GuncelUSD + " EUR:" + item.GuncelEUR, new Font(STF_Helvetica_Turkish, 9, Font.NORMAL, BaseColor.BLACK)));
        //                        cell.BorderWidthLeft = 0;
        //                        cell.BorderWidthRight = 0;
        //                        cell.BorderWidthTop = 0;
        //                        cell.BorderWidthBottom = 0;
        //                        fiyatDoviz.AddCell(cell);
                            

        //                    break;
        //                }
        //            }
        //            cell = new PdfPCell(fiyatDoviz);
        //            cell.Border = PdfPCell.NO_BORDER;
        //            cell.Colspan = 4;
        //            table.AddCell(cell);


        //            cell = new PdfPCell(new Phrase("Poz.", kolonBasligi));
        //            cell.BorderWidthLeft = 0;
        //            cell.BorderWidthRight = 0;
        //            cell.BorderWidthTop = 0;
        //            table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase("Malzeme    Açıklama", kolonBasligi));
        //            cell.BorderWidthLeft = 0;
        //            cell.BorderWidthRight = 0;
        //            cell.BorderWidthTop = 0;
        //            table.AddCell(cell);
                    
        //            cell = new PdfPCell(new Phrase("", kolonBasligi));
        //            cell.BorderWidthLeft = 0;
        //            cell.BorderWidthRight = 0;
        //            cell.BorderWidthTop = 0;
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            table.AddCell(cell);



        //            cell = new PdfPCell(new Phrase("Tutar", kolonBasligi));
        //            cell.BorderWidthLeft = 0;
        //            cell.BorderWidthRight = 0;
        //            cell.BorderWidthTop = 0;
        //            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            table.AddCell(cell);


        //            int sayac = 1;
        //            foreach (var item in fisIcerikleri)
        //            {
        //                if (item.LINETYPE == 0)
        //                {


        //                    cell = new PdfPCell(new Phrase("" + sayac, icerik));
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    cell.Rowspan = 2;
        //                    table.AddCell(cell);
        //                    sayac++;

        //                    cell = new PdfPCell(new Phrase(item.MalzemeKodu + "    " + item.MalzemeAdi, icerik));
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    table.AddCell(cell);


        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                           
        //                    cell.Rowspan = 4;
        //                    table.AddCell(cell);

        //                    cell = new PdfPCell(new Phrase(string.Format("{0:N2}", Math.Round((Convert.ToDouble(item.HesaplanmisBirimFiyatiTL) * Convert.ToDouble(item.Miktar)), 2)) + " TL", icerik));
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //                    cell.Rowspan = 2;
        //                    table.AddCell(cell);


        //                    cell = new PdfPCell(new Phrase(string.Format("{0:N4}", item.Miktar) + "    " + item.Birimi + "            " + (string.Format("{0:N4}", item.HesaplanmisBirimFiyatiTL)) + "TL", icerik));
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    table.AddCell(cell);

        //                    bool indirimVarMi = false;
        //                    foreach (var item2 in fisIcerikleri)
        //                    {
        //                        if (item2.IndiriminUygulanacagiLOGICALREF == item.LOGICALREF)
        //                        {
        //                            indirimVarMi = true;

        //                            cell = new PdfPCell(new Phrase(" ", icerik));
        //                            cell.Border = PdfPCell.NO_BORDER;
        //                            table.AddCell(cell);


        //                            cell = new PdfPCell(new Phrase("" + item2.IndirimAciklamasi + "                        %" + string.Format("{0:N3}", Math.Abs((Convert.ToDouble(item2.IndirimTutari) * Convert.ToDouble(100)) / (Convert.ToDouble(item.Miktar) * Convert.ToDouble(item.HesaplanmisBirimFiyatiTL)))) + "", new Font(STF_Helvetica_Turkish, 8, Font.ITALIC, BaseColor.BLACK)));
        //                            cell.Border = PdfPCell.NO_BORDER;
        //                            table.AddCell(cell);





        //                            cell = new PdfPCell(new Phrase("" + string.Format("{0:N2}", Math.Round(Convert.ToDouble(item2.IndirimTutari), 2)) + "TL", new Font(STF_Helvetica_Turkish, 8, Font.ITALIC, BaseColor.BLACK)));
        //                            cell.Border = PdfPCell.NO_BORDER;
        //                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //                            table.AddCell(cell);
        //                        }

        //                    }



        //                }



        //            }
        //            foreach (var item in fisIcerikleri)
        //            {

        //                if (item.LINETYPE == 0)
        //                {
        //                    dipToplam += Math.Round(Convert.ToDouble(item.HesaplanmisBirimFiyatiTL) * Convert.ToDouble(item.Miktar), 2);
        //                    KdvToplam += Math.Round(Convert.ToDouble((item.HesaplanmisBirimFiyatiTL * item.Miktar * item.Kdv) / 100), 2);
        //                }
        //                if (item.LINETYPE == 2 || item.LINETYPE == 22 || item.LINETYPE == 3)
        //                {
        //                    dipToplam += Math.Round(Convert.ToDouble(item.IndirimTutari), 2);
        //                    KdvToplam += Math.Round(Convert.ToDouble((item.IndirimTutari * item.Kdv) / 100), 2);

        //                }
        //            }

                   

        //            cell = new PdfPCell(new Phrase("Toplam:", kolonBasligi));
        //            //cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ff7a87"));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            cell.Colspan = 2;
        //            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase(string.Format("{0:N2}", Math.Round((dipToplam), 2)) + " TL", icerik));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            table.AddCell(cell);


        //            //cell = new PdfPCell(new Phrase(" ", icerik));
        //            //cell.Border = PdfPCell.NO_BORDER;
        //            //cell.Colspan = dtFisIcerikleri.Columns.Count - 3;
        //            //table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" ", icerik));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase("Kdv:", kolonBasligi));
        //            //cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ff7a87"));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            cell.Colspan = 2;
        //            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase(string.Format("{0:N2}", Math.Round(KdvToplam, 2)) + " TL", icerik));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            table.AddCell(cell);


        //            //cell = new PdfPCell(new Phrase(" ", icerik));
        //            //cell.Border = PdfPCell.NO_BORDER;
        //            //cell.Colspan = dtFisIcerikleri.Columns.Count - 3;
        //            //table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase(" ", icerik));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase("Genel Toplam:", kolonBasligi));
        //            //cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ff7a87"));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            cell.Colspan = 2;
        //            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase(string.Format("{0:N2}", (Math.Round(dipToplam + KdvToplam, 2))) + " TL", icerik));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            table.AddCell(cell);


                  


        //            cell = new PdfPCell(new Phrase(yaziyaCevir(Convert.ToDecimal(Math.Round(dipToplam + KdvToplam, 2))), new Font(STF_Helvetica_Turkish, 9, Font.BOLD, BaseColor.BLACK)));
        //            cell.Colspan = 4;
        //            cell.Border = PdfPCell.NO_BORDER;
        //            table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase("Ödeme Tipi:", icerik));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            //cell.Colspan = 3;
        //            table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase("" + OdemeTipi, icerik));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            cell.Colspan = 3;

        //            table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase(" ", icerik));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            //cell.Colspan = 3;
        //            table.AddCell(cell);


        //            cell = new PdfPCell(new Phrase("" + OdemeNotu, icerik));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            cell.Colspan = 3;
        //            table.AddCell(cell);

        //            //cell = new PdfPCell(new Phrase(OdemeTipi, icerik));
        //            //cell.Colspan = dtFisIcerikleri.Columns.Count;
        //            //cell.BorderWidthLeft = 0;
        //            //cell.BorderWidthRight = 0;
        //            //cell.BorderWidthTop = 0;
        //            //table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase(" ", icerik));
        //            cell.Border = PdfPCell.NO_BORDER;
        //            //cell.Colspan = 3;
        //            table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase(SiparisNotu, icerik));
        //            cell.Colspan = 3;
        //            cell.Border = PdfPCell.NO_BORDER;
        //            table.AddCell(cell);

        //            #endregion


        //            /***********************************************************************/



        //            /**************************************************************************/

        //            //satıcı firma bilgileri
        //            PdfPTable saticiBil = new PdfPTable(1);
        //            saticiBil.WidthPercentage = 99f;

        //            cell = new PdfPCell(new Phrase("MEGA YALITIM ÜRETİM PAZARLAMA VE DIŞ TİC.AŞ.", new Font(STF_Helvetica_Turkish, 9, Font.BOLD, BaseColor.BLACK)));
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.Border = PdfPCell.NO_BORDER;
        //            saticiBil.AddCell(cell);

        //            cell = new PdfPCell(new Phrase("ORGANİZE SANAYİ BÖL. 2.SK NO:15/2 ELAZIĞ".ToUpper(), new Font(STF_Helvetica_Turkish, 9, Font.NORMAL, BaseColor.BLACK)));
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.Border = PdfPCell.NO_BORDER;
        //            saticiBil.AddCell(cell);

        //            cell = new PdfPCell(new Phrase("(424) 255 14 44 - info@megainsulation.com.tr", new Font(STF_Helvetica_Turkish, 9, Font.NORMAL, BaseColor.BLACK)));
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.Border = PdfPCell.NO_BORDER;
        //            saticiBil.AddCell(cell);





        //            PdfPTable aliciBil = new PdfPTable(1);
        //            aliciBil.WidthPercentage = 100f;
        //            //float[] aliciBilwidths = new float[] { 13.5f, 36.5f, 13.5f, 36.5f };
        //            //aliciBil.SetWidths(aliciBilwidths);
        //            cell = new PdfPCell(new Phrase("", new Font(STF_Helvetica_Turkish, 8, Font.ITALIC, BaseColor.BLACK)));

        //            //cell = new PdfPCell(new Phrase("FATURA ADRESİ:", new Font(STF_Helvetica_Turkish, 10, Font.ITALIC, BaseColor.WHITE)));
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.PaddingTop = 3;
        //            cell.Border = PdfPCell.NO_BORDER;
        //            //cell.BackgroundColor = cell.BackgroundColor = (new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ff7a87")));                            
        //            aliciBil.AddCell(cell);


        //            cell = new PdfPCell(new Phrase(bayiAdi, new Font(STF_Helvetica_Turkish, 9, Font.BOLD, BaseColor.BLACK)));
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.Border = PdfPCell.NO_BORDER;
        //            aliciBil.AddCell(cell);

        //            cell = new PdfPCell(new Phrase("" + " " + "" + "" + "", new Font(STF_Helvetica_Turkish, 9, Font.NORMAL, BaseColor.BLACK)));
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.Border = PdfPCell.NO_BORDER;
        //            aliciBil.AddCell(cell);

        //            cell = new PdfPCell(new Phrase("" + " - " + "", new Font(STF_Helvetica_Turkish, 9, Font.NORMAL, BaseColor.BLACK)));
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.Border = PdfPCell.NO_BORDER;
        //            aliciBil.AddCell(cell);


        //            cell = new PdfPCell(new Phrase("TESLİMAT ADRESİ:", new Font(STF_Helvetica_Turkish, 8, Font.ITALIC, BaseColor.BLACK)));
        //            // cell = new PdfPCell(new Phrase("TESLİMAT ADRESİ:", new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.WHITE)));
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.Border = PdfPCell.NO_BORDER;
        //            cell.PaddingTop = 10;
        //            //cell.BackgroundColor = cell.BackgroundColor = (new BaseColor(System.Drawing.ColorTranslator.FromHtml("#ff7a87")));                           
        //            aliciBil.AddCell(cell);

        //            foreach (var item in projeVeAdresBilgileri)
        //            {

        //                cell = new PdfPCell(new Phrase(item.AdresBasligi, new Font(STF_Helvetica_Turkish, 9, Font.BOLD, BaseColor.BLACK)));
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                aliciBil.AddCell(cell);



        //                #region
        //                if (item.TeslimSekli == "ELAZIĞ FABRİKA TESLİMİ (MÜŞTERİ ARACI)")
        //                {


        //                    //cell = new PdfPCell(new Phrase("FABRİKA TESLİMİ(MÜŞTERİ ARACI)", new Font(STF_Helvetica_Turkish, 9, Font.BOLD, BaseColor.BLACK)));
        //                    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    //cell.Border = PdfPCell.NO_BORDER;
        //                    //aliciBil.AddCell(cell);

        //                    cell = new PdfPCell(new Phrase(item.Ilce + " / " + item.Sehir, new Font(STF_Helvetica_Turkish, 9, Font.NORMAL, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    aliciBil.AddCell(cell);

        //                    cell = new PdfPCell(new Phrase(item.SantiyedeTeslimAlacakKisininAdi + " - " + item.SantiyedeTeslimAlacakKisininTelefonu, new Font(STF_Helvetica_Turkish, 9, Font.NORMAL, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    aliciBil.AddCell(cell);




        //                }
        //                #endregion




        //                #region
        //                if (item.TeslimSekli == "ADRESE TESLİM (NAKLİYE DAHİL)")
        //                {


        //                    //cell = new PdfPCell(new Phrase("ADRESE TESLİM (NAKLİYE DAHİL)", new Font(STF_Helvetica_Turkish, 9, Font.BOLD, BaseColor.BLACK)));
        //                    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    //cell.Border = PdfPCell.NO_BORDER;
        //                    //aliciBil.AddCell(cell);

        //                    cell = new PdfPCell(new Phrase(item.SevkiyatAdresi + " " + item.Ilce + "/" + item.Sehir, new Font(STF_Helvetica_Turkish, 9, Font.NORMAL, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    aliciBil.AddCell(cell);

        //                    cell = new PdfPCell(new Phrase(item.SantiyedeTeslimAlacakKisininAdi + " - " + item.SantiyedeTeslimAlacakKisininTelefonu, new Font(STF_Helvetica_Turkish, 9, Font.NORMAL, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    aliciBil.AddCell(cell);





        //                }
        //                #endregion





        //                #region
        //                if (item.TeslimSekli == "ihracat")
        //                {

        //                    cell = new PdfPCell(new Phrase("Teslimat:", new Font(STF_Helvetica_Turkish, 9, Font.ITALIC, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    aliciBil.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase("İHRACAT", new Font(STF_Helvetica_Turkish, 9, Font.BOLD, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    cell.Colspan = 3;
        //                    aliciBil.AddCell(cell);

        //                    cell = new PdfPCell(new Phrase("İlgili Kişi:", new Font(STF_Helvetica_Turkish, 9, Font.ITALIC, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    aliciBil.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase(item.SantiyedeTeslimAlacakKisininAdi, new Font(STF_Helvetica_Turkish, 9, Font.BOLD, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    aliciBil.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase("Telefon:", new Font(STF_Helvetica_Turkish, 9, Font.ITALIC, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    aliciBil.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase(item.SantiyedeTeslimAlacakKisininTelefonu, new Font(STF_Helvetica_Turkish, 9, Font.BOLD, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    aliciBil.AddCell(cell);

        //                    cell = new PdfPCell(new Phrase("Ülke:", new Font(STF_Helvetica_Turkish, 9, Font.ITALIC, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    aliciBil.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase(item.Ulke, new Font(STF_Helvetica_Turkish, 9, Font.BOLD, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    cell.Colspan = 3;
        //                    aliciBil.AddCell(cell);


        //                    cell = new PdfPCell(new Phrase("Adres:", new Font(STF_Helvetica_Turkish, 9, Font.ITALIC, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    aliciBil.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase(item.SevkiyatAdresi, new Font(STF_Helvetica_Turkish, 9, Font.BOLD, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    cell.Colspan = 3;
        //                    aliciBil.AddCell(cell);

        //                }
        //                #endregion

        //            }


        //            //cell = new PdfPCell(saticiBil);
        //            //cell.BorderColor = BaseColor.GRAY;
        //            //tableLogo.AddCell(cell);

        //            PdfPTable tableLogo = new PdfPTable(3);
        //            tableLogo.WidthPercentage = 100f;
        //            tableLogo.HorizontalAlignment = 2;
        //            float[] tableLogowidths = new float[] { 40f, 30f, 30f };
        //            tableLogo.SetWidths(tableLogowidths);

        //            cell = new PdfPCell(saticiBil);
        //            cell.Border = PdfPCell.NO_BORDER;
        //            //cell.Rowspan = 2;
        //            tableLogo.AddCell(cell);

        //            cell = new PdfPCell(img);
        //            cell.Border = PdfPCell.NO_BORDER;
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;

        //            //cell.PaddingBottom = 3;
        //            tableLogo.AddCell(cell);



        //            foreach (var item in projeVeAdresBilgileri)
        //            {
        //                PdfPTable tabloTarih = new PdfPTable(2);
        //                tabloTarih.WidthPercentage = 100f;



        //                cell = new PdfPCell(new Phrase("MEGA SİPARİŞ FORMU", new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
        //                // cell = new PdfPCell(new Phrase("TESLİMAT ADRESİ:", new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.WHITE)));
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                cell.Colspan = 2;
        //                cell.BackgroundColor = cell.BackgroundColor = (new BaseColor(System.Drawing.ColorTranslator.FromHtml("#eee")));
        //                tabloTarih.AddCell(cell);

        //                cell = new PdfPCell(new Phrase("Referans No:", new Font(STF_Helvetica_Turkish, 10, Font.ITALIC, BaseColor.BLACK)));
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                tabloTarih.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(item.BayiKodu + "/" + item.LOGICALREF, new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                tabloTarih.AddCell(cell);

        //                cell = new PdfPCell(new Phrase("Tarih:", new Font(STF_Helvetica_Turkish, 10, Font.ITALIC, BaseColor.BLACK)));
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                tabloTarih.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("" + Convert.ToDateTime(item.EklenmeTarihi).ToString("dd.MM.yyyy"), new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                tabloTarih.AddCell(cell);

        //                cell = new PdfPCell(new Phrase("Proje Kodu:", new Font(STF_Helvetica_Turkish, 10, Font.ITALIC, BaseColor.BLACK)));
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                tabloTarih.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("" + item.ProjeAdi, new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                tabloTarih.AddCell(cell);

        //                cell = new PdfPCell(new Phrase("Yükleme Merkezi:", new Font(STF_Helvetica_Turkish, 10, Font.ITALIC, BaseColor.BLACK)));
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                tabloTarih.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("" + item.YuklemeMerkezi, new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                tabloTarih.AddCell(cell);


        //                cell = new PdfPCell(new Phrase("Teslim Şekli:", new Font(STF_Helvetica_Turkish, 10, Font.ITALIC, BaseColor.BLACK)));
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                tabloTarih.AddCell(cell);
        //                cell = new PdfPCell(new Phrase("" + item.TeslimSekli, new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                tabloTarih.AddCell(cell);

        //                if (item.FisiOlusturanAdmin != " ")
        //                {
        //                    cell = new PdfPCell(new Phrase("Sipariş Oluşturan:", new Font(STF_Helvetica_Turkish, 10, Font.ITALIC, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    tabloTarih.AddCell(cell);
        //                    cell = new PdfPCell(new Phrase("" + item.FisiOlusturanAdmin, new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    tabloTarih.AddCell(cell);
        //                }

        //                cell = new PdfPCell(tabloTarih);
        //                //cell.Border = PdfPCell.NO_BORDER;
        //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //                cell.VerticalAlignment = Element.ALIGN_TOP;
        //                cell.Rowspan = 2;
        //                cell.Border = PdfPCell.NO_BORDER;
        //                tableLogo.AddCell(cell);


        //                cell = new PdfPCell(aliciBil);
        //                cell.Border = PdfPCell.NO_BORDER; ;
        //                cell.Colspan = 1;
        //                tableLogo.AddCell(cell);


        //                if (Convert.ToBoolean(item.IhracKayitliMi))
        //                {
        //                    cell = new PdfPCell(new Phrase("İHRAÇ\nKAYITLI", new Font(STF_Helvetica_Turkish, 20, Font.BOLD, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    cell.PaddingTop = 10;
        //                    tableLogo.AddCell(cell);
        //                }
        //                else
        //                {
        //                    cell = new PdfPCell(new Phrase("", new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
        //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell.Border = PdfPCell.NO_BORDER;
        //                    tableLogo.AddCell(cell);
        //                }


        //            }









        //            document.Open();
        //            document.Add(tableLogo);
        //            document.Add(new Paragraph("\n"));
        //            document.Add(table);
        //            document.Close();
        //            byte[] bytes = memoryStream.ToArray();
        //            memoryStream.Close();

                  
        //            return bytes;




        //        }

        //    }
        //    catch (Exception hata)
        //    {
        //        return null;
        //    }
        //}
        //public string yaziyaCevir(decimal tutar)
        //{
        //    string sTutar = tutar.ToString("F2").Replace('.', ','); // Replace('.',',') ondalık ayracının . olma durumu için            
        //    string lira = sTutar.Substring(0, sTutar.IndexOf(',')); //tutarın tam kısmı
        //    string kurus = sTutar.Substring(sTutar.IndexOf(',') + 1, 2);
        //    string yazi = "";

        //    string[] birler = { "", "BİR", "İKİ", "ÜÇ", "DÖRT", "BEŞ", "ALTI", "YEDİ", "SEKİZ", "DOKUZ" };
        //    string[] onlar = { "", "ON", "YİRMİ", "OTUZ", "KIRK", "ELLİ", "ALTMIŞ", "YETMİŞ", "SEKSEN", "DOKSAN" };
        //    string[] binler = { "KATRİLYON", "TRİLYON", "MİLYAR", "MİLYON", "BİN", "" }; //KATRİLYON'un önüne ekleme yapılarak artırabilir.

        //    int grupSayisi = 6; //sayıdaki 3'lü grup sayısı. katrilyon içi 6. (1.234,00 daki grup sayısı 2'dir.)
        //                        //KATRİLYON'un başına ekleyeceğiniz her değer için grup sayısını artırınız.

        //    lira = lira.PadLeft(grupSayisi * 3, '0'); //sayının soluna '0' eklenerek sayı 'grup sayısı x 3' basakmaklı yapılıyor.            

        //    string grupDegeri;

        //    for (int i = 0; i < grupSayisi * 3; i += 3) //sayı 3'erli gruplar halinde ele alınıyor.
        //    {
        //        grupDegeri = "";

        //        if (lira.Substring(i, 1) != "0")
        //            grupDegeri += birler[Convert.ToInt32(lira.Substring(i, 1))] + "YÜZ"; //yüzler                

        //        if (grupDegeri == "BİRYÜZ") //biryüz düzeltiliyor.
        //            grupDegeri = "YÜZ";

        //        grupDegeri += onlar[Convert.ToInt32(lira.Substring(i + 1, 1))]; //onlar

        //        grupDegeri += birler[Convert.ToInt32(lira.Substring(i + 2, 1))]; //birler                

        //        if (grupDegeri != "") //binler
        //            grupDegeri += " " + binler[i / 3] + " ";

        //        if (grupDegeri == "BİRBİN") //birbin düzeltiliyor.
        //            grupDegeri = "BİN";

        //        yazi += grupDegeri;
        //    }

        //    if (yazi != "")
        //        yazi += "TL ";

        //    int yaziUzunlugu = yazi.Length;

        //    if (kurus.Substring(0, 1) != "0") //kuruş onlar
        //        yazi += onlar[Convert.ToInt32(kurus.Substring(0, 1))];

        //    if (kurus.Substring(1, 1) != "0") //kuruş birler
        //        yazi += birler[Convert.ToInt32(kurus.Substring(1, 1))];

        //    if (yazi.Length > yaziUzunlugu)
        //        yazi += " KR";
        //    else
        //        yazi += "SIFIR KR";

        //    return yazi;
        //}

        //private static bool EmailKontrol(string email)
        //{
        //    try
        //    {
        //        MailAddress m = new MailAddress(email);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }


        //}
    }
   
}