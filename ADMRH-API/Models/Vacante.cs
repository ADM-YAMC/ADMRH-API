using System;
using System.Collections.Generic;

#nullable disable

namespace ADMRH_API.Models
{
    public partial class Vacante
    {
        public int IdVacante { get; set; }
        public int IdUsuarioCreacion { get; set; }
        public string Nombre { get; set; }
        public string Departamento { get; set; }
        public string Descripcion { get; set; }
        public string Cargo { get; set; }
        public string Salario { get; set; }
        public string FechaCreacion { get; set; }
    }
}
