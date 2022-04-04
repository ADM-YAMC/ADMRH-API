using System;
using System.Collections.Generic;

#nullable disable

namespace ADMRH_API.Models
{
    public partial class Candidato
    {
        public int IdCandidato { get; set; }
        public int IdVacante { get; set; }
        public int IdArchivos { get; set; }
        public int IdUsuarioCreacion { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cedula { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string FechaCreacion { get; set; }
    }
}
