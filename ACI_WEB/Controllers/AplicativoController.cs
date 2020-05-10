using ACI_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACI_WEB.Controllers
{
    public class AplicativoController : Controller
    {

        private DBModel db = new DBModel();

        public ActionResult ListarAplicativos(int codequipo)
        {
            return Json(db.Aplicativo.Where(x => x.codequipo == codequipo).Select(s => new {
                codaplicativo = s.codaplicativo,
                aplicativo1 = s.aplicativo1,
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}