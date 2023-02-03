using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaleBlokBims.Models.Classlar
{
    public class Yetkiler
    {
        public static BayiKullaniciYetkileri BayiYetkileri()
        {
            if (HttpContext.Current.Session["AdminMi"].ToString()=="1")
            {
                var db = new Models.IZOKALEPORTALEntities();
                return db.BayiKullaniciYetkileri.Where(x=>x.KullaniciID==-1).FirstOrDefault();

            }
            return JsonConvert.DeserializeObject<BayiKullaniciYetkileri>(HttpContext.Current.Session["Yetkiler"].ToString());
           
        }

        public static AdminKullaniciYetkisi AdminKullaniciYetkisi()
        {           
            return JsonConvert.DeserializeObject<AdminKullaniciYetkisi>(HttpContext.Current.Session["AdminYetkiler"].ToString());

        }
    }
}