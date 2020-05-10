using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACI_WEB.Models
{
    public class MisTickets
    {
        public Ticket ticketdetalle { get; set; }
        public Equipo equipodetalle { get; set; }
        public Usuario usuariodetalle { get; set; }
        public Tipo tipodetalle { get; set; }
        public Aplicativo aplicativodetalle { get; set; }
    }
}