//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACI_WEB.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Aplicativo
    {
        public int codaplicativo { get; set; }
        public string aplicativo1 { get; set; }
        public Nullable<int> codequipo { get; set; }
        public string matricula { get; set; }
        public string backup1 { get; set; }
        public string backup2 { get; set; }
    
        public virtual Equipo Equipo { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
