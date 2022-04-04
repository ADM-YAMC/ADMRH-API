using ADMRH_API.Guard;
using ADMRH_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
                    var token = tokens.generateToken();
                    var clienteUsuario = await _context.Usuarios.FirstAsync(user =>
                         user.Correo == dataLogin.user && user.Contraseña == dataLogin.pass);
                    if (await UpdateToken(clienteUsuario, token))
                    {
                        return Ok(JsonData.ConvertJsonDataLogin(clienteUsuario, token));
                    }
                    else
                    {
                        return Ok(new Ans()
                        {
                            ok = false,
                            mensaje = "Ocurrio un error faltal al actualizar su token de inicio..."
                        });
                    }
                }
                catch (Exception)
                {
                    return Ok(new Ans()
                    {
                        ok = false,
                        mensaje = "El usuario o la contraseña son incorrectos..."
                    });
                }
            }
       
        private async Task<bool> UpdateToken(Usuario usuario, string token)
        {
            usuario.Token = token;
            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(usuario.IdUsuario))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
    public class userDataLogin
    {
        public string user { get; set; }
        public string pass { get; set; }
    }
    public class Ans
    {
        public bool ok { get; set; }
        public string mensaje { get; set; }
    }
}
