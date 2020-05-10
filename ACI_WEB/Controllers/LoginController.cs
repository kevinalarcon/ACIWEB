using ACI_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACI_WEB.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            //https://www.youtube.com/watch?v=EyrKUSwi4uI
            return View();
        }

        [HttpPost]
        public ActionResult Autorizacion(Usuario us)
        {
            using (DBModel db = new DBModel())
            {
                var userDetails = db.Usuario.Where(x => x.matricula == us.matricula && x.pass == us.pass).FirstOrDefault();
                if (userDetails == null)
                {
                    us.LoginErrorMessage = "Matrícula y/o Contraseña incorrectos";
                    return View("Index", us);
                }
                else
                {
                    Session["Matricula"] = userDetails.matricula;
                    Session["Nombre"] = userDetails.nombres;
                    return RedirectToAction("MostrarTicket", "Ticket");
                }
            }
        }

        public ActionResult LogOut()
        {
            Session["Matricula"] = null;
            Session["Nombre"] = null;
            Session.Abandon();
            return RedirectToAction("Index","Login");
        }
    }
}