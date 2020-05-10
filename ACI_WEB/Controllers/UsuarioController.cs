using ACI_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACI_WEB.Controllers
{
    public class UsuarioController : Controller
    {
        private DBModel db = new DBModel();

        public ActionResult ListarUsuarios(int codequipo)
        {
            return Json(db.Usuario.Where(x => x.codequipo == codequipo).Select(s => new {
                matricula = s.matricula,
                nombrecompleto = string.Concat(s.nombres," ",s.ap_paterno, " ",s.ap_materno),
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}