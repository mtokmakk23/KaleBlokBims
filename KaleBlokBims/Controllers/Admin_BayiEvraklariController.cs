using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaleBlokBims.Controllers
{
    public class Admin_BayiEvraklariController : Controller
    {
        // GET: Admin_BayiEvraklari
        public ActionResult BayiEvraklari()
        {
            return View();
        }

        [HttpPost]
        public string TumEvraklar()
        {
            var db = new Models.IZOKALEPORTALEntities();
            var query = db.BayiEvraklari.ToList();
            return JsonConvert.SerializeObject(query);
        }
    }
}