using ACI_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACI_WEB.Controllers
{
    public class EquipoController : Controller
    {
        private DBModel db = new DBModel();

        public ActionResult ListarEquipos()
        {
            return Json(db.Equipo.Where(x => x.estado == 1).Select(s => new {
                codequipo = s.codequipo,
                equipo1 = s.equipo1,
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}