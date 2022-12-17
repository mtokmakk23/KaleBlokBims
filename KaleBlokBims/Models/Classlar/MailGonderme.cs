using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace KaleBlokBims.Models.Classlar
{
    public class MailGonderme
    {
        string mesajGonderecekMail = "bportal@kaleblokbims.com";
        string mailAdi = "Kale Blok Bims Sistem";
        string MesajSifresi = "@CViA3id4Ni3@";
        public bool EkliMailGonderme(string Bcc, string CC, string Alici, string Konu, string Icerik, byte[] DosyaByte, string UzantiliSekildeDosyaAdi)
        {
            try
            {
                MailMessage mesaj = new MailMessage();
                mesaj.From = new MailAddress(mesajGonderecekMail, mailAdi);
                if (Bcc.Trim() != "")
                {
                    if (Bcc[Bcc.Length - 1].ToString() != ",")
                    {
                        Bcc = Bcc + ",";
                    }
                    var bccString = Bcc.Split(',');
                    for (int i = 0; i < bccString.Length - 1; i++)
                    {
                        mesaj.Bcc.Add(bccString[i]);
                    }
                }
                if (CC.Trim() != "")
                {
                    if (CC[CC.Length - 1].ToString() != ",")
                    {
                        CC = CC + ",";
                    }
                    var ccString = CC.Split(',');
                    for (int i = 0; i < ccString.Length - 1; i++)
                    {
                        mesaj.CC.Add(ccString[i]);
                    }
                }

                if (Alici.Trim() != "")
                {

                    if (Alici[Alici.Length - 1].ToString() != ",")
                    {
                        Alici = Alici + ",";
                    }
                    var toString = Alici.Split(',');
                    for (int i = 0; i < toString.Length - 1; i++)
                    {
                        mesaj.To.Add(toString[i]);
                    }

                }


                mesaj.Subject = Konu;
                mesaj.IsBodyHtml = true;
                mesaj.Body = Icerik;
                Attachment at = new Attachment(new MemoryStream(DosyaByte), UzantiliSekildeDosyaAdi);
                mesaj.Attachments.Add(at);
                mesaj.IsBodyHtml = true; // giden mailin içeriği html olmasını istiyorsak true kalması lazım
                SmtpClient client = new SmtpClient("mail.kaleblokbims.com", 587);
                client.Credentials = new NetworkCredential(mesajGonderecekMail, MesajSifresi);
                client.EnableSsl = true;
                ServicePointManager.ServerCertificateValidationCallback =
           delegate (
               object s,
               X509Certificate certificate,
               X509Chain chain,
               SslPolicyErrors sslPolicyErrors
           )
           {
               return true;
           };
                client.Send(mesaj);
                at.Dispose();
                return true;
            }
            catch (Exception hata)
            {
                return false;
            }

        }

        public bool EksizMailGonderme(string Bcc, string CC, string Alici, string Konu, string Icerik)
        {
            try
            {
                MailMessage mesaj = new MailMessage();
                mesaj.From = new MailAddress(mesajGonderecekMail, mailAdi);

                if (Bcc.Trim() != "")
                {
                    if (Bcc[Bcc.Length - 1].ToString() != ",")
                    {
                        Bcc = Bcc + ",";
                    }
                    var bccString = Bcc.Split(',');
                    for (int i = 0; i < bccString.Length - 1; i++)
                    {
                        mesaj.Bcc.Add(bccString[i]);
                    }
                }
                if (CC.Trim() != "")
                {
                    if (CC[CC.Length - 1].ToString() != ",")
                    {
                        CC = CC + ",";
                    }
                    var ccString = CC.Split(',');
                    for (int i = 0; i < ccString.Length - 1; i++)
                    {
                        mesaj.CC.Add(ccString[i]);
                    }
                }


                if (Alici.Trim() != "")
                {

                    if (Alici[Alici.Length - 1].ToString() != ",")
                    {
                        Alici = Alici + ",";
                    }
                    var toString = Alici.Split(',');
                    for (int i = 0; i < toString.Length - 1; i++)
                    {
                        mesaj.To.Add(toString[i]);
                    }

                }


                mesaj.Subject = Konu;
                mesaj.IsBodyHtml = true;
                mesaj.Body = Icerik;
                mesaj.IsBodyHtml = true; // giden mailin içeriği html olmasını istiyorsak true kalması lazım
                SmtpClient client = new SmtpClient("mail.kaleblokbims.com", 587);
                client.Credentials = new NetworkCredential(mesajGonderecekMail, MesajSifresi);
                client.EnableSsl = true;
                ServicePointManager.ServerCertificateValidationCallback =
            delegate (
                object s,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors
            )
            {
                return true;
            };
                client.Send(mesaj);

                return true;
            }
            catch (Exception hata)
            {

                return false;
            }

        }
    }
}