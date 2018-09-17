using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timescales.Models;

namespace Timescales.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimescalesController : ControllerBase
    {
        private readonly Context _context;

        public TimescalesController(Context context)
        {
            _context = context;
        }

        // GET: api/Timescales
        [HttpGet]
        public IEnumerable<Timescale> GetTimescales()
        {
            return _context.Timescales;
        }

        // GET: api/Timescales/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTimescale([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var timescale = await _context.Timescales.FindAsync(id);

            if (timescale == null)
            {
                return NotFound();
            }

            return Ok(timescale);
        }

        // PUT: api/Timescales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimescale([FromRoute] Guid id, [FromBody] Timescale timescale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != timescale.Id)
            {
                return BadRequest();
            }

            timescale.UpdatedDate = DateTime.Now;
            _context.Entry(timescale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimescaleExists(id))
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

        // POST: api/Timescales
        [HttpPost]
        public async Task<IActionResult> PostTimescale([FromBody] Timescale timescale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            timescale.UpdatedDate = DateTime.Now;
            _context.Timescales.Add(timescale);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTimescale", new { id = timescale.Id }, timescale);
        }

        // DELETE: api/Timescales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimescale([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var timescale = await _context.Timescales.FindAsync(id);
            if (timescale == null)
            {
                return NotFound();
            }

            _context.Timescales.Remove(timescale);
            await _context.SaveChangesAsync();

            return Ok(timescale);
        }

        private bool TimescaleExists(Guid id)
        {
            return _context.Timescales.Any(e => e.Id == id);
        }
    }
}