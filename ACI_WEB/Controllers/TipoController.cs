using ACI_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACI_WEB.Controllers
{
    public class TipoController : Controller
    {
        private DBModel db = new DBModel();

        public ActionResult ListarTipos(int codequipo)
        {
            return Json(db.Tipo.Where(x => x.codequipo == codequipo).Select(s => new {
                codtipo = s.codtipo,
                tipo1 = s.tipo1,
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}