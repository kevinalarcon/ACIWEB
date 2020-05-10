using ACI_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACI_WEB.Controllers
{
    public class ResponsableController : Controller
    {
        private DBModel db = new DBModel();

        public ActionResult ListarResponsables()
        {
            return Json(db.Responsable.Select(s => new {
                codresponsable = s.codresponsable,
                responsable1 = s.responsable1
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}