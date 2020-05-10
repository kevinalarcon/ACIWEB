using ACI_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACI_WEB.Controllers
{
    public class TicketController : Controller
    {
        private DBModel db = new DBModel();

        // GET: Ticket
        public ActionResult Index()
        {
            if (Session["Matricula"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        public ActionResult MostrarTicket()
        {
            if (Session["Matricula"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(ListarTodoslosTickets());
        }

        public ActionResult MostrarTicketAsignados()
        {
            if (Session["Matricula"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(ListarTodoslosTicketsAsignados());
        }

        public ActionResult RegistrarTicket()
        {
            if (Session["Matricula"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.equipo = db.Equipo.Where(x => x.estado == 1).ToList();
            //ViewBag.tipo = db.Tipo.ToList();
            return View();
        }

        [HttpPost]
        //public ActionResult RegistrarTicket(FormCollection formcollection)
        public ActionResult RegistrarTicket(Ticket ticket)
        {
            try
            {
                //Ticket ticket = new Ticket();
                ticket.matricula = Session["Matricula"].ToString();
                ticket.fechaingreso = DateTime.Now;
                ticket.estado = "Pendiente";

                var codtipo = ticket.codtipo;
                ticket.idticket = GenerarCodTicket((int)codtipo);

                var codaplicativo = ticket.codaplicativo; 
                ticket.matricula_responsable = AsignarUsuarioAplicattivo((int)codaplicativo);

                using (DBModel d = new DBModel())
                {
                    d.Ticket.Add(ticket);
                    d.SaveChanges();
                }
                //return Json(new { Url = Url.Action("MostrarTicket", "Ticket") });
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "MostrarTicket", ListarTodoslosTickets()), message = "El ticket ingresado es " + ticket.idticket + "" }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("MostrarTicket", "Ticket");

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }

        IEnumerable<MisTickets> ListarTodoslosTickets()
        {
            string matricula = Session["Matricula"].ToString();
            List<Ticket> tickets;
            List<Equipo> equipos;
            IEnumerable<MisTickets> mistickets;

            using (DBModel d = new DBModel())
            {
                tickets = d.Ticket.ToList();
                equipos = d.Equipo.ToList();
            }

            mistickets = from t in tickets
                         where t.matricula == matricula
                         join e in equipos on t.codequipo equals e.codequipo into table1
                         from e in table1.DefaultIfEmpty()
                            
                            orderby t.fechaingreso descending
                             select new MisTickets
                             {
                                 ticketdetalle = t,
                                 equipodetalle = e
                             };


            return mistickets;
        }

        IEnumerable<MisTickets> ListarTodoslosTicketsAsignados()
        {


            string matricula = Session["Matricula"].ToString();


            List<Ticket> tickets;
            List<Equipo> equipos;
            List<Usuario> usuarios;
            List<Tipo> tipo;
            List<Aplicativo> aplicativo;

            IEnumerable<MisTickets> mistickets;

            using (DBModel d = new DBModel())
            {
                tickets = d.Ticket.ToList();
                equipos = d.Equipo.ToList();
                usuarios = d.Usuario.ToList();
                tipo = d.Tipo.ToList();
                aplicativo = d.Aplicativo.ToList();
            }

            mistickets = from t in tickets
                         where t.estado == "Pendiente" && t.matricula_responsable == matricula
                         join e in equipos on t.codequipo equals e.codequipo into table1
                         from e in table1.DefaultIfEmpty()
                         join a in aplicativo on t.codaplicativo equals a.codaplicativo into table2
                         from a in table2.DefaultIfEmpty()
                         //where a.matricula == matricula
                         join u in usuarios on t.matricula equals u.matricula into table4
                         from u in table4.DefaultIfEmpty()
                         join ti in tipo on t.codtipo equals ti.codtipo into table3
                         from ti in table3.DefaultIfEmpty()
                         orderby t.fechaingreso descending
                         select new MisTickets
                         {
                             ticketdetalle = t,
                             equipodetalle = e,
                             usuariodetalle = u,
                             tipodetalle = ti,
                             aplicativodetalle = a
                         };

            return mistickets;
        }

        public ActionResult VerTicketDetalle(string idticket)
        {
            if (Session["Matricula"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            MisTickets ticket = VerDetalleTicketEnviado(idticket);

            return View(ticket);
        }

        public MisTickets VerDetalleTicketEnviado(string idticket)
        {
            List<Ticket> tickets;
            List<Equipo> equipos;
            List<Usuario> usuarios;
            List<Tipo> tipo;
            List<Aplicativo> aplicativo;

            IEnumerable<MisTickets> mistickets;
            MisTickets ticketdetalle;

            using (DBModel d = new DBModel())
            {
                tickets = d.Ticket.Where(x => x.idticket == idticket).ToList();
                equipos = d.Equipo.ToList();
                usuarios = d.Usuario.ToList();
                tipo = d.Tipo.ToList();
                aplicativo = d.Aplicativo.ToList();
            }

            mistickets = from t in tickets
                         join e in equipos on t.codequipo equals e.codequipo into table1
                         from e in table1.DefaultIfEmpty()
                         join a in aplicativo on t.codaplicativo equals a.codaplicativo into table2
                         from a in table2.DefaultIfEmpty()
                         join ti in tipo on t.codtipo equals ti.codtipo into table3
                         from ti in table3.DefaultIfEmpty()
                         join u in usuarios on t.matricula_responsable equals u.matricula into table4
                         from u in table4.DefaultIfEmpty()
                         orderby t.fechaingreso descending
                         select new MisTickets
                         {
                             ticketdetalle = t,
                             equipodetalle = e,
                             usuariodetalle = u,
                             tipodetalle = ti,
                             aplicativodetalle = a
                         };

            ticketdetalle = mistickets.FirstOrDefault();

            return ticketdetalle;
        }


        public ActionResult AtenderTicket(string idticket)
        {
            if (Session["Matricula"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            MisTickets ticket = VerDetalleTicketEnviado(idticket);

            return View(ticket);
        }

        [HttpPost]
        public ActionResult AtenderTicket(Ticket ticket)
        {
            try
            {
                ticket.estado = "Atendido";
                ticket.fechaatencion = DateTime.Now;

                using (db as DBModel)
                {
                    db.Ticket.Attach(ticket);
                    db.Entry(ticket).Property(x => x.comentariofinal).IsModified = true;
                    db.Entry(ticket).Property(x => x.estado).IsModified = true;
                    db.Entry(ticket).Property(x => x.fechaatencion).IsModified = true;

                    db.Entry(ticket).Property(x => x.accion_inmediata).IsModified = true;
                    db.Entry(ticket).Property(x => x.causa).IsModified = true;
                    db.Entry(ticket).Property(x => x.codresponsable).IsModified = true;
                    // .State = System.Data.Entity.EntityState.Modified;

                    db.Entry(ticket).Property(x => x.cantidad_clientes).IsModified = true;
                    db.Entry(ticket).Property(x => x.cantidad_cuentas).IsModified = true;
                    db.Entry(ticket).Property(x => x.deuda_vencida).IsModified = true;
                    db.Entry(ticket).Property(x => x.deuda_total).IsModified = true;



                    db.SaveChanges();
                }

                //return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "MostrarTicketAsignados", ListarTodoslosTicketsAsignados()), message = "" }, JsonRequestBehavior.AllowGet);
                //return View("MostrarTicketAsignados", ListarTodoslosTicketsAsignados());
                //return RedirectToAction("MostrarTicketAsignados", "Ticket");
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "MostrarTicketAsignados", ListarTodoslosTicketsAsignados()), message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult DerivarTicket(string idticket)
        {
            if (Session["Matricula"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            MisTickets ticket = VerDetalleTicketEnviado(idticket);

            return View(ticket);
        }

        [HttpPost]
        public ActionResult DerivarTicket(Ticket ticket)
        {
            try
            {
                Ticket ticketderivado = new Ticket();
                Derivaciones derivaciones = new Derivaciones();

                ticketderivado.idticket = ticket.idticket;
                ticketderivado.matricula_responsable = ticket.matricula_nueva;

                if (ticket.tipoderivacion == 2)
                {
                    ticketderivado.codequipo = ticket.codequipo;
                    ticketderivado.codaplicativo = ticket.codaplicativo;

                    var codaplicativo = ticket.codaplicativo;
                    ticketderivado.matricula_responsable = AsignarUsuarioAplicattivo((int)codaplicativo);
                }

                derivaciones.idticket = ticket.idticket;
                derivaciones.matricula_origen = ticket.matricula_responsable;
                derivaciones.matricula_nueva = ticket.matricula_nueva;
                derivaciones.fechaderivacion = DateTime.Now;
                derivaciones.motivo = ticket.motivo;

                using (db as DBModel)
                {
                    db.Derivaciones.Add(derivaciones);
                    db.SaveChanges();

                    db.Ticket.Attach(ticketderivado);

                    if (ticket.tipoderivacion == 1)
                    {
                        db.Entry(ticketderivado).Property(x => x.matricula_responsable).IsModified = true;
                    }
                    else
                    {
                        db.Entry(ticketderivado).Property(x => x.codequipo).IsModified = true;
                        db.Entry(ticketderivado).Property(x => x.codaplicativo).IsModified = true;
                        db.Entry(ticketderivado).Property(x => x.matricula_responsable).IsModified = true;
                    }

                    db.SaveChanges();
                }
                
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "MostrarTicketAsignados", ListarTodoslosTicketsAsignados()), message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult MostrarAyuda(string idticket)
        {
            if (Session["Matricula"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }



            //Se asigna el usuario responsable del aplicativo o la cartera, pero si este no esta disponibe, se asigna a su backup
            public string AsignarUsuarioAplicattivo(int codaplicativo)
        {
            List<Usuario> usuarios;
            List<Aplicativo> aplicativo;

            IEnumerable<MisTickets> mistickets;
            MisTickets usuarioelegido;

            using (DBModel d = new DBModel())
            {
                usuarios = d.Usuario.ToList();
                aplicativo = d.Aplicativo.ToList();
            }

            mistickets = from a in aplicativo
                         where a.codaplicativo == codaplicativo
                         join u in usuarios on a.matricula equals u.matricula into table4
                         from u in table4.DefaultIfEmpty()
                         select new MisTickets
                         {
                             aplicativodetalle = a,
                             usuariodetalle = u
                         };

            usuarioelegido = mistickets.FirstOrDefault();

            if (usuarioelegido.usuariodetalle.estado == 0)
            {
                mistickets = from a in aplicativo
                             where a.codaplicativo == codaplicativo
                             join u in usuarios on a.backup1 equals u.matricula into table4
                             from u in table4.DefaultIfEmpty()
                             select new MisTickets
                             {
                                 aplicativodetalle = a,
                                 usuariodetalle = u
                             };

                usuarioelegido = mistickets.FirstOrDefault();

                if (usuarioelegido.usuariodetalle.estado == 0)
                {
                    mistickets = from a in aplicativo
                                 where a.codaplicativo == codaplicativo
                                 join u in usuarios on a.backup2 equals u.matricula into table4
                                 from u in table4.DefaultIfEmpty()
                                 select new MisTickets
                                 {
                                     aplicativodetalle = a,
                                     usuariodetalle = u
                                 };

                    usuarioelegido = mistickets.FirstOrDefault();
                }
            }

            return usuarioelegido.usuariodetalle.matricula;
        }

        public string GenerarCodTicket(int codtipo)
        {
            string codticket = "";
            int numero = 0;
            string numerodes = "";
            switch (codtipo)
            {
                case 1:
                    codticket = "QU";
                    break;
                case 2:
                    codticket = "IN";
                    break;
                case 3:
                    codticket = "WO";
                    break;
                case 4:
                    codticket = "AC";
                    break;
                case 5:
                    codticket = "QA";
                    break;
                case 6:
                    codticket = "CL";
                    break;
                default:
                    codticket = "RE";
                    break;
            }

            using (DBModel d = new DBModel())
            {
                numero = d.Ticket.Where(x => x.codequipo == codtipo).Count();
                numero = numero + 1;
            }

            if (numero < 10)
            {
                numerodes = "000000000" + numero.ToString();
            }
            else if (numero >=10 && numero < 100)
            {
                numerodes = "00000000" + numero.ToString();
            }
            else if (numero >= 100 && numero < 1000)
            {
                numerodes = "0000000" + numero.ToString();
            }
            else if (numero >= 1000 && numero < 10000)
            {
                numerodes = "000000" + numero.ToString();
            }
            else if (numero >= 10000 && numero < 100000)
            {
                numerodes = "00000" + numero.ToString();
            }
            else if (numero >= 100000 && numero < 1000000)
            {
                numerodes = "0000" + numero.ToString();
            }
            else if (numero >= 1000000 && numero < 10000000)
            {
                numerodes = "000" + numero.ToString();
            }
            else if (numero >= 10000000 && numero < 100000000)
            {
                numerodes = "00" + numero.ToString();
            }
            else if (numero >= 100000000 && numero < 1000000000)
            {
                numerodes = "0" + numero.ToString();
            }
            else
            {
                numerodes = numero.ToString();
            }

            codticket = codticket + numerodes;

            return codticket;
        }


    }
}