using System;
using System.Collections.Generic;

#nullable disable

namespace ADMRH_API.gh
{
    public partial class Vacante
    {
        public int IdVacante { get; set; }
        public int IdUsuarioCreacion { get; set; }
        public string Nombre { get; set; }
        public string Provincia { get; set; }
        public string DiaSemanaInicio { get; set; }
        public string DiaSemanaFin { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string EstadoPuesto { get; set; }
        public string Departamento { get; set; }
        public string Descripcion { get; set; }
        public string Cargo { get; set; }
        public string Salario { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
