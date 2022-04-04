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

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }

        private bool enviarCorreo(Usuario usuario)
        {
            string boddys = @" <html lang='en'><body> <style>
        .container{
            width: 100%; 
            height: 700px; 
            background: #1c57b9; 
            padding: 0px; 
            margin: 0px; 
            color: aliceblue;
        }
       .container .content-body{
        width: 40%; 
        height: 600px; 
        margin: auto;
        background: #1c57b9; 
        border: solid 2px #45536b; 
        border-radius:5px; margin-top: 2.1%; 
       }
       .content-body .content-sub-body{
        height: 600px; 
        width: 100%; 
       }
       .content-sub-body .header{
        width: 100%; 
        height: 70px; 
        margin: auto; 
        background: #023877; 
        border-top-left-radius:5px; 
        border-top-right-radius:5px;
       }
       .content-sub-body .fhotter{
        height: 70px; 
        width: 100%; 
        background: #023877; 
        margin: auto;
        border-bottom-left-radius:5px; 
        border-bottom-right-radius:5px;
       }
      .container .content-sub-body .info{
        width:90%; 
        height:440px; 
        padding:10px;
        margin: auto;
        text-align: justify;
        word-wrap: break-word;
       }
       @media only screen and (max-width: 768px) {
        .container.content - body{
            width: 90%;
            word-wrap: break-word;
        }
       }
    </style>
";
            string sub = @$"{boddys}<div class='container'>
        <br />
        <div style='background: #1c57b9; border-radius: 5px; padding:10px; color:aliceblue;' class='content-body'>
            <div class='content-sub-body'>
                <div class='header'>
                    <h1 style='padding-top:14px; margin-top: 0px; color:aliceblue'>JustQuallity</h1>
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
    </div> </body>
</html>";

            MailMessage correo = new MailMessage();
            correo.From = new MailAddress("for.foot.fusion@gmail.com", "for foot", System.Text.Encoding.UTF8);
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
            smtp.Credentials = new System.Net.NetworkCredential("for.foot.fusion@gmail.com", "Forfoot9550");//Cuenta de correo
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
}
