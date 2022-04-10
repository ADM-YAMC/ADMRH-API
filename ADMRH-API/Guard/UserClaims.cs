using System;

namespace ADMRH_API.Guard
{
    public class UserClaims
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Rol { get; set; }
        public string Correo { get; set; }
        public DateTime LoginDate { get; set; }
    }
}
