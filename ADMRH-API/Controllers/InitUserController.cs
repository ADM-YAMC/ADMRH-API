using ADMRH_API.Guard;
using ADMRH_API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ADMRH_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InitUserController : ControllerBase
    {
        Tokens tokens = new Tokens();
        JsonDataLogin JsonData = new JsonDataLogin();
        private readonly ADMRHJQContext _context;

        public InitUserController(ADMRHJQContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        public async Task<ActionResult<Usuario>> GetClienteUsuario(userDataLogin dataLogin)
        {
            try
            {
                var descodePass = Base64Encode(dataLogin.pass);
                var clienteUsuario = await _context.Usuarios.FirstOrDefaultAsync(user =>
                        user.Correo == dataLogin.user && user.Contraseña == descodePass);
                    
                if(clienteUsuario == null || clienteUsuario?.IdUsuario == default)
                    return Ok(new Ans() { Mensaje = "Usuario o contraseña incorrecta" });

                return Ok(new Ans() 
                { 
                    Ok = true,
                    Claims = new UserClaims()
                    {
                        IdUsuario = clienteUsuario.IdUsuario,
                        PCambio = clienteUsuario.PCambio,
                        Nombre = clienteUsuario.Nombre,
                        Apellido = clienteUsuario.Apellido,
                        Correo = clienteUsuario.Correo,
                        LoginDate = clienteUsuario.LoginDate,
                        Rol = clienteUsuario.Rol
                    }
                });
            }
            catch (Exception)
            {
                return Ok(new Ans()
                {
                    Mensaje = "Ocurrió un problema con las credenciales proporcionadas"
                });
            }
        }
        private static string Base64Encode(string word)
        {
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(word);
            return Convert.ToBase64String(byt);
        }
    }
    public class userDataLogin
    {
        public string user { get; set; }
        public string pass { get; set; }
    }
    public class Ans
    {
        public bool Ok { get; set; }
        public UserClaims Claims { get; set; }
        public string Mensaje { get; set; }
    }
}
