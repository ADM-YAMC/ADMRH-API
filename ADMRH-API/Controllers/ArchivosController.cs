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
    public class ArchivosController : ControllerBase
    {
        private readonly ADMRHJQContext _context;

        public ArchivosController(ADMRHJQContext context)
        {
            _context = context;
        }

        // GET: api/Archivos
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<RootArchivo>>> GetArchivos()
        //{
        //    var response = new RootArchivo()
        //    {
        //        ok = true,
        //        archivos = await _context.Archivos.ToListAsync()
        //    };

        //    return Ok(response);
        //}

        // GET: api/Archivos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Archivo>> GetArchivo(int id)
        {
            var archivo = await _context.Archivos.FindAsync(id);

            if (archivo == null)
            {
                return NotFound();
            }

            return archivo;
        }

        // PUT: api/Archivos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Archivo>> PutArchivo(int id, Archivo archivo)
        {
            if (id != archivo.IdArchivos)
            {
                return BadRequest();
            }

            _context.Entry(archivo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                var archivoA = await _context.Archivos.FindAsync(id);
                return archivoA;

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArchivoExists(id))
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

        // POST: api/Archivos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Archivo>> PostArchivo(Archivo archivo)
        {
            _context.Archivos.Add(archivo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArchivo", new { id = archivo.IdArchivos }, archivo);
        }

        // DELETE: api/Archivos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArchivo(int id)
        {
            var archivo = await _context.Archivos.FindAsync(id);
            if (archivo == null)
            {
                return NotFound();
            }

            _context.Archivos.Remove(archivo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArchivoExists(int id)
        {
            return _context.Archivos.Any(e => e.IdArchivos == id);
        }
    }
}
