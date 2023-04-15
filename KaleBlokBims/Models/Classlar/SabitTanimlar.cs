using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaleBlokBims.Models.Classlar
{
    public class SabitTanimlar
    {
        public static string SiparisFormuGonderilecekMailler()
        {
            var db = new Models.IZOKALEPORTALEntities();
            string mailAdresleri = "";
            foreach (var item in db.SiparisVeTeklifFormuGonderilecekMailler)
            {
                mailAdresleri += item.MailAdresi + ",";
            }
            return mailAdresleri;
            //return 
            //    "hayatiozdemir@kaleblokbims.com," +
            //    "yener@kaleblokbims.com," +
            //    "etanriver@hotmail.com," +
            //  //  "bayram.gul@kaleblokbims.com," +
            //    "selman.karakaya@kaleblokbims.com," +
            //    "ozcaniskender61@hotmail.com," +
            //    "tolgahan.imamgiller@kaleblokbims.com," +
            //    "tanriveronur@icloud.com," +
            //    "sevkiyat@kaleblokbims.com," +
            //    "osman@kaleblokbims.com," +
            //    "firat.yurdigul@kaleblokbims.com" +
            //    "" +               
            //    "";

        }

        public static string SikayetBildirimiYapilacakNumaralar()
        {
            var db = new Models.IZOKALEPORTALEntities();
            string gsmler = "";
            foreach (var item in db.SikayetBildirimiAtilacakKisiler)
            {
                if (item.Gsm!="" || item.Gsm!=null)
                {
                    gsmler += item.Gsm + ",";
                }
              
            }
            return gsmler;
           

        }
        public static string SikayetBildirimiYapilacakEmailler()
        {
            var db = new Models.IZOKALEPORTALEntities();
            string email = "";
            foreach (var item in db.SikayetBildirimiAtilacakKisiler)
            {
                if (item.Email != "" || item.Email != null)
                {
                    email += item.Email + ",";
                }

            }
            return email;


        }
    }
}