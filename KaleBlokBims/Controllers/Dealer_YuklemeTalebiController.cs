using iTextSharp.text;
using iTextSharp.text.pdf;
using KaleBlokBims.Models;
using KaleBlokBims.Models.Classlar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Dealer_YuklemeTalebiController : Controller
    {
        // GET: Dealer_YuklemeTalebi
        public ActionResult YuklemeTalepleri()
        {
            return View();
        }

        [HttpPost]
        public string _yuklemeTalebiOlustur(string SiparisNo, string SiparisTarihi, string TalepAciklamasi,string Plakalar, string YuklemeTarihi, string yuklemeYapilacakMalzemelerJson)
        {
            try
            {
                var BayiKodu = Session["BayiKodu"].ToString();
                var BayiAdi = Session["BayiAdi"].ToString();
                var db = new Models.IZOKALEPORTALEntities();
                var yuklemeYapilacakMalzemelerDt = JsonConvert.DeserializeObject<DataTable>(yuklemeYapilacakMalzemelerJson);

                YuklemeTalebiMaster ytm = new YuklemeTalebiMaster
                {
                    BayiKodu = BayiKodu,
                    BayiAdi = BayiAdi,
                    SiparisNo = SiparisNo,
                    SiparisTarihi = Convert.ToDateTime(SiparisTarihi),
                    TalepAciklamasi = TalepAciklamasi,
                    BayiTalebiYuklemeTarihi = Convert.ToDateTime(YuklemeTarihi),
                    TalepAcilisTarihi = DateTime.Now,
                    Plakalar=Plakalar,
                    TalebiAcan=Session["MailAdresi"].ToString(),
                    YuklendiMi=false

                };
                db.YuklemeTalebiMaster.Add(ytm);
                db.SaveChanges();


                long talepMasterLog = db.YuklemeTalebiMaster.Where(x => x.BayiKodu.Equals(BayiKodu)).Max(x => x.LOGICALREF);

                YuklemeTalebiDetayi ytd;

                for (int i = 0; i < yuklemeYapilacakMalzemelerDt.Rows.Count; i++)
                {
                    ytd = new YuklemeTalebiDetayi
                    {
                        BekleyenMiktar = Convert.ToDouble(yuklemeYapilacakMalzemelerDt.Rows[i]["bekleyenMiktar"]),
                        Birim = yuklemeYapilacakMalzemelerDt.Rows[i]["birim"].ToString(),
                        MalzemeAciklamasi = yuklemeYapilacakMalzemelerDt.Rows[i]["malzemeAdi"].ToString(),
                        MalzemeKodu = yuklemeYapilacakMalzemelerDt.Rows[i]["malzemeKodu"].ToString(),
                        YuklemeTalepMiktari = Convert.ToDouble(yuklemeYapilacakMalzemelerDt.Rows[i]["yuklenecekMiktar"]),
                        YuklemeTeklifiMasterLogicalref = talepMasterLog,


                    };
                    db.YuklemeTalebiDetayi.Add(ytd);
                    db.SaveChanges();

                }


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



                    /***********************************************************************/

                    //fiş içerikleri
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 99f;
                    float[] tablewidths = new float[] { 10f, 60f, 10f, 10f, 10f };
                    table.SetWidths(tablewidths);

                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["Logo"].ToString()));
                    img.ScaleToFit(100, 80);
                    cell = new PdfPCell(img);
                    cell.Border = PdfPCell.NO_BORDER;
                    cell.Colspan = 5;
                    table.AddCell(cell);


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

                    cell = new PdfPCell(new Phrase(":" + BayiAdi, kolonBasligi));
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

                    cell = new PdfPCell(new Phrase(":" + SiparisTarihi + "/" + SiparisNo, kolonBasligi));
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

                    cell = new PdfPCell(new Phrase(":" + Convert.ToDateTime(YuklemeTarihi).ToString("dd.MM.yyyy"), kolonBasligi));
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
                            "<td style='padding:2px'>" + yuklemeYapilacakMalzemelerDt.Rows[i]["malzemeKodu"].ToString() + "</td>" +
                            "<td style='padding:2px'>" + yuklemeYapilacakMalzemelerDt.Rows[i]["malzemeAdi"].ToString() + "</td>" +
                            "<td style='padding:2px'>" + yuklemeYapilacakMalzemelerDt.Rows[i]["birim"].ToString() + "</td>" +
                            "<td style='padding:2px'>" + yuklemeYapilacakMalzemelerDt.Rows[i]["bekleyenMiktar"].ToString() + "</td>" +
                            "<td style='padding:2px'>" + yuklemeYapilacakMalzemelerDt.Rows[i]["yuklenecekMiktar"].ToString() + "</td>" +
                            "</tr>";

                        cell = new PdfPCell(new Phrase(yuklemeYapilacakMalzemelerDt.Rows[i]["malzemeKodu"].ToString(), kolonBasligi));
                        cell.BorderWidthTop = 0;
                        cell.PaddingBottom = 10;
                        if (i != yuklemeYapilacakMalzemelerDt.Rows.Count - 1)
                            cell.BorderWidthBottom = 0;

                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(yuklemeYapilacakMalzemelerDt.Rows[i]["malzemeAdi"].ToString(), kolonBasligi));
                        cell.BorderWidthTop = 0;
                        cell.PaddingBottom = 10;
                        if (i != yuklemeYapilacakMalzemelerDt.Rows.Count - 1)
                            cell.BorderWidthBottom = 0;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(yuklemeYapilacakMalzemelerDt.Rows[i]["birim"].ToString(), kolonBasligi));
                        cell.BorderWidthTop = 0;
                        cell.PaddingBottom = 10;
                        if (i != yuklemeYapilacakMalzemelerDt.Rows.Count - 1)
                            cell.BorderWidthBottom = 0;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(yuklemeYapilacakMalzemelerDt.Rows[i]["bekleyenMiktar"].ToString(), kolonBasligi));
                        cell.BorderWidthTop = 0;
                        cell.PaddingBottom = 10;
                        if (i != yuklemeYapilacakMalzemelerDt.Rows.Count - 1)
                            cell.BorderWidthBottom = 0;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(yuklemeYapilacakMalzemelerDt.Rows[i]["yuklenecekMiktar"].ToString(), kolonBasligi));
                        cell.BorderWidthTop = 0;
                        cell.PaddingBottom = 10;
                        if (i != yuklemeYapilacakMalzemelerDt.Rows.Count - 1)
                            cell.BorderWidthBottom = 0;
                        table.AddCell(cell);


                    }

                    tablestring += "</tbody></table><br/><b style='font-size:20px'>" + TalepAciklamasi + "</b><br/>Plakalar:<b>"+Plakalar+"</b>";
                    cell = new PdfPCell(new Phrase(TalepAciklamasi, new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.Colspan = 5;
                    cell.PaddingTop = 20;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Plakalar: "+Plakalar, new Font(STF_Helvetica_Turkish, 10, Font.BOLD, BaseColor.BLACK)));
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




                    var icerikk = "<p>" + BayiAdi + " tarafından oluşturulan yükleme formu ekte yer almaktadır.</p>" +
                        tablestring;

                    MailGonderme mailGonder = new MailGonderme();
                    mailGonder.EkliMailGonderme("", Session["MailAdresi"].ToString(), Models.Classlar.SabitTanimlar.SiparisFormuGonderilecekMailler(), BayiAdi + " YÜKLEME FORMU", icerikk, bytes, "Yukleme_" + DateTime.Now.ToString("dd-MM-yyyy") + ".pdf");
                }

                return "true";
            }
            catch (Exception hata)
            {

                return "false";
            }

        }
    }
}