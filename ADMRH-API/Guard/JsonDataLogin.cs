using ADMRH_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADMRH_API.Guard
{
    public class JsonDataLogin
    {
        public AnsWers ConvertJsonDataLogin(Usuario usuario, string tokens)
        {

            var dta = new AnsWers()
            {
                ok = true,
                token = tokens,
                user = new List<Usuario>()
                    {
                        new Usuario()
                        {
                           IdUsuario = usuario.IdUsuario,
                           Cedula = usuario.Cedula,
                           Nombre = usuario.Nombre,
                           Apellido = usuario.Apellido,
                           Departamento = usuario.Departamento,
                           Telefono = usuario.Telefono,
                           Direccion = usuario.Direccion,
                           Rol = usuario.Rol,
                           Correo = usuario.Correo,

                        }
                    }
            };
            return dta;
        }

    public class AnsWers
    {
        public bool ok { get; set; }
        public string token { get; set; }
        public List<Usuario> user { get; set; }
    }
}
}
