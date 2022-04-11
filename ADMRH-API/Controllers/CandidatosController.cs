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
    public class CandidatosController : ControllerBase
    {
        private readonly ADMRHJQContext _context;

        public CandidatosController(ADMRHJQContext context)
        {
            _context = context;
        }

        // GET: api/Candidatos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidato>>> GetCandidatos()
        {
            return await _context.Candidatos.ToListAsync();
        }

        // GET: api/Candidatos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseC>> GetCandidato(int id)
        {
            var candidato = await _context.Candidatos.FindAsync(id);

            if (candidato == null)
            {
                return new ResponseC()
                {
                    ok = false,
                    message ="El candidato no existe..."
                };
            }

             return new ResponseC()
            {
                ok = true,
                candidatos = candidato
            }; 
        }

        [HttpGet("Cedula/{cedula}")]
        public async Task<ActionResult<List<Candidato>>> GetCandidatoCedula(string cedula)
        {
            var candidato = await _context.Candidatos.Where(x => x.Cedula == cedula).Select(x => x).ToListAsync();

            if (candidato == null)
            {
                return NotFound();
            }

            return candidato;
        }

        // PUT: api/Candidatos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ResponseC> PutCandidato(int id, Candidato candidato)
        {
            if (id != candidato.IdCandidato)
            {
                return new ResponseC()
                {
                    ok = false,
                    message = "Ocurrio un error al actualizar los datos..."
                };
            }

            _context.Entry(candidato).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return new ResponseC()
                {
                    ok = true,
                    message = "El candidato a sido actualizado con exito..."
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidatoExists(id))
                {
                    return new ResponseC()
                    {
                        ok = false,
                        message = "El candidato no existe..."
                    };
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Candidatos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ResponseC>> PostCandidato(Candidato candidato)
        {
            try
            {
                _context.Candidatos.Add(candidato);
                await _context.SaveChangesAsync();
                enviarCorreo(candidato);
                return new ResponseC()
                {
                    ok = true,
                    message = "Los datos se guardaron correctamente..."
                };
            }
            catch (Exception)
            {
                return new ResponseC()
                {
                    ok = true,
                    message = "Ocurrio un error al insertar los datos..."
                };
            }
        }

        // DELETE: api/Candidatos/5
        [HttpDelete("{id}")]
        public async Task<ResponseC> DeleteCandidato(int id)
        {
            var candidato = await _context.Candidatos.FindAsync(id);
            if (candidato == null)
            {
                 return new ResponseC()
                {
                    ok = false,
                    message = "El candidato no existe..."
                };
            }

            _context.Candidatos.Remove(candidato);
            await _context.SaveChangesAsync();

            return new ResponseC()
            {
                ok = true,
                message = "El candidato a sido eliminado..."
            };
        }

        private bool CandidatoExists(int id)
        {
            return _context.Candidatos.Any(e => e.IdCandidato == id);
        }
        private bool enviarCorreo(Candidato candidato)
        {
            
            string sub = @$"<div class='container'>
        <br />
        <div style='background: #023877; border-radius: 5px; padding:10px; color:aliceblue;' class='content-body'>
            <div class='content-sub-body'>
                <div class='header'>
                    <h1 style='padding-top:14px; margin-top: 0px; color:aliceblue'>Just Quality HR</h1>
                </div>
                <div class='info'>
                    <label style='padding-top:50px;'>Hola Sr/a. {candidato.Nombre} {candidato.Apellido},</label>
                    <p style='padding-top:30px;'>Gracias por ver registrado sus datos en nuestra plataforma de solicitud de empleos.</p>
                    <p style='padding-top:5px;'>
                      Mas adelante nos estaremos poniendo en contacto con usted ante cualquier novedad. Cualquier inquietud puede 
                      comunicarse con nosotros al siguiente número.
                    </p>
                   <h4 style='padding-top:10px; color:aliceblue'>809-789-5363</h4>
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
            correo.To.Add(candidato.Correo);
            correo.Subject = "Confirmacion de solicitud de empleo";
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
   public class ResponseC
    {
        public bool ok { get; set; }
        public string message { get; set; }
        public Candidato? candidatos { get; set; }
    }
}
