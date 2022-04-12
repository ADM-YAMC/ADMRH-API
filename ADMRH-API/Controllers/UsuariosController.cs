using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ADMRH_API.Models;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
namespace ADMRH_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ADMRHJQContext _context;

        public UsuariosController(ADMRHJQContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("creacion/{id}")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosIdCreacion(int id)
        {
            return await _context.Usuarios.Where(x => x.IdCreacionUser == id).Select(x => x).ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        [HttpGet("cantidadVC/{id}")]
        public async Task<ActionResult<ResponseMessageCountCantidaVC>> cantidadVC(int id)
        {
            var candidatos = await _context.Candidatos.Where(x => x.IdUsuarioCreacion == id).Select(x => x).ToListAsync();
            var vacantes = await _context.Vacantes.Where(x => x.IdUsuarioCreacion == id).Select(x => x).ToListAsync();


            return new ResponseMessageCountCantidaVC()
            {
                ok = true,
                cantidadVacantes = vacantes.Count,
                CantidadCandidatos = candidatos.Count,

            };
        }


        [HttpGet("cantidadTotal_UCV")]
        public async Task<ActionResult<ResponsecantidadTotal_UCV>> cantidadTotal_UCV()
        {
            var candidatos = await _context.Candidatos.ToListAsync();
            var vacantes = await _context.Vacantes.ToListAsync();
            var usuarios = await _context.Usuarios.ToListAsync();


            return new ResponsecantidadTotal_UCV()
            {
                ok = true,
                cantidadTVacantes = vacantes.Count,
                CantidadTCandidatos = candidatos.Count,
                CantidadTUsuarios = usuarios.Count,
            };
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ResponseUser> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return new ResponseUser()
                {
                    ok = false,
                    mensaje = "El usuario no valido..."
                };
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return new ResponseUser()
                {
                    ok = true,
                    mensaje = "El usuario a sido actualizado con exito..."
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return new ResponseUser()
                    {
                        ok = false,
                        mensaje = "El usuario no existe..."
                    };
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPut("cambioContraseña/{id}")]
        public async Task<ResponseUser> PutPasswordUsuario(int id, CambioContraseña cambio)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            var descodePass = Base64Decode(usuario.Contraseña);
            if (descodePass != cambio.viejaContraseña)
            {
                return new ResponseUser()
                {
                    ok = false,
                    mensaje = "La antigua contraseña no es valida..."
                };
            }
            usuario.Contraseña = Base64Encode(cambio.nuevaContraseña);
            usuario.PCambio = cambio.Estado;
            _context.Entry(usuario).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return new ResponseUser()
                {
                    ok = true,
                    mensaje = "La contraseña a sido actualizado con exito..."
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return new ResponseUser()
                    {
                        ok = false,
                        mensaje = "El usuario no existe..."
                    };
                }
                else
                {
                    return new ResponseUser()
                    {
                        ok = false,
                        mensaje = "Ocurrio un error..."
                    };
                }
            }
        }

        [HttpPut("LookUnLookUser/{idUser}/{estado}")]
        public async Task<ResponseUser> PutLookUsuario(int idUser, int estado)
        {
            var usuario = await _context.Usuarios.FindAsync(idUser);
            usuario.Estado = estado;
            _context.Entry(usuario).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                if (estado ==1)
                {
                    return new ResponseUser()
                    {
                        ok = true,
                        mensaje = "El usuario a sido bloqueado con exito..."
                    };
                }
                else
                {
                    return new ResponseUser()
                    {
                        ok = true,
                        mensaje = "El usuario a sido desbloqueado con exito..."
                    };
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(idUser))
                {
                    return new ResponseUser()
                    {
                        ok = false,
                        mensaje = "El usuario no existe..."
                    };
                }
                else
                {
                    return new ResponseUser()
                    {
                        ok = false,
                        mensaje = "Ocurrio un error..."
                    };
                }
            }
        }
        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ResponseUser>> PostUsuario(Usuario usuario)
        {
            
            try
            {
                var usuarios = await _context.Usuarios.ToListAsync();
                var result = usuarios.Where(x => x.Correo == usuario.Correo || x.Cedula == usuario.Cedula).Select(x => x).ToList();
                if (result.Count.Equals(0))
                {
                    usuario.Estado = 0;
                    usuario.Contraseña = Base64Encode(usuario.Contraseña);
                    _context.Usuarios.Add(usuario);
                    await _context.SaveChangesAsync();
                    enviarCorreo(usuario);
                    return new ResponseUser()
                    {
                        ok = true,
                        mensaje = "El usuario a sido registrado con exito..."
                    };
                }
                else
                {
                    return new ResponseUser()
                    {
                        ok = false,
                        mensaje = "Ya existe un correo o una cedula del mismo tipo en el sistema..."
                    };
                }
            }
            catch (Exception)
            {

                return new ResponseUser()
                {
                    ok = false,
                    mensaje = "Ocurrio un error al intertar guardar los datos... Favor de comunicarse con el servicio tecnico..."
                };
            }
           
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<ResponseUser> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return new ResponseUser()
                {
                    ok = false,
                    mensaje = "El usuario que intenta eliminar no es valido..."
                };
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return new ResponseUser()
            {
                ok = true,
                mensaje = "El usuario fue eliminado con exito..."
            };
        }

        private static string Base64Encode(string word)
        {
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(word);
            return  Convert.ToBase64String(byt);
        }
        private static string Base64Decode(string word)
        {
            byte[] b = Convert.FromBase64String(word);
            return System.Text.Encoding.UTF8.GetString(b);
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }

        private bool enviarCorreo(Usuario usuario)
        {
           
            string sub = @$"<div class='container'>
        <br />
        <div style='background: #023877; border-radius: 5px; padding:10px; color:aliceblue;' class='content-body'>
            <div class='content-sub-body'>
                <div class='header'>
                    <h1 style='padding-top:14px; margin-top: 0px; color:aliceblue'>Just Quality HR</h1>
                </div>
                <div class='info'>
                    <label style='padding-top:50px;'>Hola Sr/a. {usuario.Nombre} {usuario.Apellido},</label>
                    <p style='padding-top:30px;'>Su contraseña para poder ingresar al sistema de reclutamiento es:</p>
                    <h2 style='padding-top:30px; color:aliceblue'>{usuario.Contraseña}</h2>
                    <p style='padding-top:30px;'>
                        NOTA: Una vez haya iniciado session por primera vez, deberas cambiar tu clave de acceso para que tengas mayor seguridad
                        de los datos que manejas en la aplicacion de reclutamiento.
                    </p>
                    <p style='padding-top:30px;'>Saludos cordiales,</p>
                    <p>Exitos. Un abrazo.</p>
                </div>
    
                <div class='fhotter'>
                </div>
            </div>
        </div>
    </div>";

            MailMessage correo = new MailMessage();
            correo.From = new MailAddress("jq.hr.system@gmail.com", "JQHR System", System.Text.Encoding.UTF8);
            correo.To.Add(usuario.Correo);
            correo.Subject = "Confirmacion de registro";
            correo.Body = $"{sub}";
            correo.IsBodyHtml = true;
            correo.Priority = MailPriority.Normal;
            SmtpClient smtp = new SmtpClient();
            smtp.UseDefaultCredentials = false;
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;//True si el servidor de correo permite ssl
            smtp.Credentials = new System.Net.NetworkCredential("jq.hr.system@gmail.com", "Jqrh4321");//Cuenta de correo
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            smtp.Send(correo);
            return true;

        }
    }

    public class ResponseUser
    {
        public bool ok { get; set; }
        public string mensaje { get; set; }
    }

    public class ResponseMessageCountCantidaVC
    {
        public bool ok { get; set; }
        public int cantidadVacantes { get; set; }
        public int CantidadCandidatos { get; set; }
    }

    public class ResponsecantidadTotal_UCV
    { 
        public bool ok { get; set; }
        public int cantidadTVacantes { get; set; }
        public int CantidadTCandidatos { get; set; }
        public int CantidadTUsuarios { get; set; }
    }

    public class CambioContraseña
    {
        public int Estado { get; set; }
        public int idUsuario { get; set; }
        public string viejaContraseña { get; set; }
        public string nuevaContraseña { get; set; }
        public string rnuevaContraseña { get; set; }
    }
}
