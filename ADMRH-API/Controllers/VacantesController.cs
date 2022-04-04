using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ADMRH_API.Models;

namespace ADMRH_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacantesController : ControllerBase
    {
        private readonly ADMRHJQContext _context;

        public VacantesController(ADMRHJQContext context)
        {
            _context = context;
        }

        // GET: api/Vacantes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Response>>> GetVacantes()
        {
            return Ok(new Response()
            {
                ok = true,
                vacante = await _context.Vacantes.ToListAsync()
            });
        }

        // GET: api/Vacantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetVacante(int id)
        {
            var vacante = await _context.Vacantes.FindAsync(id);

            if (vacante == null)
            {
               return Ok(new Response()
                {
                    ok = true,
                    mensaje = "El usuario no existe..."
                });
            }

            return Ok(new Response()
            {
                ok = true,
                vacante = new List<Vacante>(){vacante}
            });
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<Response>> GetVacanteForUser(int id)
        {
            var vacante = await _context.Vacantes.Where(x => x.IdUsuarioCreacion == id).Select(x => x).ToListAsync();

            return Ok(new Response()
            {
                ok = true,
                vacante = vacante
            });
        }


        // PUT: api/Vacantes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVacante(int id, Vacante vacante)
        {
            if (id != vacante.IdVacante)
            {
                return Ok(new Response()
                {
                    ok = false,
                    mensaje = "La vacante ha actualizar no existe..."
                });
            }
            _context.Entry(vacante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new Response()
                {
                    ok = true,
                    mensaje = "Vacante actualizada con exito..."
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacanteExists(id))
                {
                    return Ok(new Response()
                    {
                        ok = false,
                        mensaje = "Ocurrio un error..."
                    });
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Vacantes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPost]
        public async Task<ActionResult<Response>> PostVacante(Vacante vacante)
        {
            _context.Vacantes.Add(vacante);
            await _context.SaveChangesAsync();

            return Ok(new Response()
            {
                ok = true,
                mensaje = "Vacante registrda con exito..."
            });

        }

        // DELETE: api/Vacantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVacante(int id)
        {
            var vacante = await _context.Vacantes.FindAsync(id);
            if (vacante == null)
            {
               return Ok(new Response()
                {
                    ok = false,
                    mensaje = "Ocurrio un error al eliminar la vacante..."
                });
            }

            _context.Vacantes.Remove(vacante);
            await _context.SaveChangesAsync();

            return Ok(new Response()
            {
                ok = true,
                mensaje = "Vacante eliminada con exito..."
            });
        }

        private bool VacanteExists(int id)
        {
            return _context.Vacantes.Any(e => e.IdVacante == id);
        }
    }

    public class Response
    {
        public bool ok { get; set; }
        public List<Vacante> vacante { get; set; }
        public string? mensaje { get; set; }
    }
}
