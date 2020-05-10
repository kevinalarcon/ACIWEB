using ACI_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACI_WEB.Controllers
{
    public class SubTipoController : Controller
    {
        private DBModel db = new DBModel();

        public ActionResult ListarSubTipos(int codtipo)
        {
            return Json(db.SubTipo.Where(x => x.codtipo == codtipo).Select(s => new {
                codsubtipo = s.codsubtipo,
                subtipo1 = s.subtipo1,
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}