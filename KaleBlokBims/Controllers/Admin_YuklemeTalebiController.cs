using iTextSharp.text;
using iTextSharp.text.pdf;
using KaleBlokBims.Models;
using KaleBlokBims.Models.Classlar;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Admin_YuklemeTalebiController : Controller
    {
        // GET: Admin_YuklemeTalebi
        public ActionResult YuklemeTalepleri()
        {
            return View();
        }


        [HttpPost]
        public string _talepleriGetir()
        {

            var db = new Models.IZOKALEPORTALEntities();

            return JsonConvert.SerializeObject(db.YuklemeTalebiMaster, new IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });

        }

        [HttpPost]
        public string _talepBilgileri(int LOGICALREF)
        {
            var db = new Models.IZOKALEPORTALEntities();
            var tbl = db.YuklemeTalebiDetayi.Where(x => x.YuklemeTeklifiMasterLogicalref.ToString() == (LOGICALREF.ToString()));


            return JsonConvert.SerializeObject(tbl, new IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });

        }

        [HttpPost]
        public string _talebiGuncelle(string LOGICALREF, string firmaTarihi, string firmaAciklamasi)
        {
            try
            {
                var db = new Models.IZOKALEPORTALEntities();

                YuklemeTalebiMaster guncellenecekUrun = db.YuklemeTalebiMaster.FirstOrDefault(u => u.LOGICALREF.ToString() == LOGICALREF.ToString());
                guncellenecekUrun.FirmaYuklemeTarihi = Convert.ToDateTime(firmaTarihi);
                guncellenecekUrun.FirmaAciklamasi = (firmaAciklamasi);
                db.SaveChanges();               
                var yuklemeYapilacakMalzemelerDt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(db.YuklemeTalebiDetayi.Where(x => x.YuklemeTeklifiMasterLogicalref.ToString() == (LOGICALREF))));
                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 15f, 15f, 15f, 15f);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BaseFont STF_Helvetica_Turkish = BaseFont.CreateFont("C:\\Windows\\Fonts\\Trebuc.ttf", "windows-1254", true);

                    //BaseFont STF_Helvetica_Turkish = BaseFont.CreateFont("Helvetica", "CP1254", BaseFont.NOT_EMBEDDED);
                    Font icerik = new Font(STF_Helvetica_Turkish, 8, Font.NORMAL, BaseColor.BLACK);
                    Font kolonBasligi = new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK);
                    Font adresDetayi = new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    PdfPCell cell;

                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Server.MapPath("/assets/images/logom.png"));
                    img.ScaleToFit(100, 80);

                    /***********************************************************************/

                    //fiş içerikleri
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 99f;
                    float[] tablewidths = new float[] { 10f, 60f, 10f, 10f, 10f };
                    table.SetWidths(tablewidths);

                    cell = new PdfPCell(new Phrase("YÜKLEME TALEP FORMU", new Font(STF_Helvetica_Turkish, 13, Font.BOLD, BaseColor.BLACK)));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.Colspan = 5;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Firma Adı", kolonBasligi));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(":" + guncellenecekUrun.BayiAdi, kolonBasligi));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.Colspan = 3;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(DateTime.Now.ToString("dd.MM.yyyy"), kolonBasligi));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);


                    cell = new PdfPCell(new Phrase("Sipariş Tarih/No", kolonBasligi));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(":" +Convert.ToDateTime(guncellenecekUrun.SiparisTarihi).ToString("dd.MM.yyyy") + "/" + guncellenecekUrun.SiparisNo, kolonBasligi));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.Colspan = 3;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", kolonBasligi));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);


                    cell = new PdfPCell(new Phrase("Yükleme Tarihi", kolonBasligi));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.PaddingBottom = 20;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(":" + Convert.ToDateTime(guncellenecekUrun.FirmaYuklemeTarihi).ToString("dd.MM.yyyy"), kolonBasligi));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.PaddingBottom = 20;
                    cell.Colspan = 3;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", kolonBasligi));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.PaddingBottom = 20;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);
                    var tablestring = "<table cellspacing='0' style='font-size:12px'><thead><tr><td style='border-bottom:1px solid #dedede;padding:3px'>Kodu</td><td style='border-bottom:1px solid #dedede;padding:3px'>Adı</td><td style='border-bottom:1px solid #dedede;padding:3px'>Birimi</td><td style='border-bottom:1px solid #dedede;padding:3px'>Bekleyen Miktar</td><td style='border-bottom:1px solid #dedede;padding:3px'>Yüklenecek  Miktar</td></tr></thead><tbody>";

                    cell = new PdfPCell(new Phrase("Kodu", kolonBasligi));
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Adı", kolonBasligi));
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Birimi", kolonBasligi));
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Bekleyen Miktar", kolonBasligi));
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Yüklenecek  Miktar", kolonBasligi));
                    table.AddCell(cell);

                    for (int i = 0; i < yuklemeYapilacakMalzemelerDt.Rows.Count; i++)
                    {
                        tablestring += "<tr>" +
                            "<td style='padding:2px'>" + yuklemeYapilacakMalzemelerDt.Rows[i]["MalzemeKodu"].ToString() + "</td>" +
                            "<td style='padding:2px'>" + yuklemeYapilacakMalzemelerDt.Rows[i]["MalzemeAciklamasi"].ToString() + "</td>" +
                            "<td style='padding:2px'>" + yuklemeYapilacakMalzemelerDt.Rows[i]["Birim"].ToString() + "</td>" +
                            "<td style='padding:2px'>" + yuklemeYapilacakMalzemelerDt.Rows[i]["BekleyenMiktar"].ToString() + "</td>" +
                            "<td style='padding:2px'>" + yuklemeYapilacakMalzemelerDt.Rows[i]["YuklemeTalepMiktari"].ToString() + "</td>" +
                            "</tr>";
                        cell = new PdfPCell(new Phrase(yuklemeYapilacakMalzemelerDt.Rows[i]["MalzemeKodu"].ToString(), kolonBasligi));
                        cell.BorderWidthTop = 0;
                        cell.PaddingBottom = 10;
                        if (i != yuklemeYapilacakMalzemelerDt.Rows.Count - 1)
                            cell.BorderWidthBottom = 0;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(yuklemeYapilacakMalzemelerDt.Rows[i]["MalzemeAciklamasi"].ToString(), kolonBasligi));
                        cell.BorderWidthTop = 0;
                        cell.PaddingBottom = 10;
                        if (i != yuklemeYapilacakMalzemelerDt.Rows.Count - 1)
                            cell.BorderWidthBottom = 0;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(yuklemeYapilacakMalzemelerDt.Rows[i]["Birim"].ToString(), kolonBasligi));
                        cell.BorderWidthTop = 0;
                        cell.PaddingBottom = 10;
                        if (i != yuklemeYapilacakMalzemelerDt.Rows.Count - 1)
                            cell.BorderWidthBottom = 0;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(yuklemeYapilacakMalzemelerDt.Rows[i]["BekleyenMiktar"].ToString(), kolonBasligi));
                        cell.BorderWidthTop = 0;
                        cell.PaddingBottom = 10;
                        if (i != yuklemeYapilacakMalzemelerDt.Rows.Count - 1)
                            cell.BorderWidthBottom = 0;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(yuklemeYapilacakMalzemelerDt.Rows[i]["YuklemeTalepMiktari"].ToString(), kolonBasligi));
                        cell.BorderWidthTop = 0;
                        cell.PaddingBottom = 10;
                        if (i != yuklemeYapilacakMalzemelerDt.Rows.Count - 1)
                            cell.BorderWidthBottom = 0;
                        table.AddCell(cell);
                    }
                    tablestring += "</tbody></table><br/><b>" + guncellenecekUrun.TalepAciklamasi + "</b><br/>Plakalar: <b>" + guncellenecekUrun.Plakalar + "</b><br/>Firma Açıklaması: <b>" + guncellenecekUrun.FirmaAciklamasi + "</b>";
                    cell = new PdfPCell(new Phrase(guncellenecekUrun.TalepAciklamasi, new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.Colspan = 5;
                    cell.PaddingTop = 20;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Plakalar:" + guncellenecekUrun.Plakalar, new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.Colspan = 5;
                    cell.PaddingTop = 20;
                    table.AddCell(cell);


                    cell = new PdfPCell(new Phrase("Firma Açıklaması:" + guncellenecekUrun.FirmaAciklamasi, new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.Colspan = 5;
                    cell.PaddingTop = 20;
                    table.AddCell(cell);


                    document.Open();
                    document.Add(table);
                    document.Close();
                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    var icerikk = "<p>KALEBLOK tarafından güncellenen yükleme formu ekte yer almaktadır.</p>" +
                        tablestring;
                    MailGonderme mailGonder = new MailGonderme();
                    mailGonder.EkliMailGonderme("", Models.Classlar.SabitTanimlar.SiparisFormuGonderilecekMailler(), guncellenecekUrun.TalebiAcan,
                        "YÜKLEME FORMU GÜNCELLEME",
                        icerikk, bytes, "SiparisFormu_" + DateTime.Now.ToString("dd-MM-yyyy") + ".pdf");
                }
                return "true";
            }
            catch (Exception hata)
            {

                return "false";
            }


        }

        [HttpPost]
        public string _talebiSil(int LOGICALREF)
        {
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                db.Database.ExecuteSqlCommand("delete from YuklemeTalebiMaster where LOGICALREF="+LOGICALREF);
                db.Database.ExecuteSqlCommand("delete from YuklemeTalebiDetayi where YuklemeTeklifiMasterLogicalref=" + LOGICALREF);
                return "true";
            }
            catch (Exception)
            {

                return "false";
            }


        }  
        
        [HttpPost]
        public string _talepYuklendi(int LOGICALREF)
        {
            try
            {
                var db = new Models.IZOKALEPORTALEntities();
                var yuklemeTalebi = db.YuklemeTalebiMaster.Where(x => x.LOGICALREF == LOGICALREF).FirstOrDefault();
                if (yuklemeTalebi!=null)
                {
                    yuklemeTalebi.YuklendiMi = true;
                    db.SaveChanges();
                }
                return "true";
            }
            catch (Exception)
            {

                return "false";
            }


        }
    }
}