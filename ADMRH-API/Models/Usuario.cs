using System;
using System.Collections.Generic;

#nullable disable

namespace ADMRH_API.Models
{
    public partial class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string Departamento { get; set; }
        public string Direccion { get; set; }
        public string Rol { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string? Token { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
