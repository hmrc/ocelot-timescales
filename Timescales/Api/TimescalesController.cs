using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}