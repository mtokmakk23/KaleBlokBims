using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Dealer_RaporlarController : Controller
    {
        // GET: Dealer_Raporlar
        public ActionResult MalzemeSevkRaporu()
        {
            return View();
        }

        [HttpPost]
        public string _MalzemeSevkRaporu(string baslangicTarihi,string bitisTarihi)
        {
            var response = new RestSharp.RestResponse();
            if (baslangicTarihi.Trim()==""||bitisTarihi.Trim()=="")
            {
                response.IsSuccessStatusCode = false;
                response.ErrorMessage = "Başlangıç Tarihi ve Bitiş Tarihi Doldurulmalıdır.";
            }
            else
            {
                var servis = new M2BWebService.ZOKALEAPISoapClient();
                response.IsSuccessStatusCode = true;
                response.Content = servis.MusteriyeSevkRaporu(Session["BayiKodu"].ToString(), Convert.ToDateTime(baslangicTarihi).ToString("yyyy-MM-dd"), Convert.ToDateTime(bitisTarihi).ToString("yyyy-MM-dd"));
               

            }
            return JsonConvert.SerializeObject(response);
        }
    }
}