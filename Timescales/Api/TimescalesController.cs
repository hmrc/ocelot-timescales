using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timescales.Controllers.Helpers.Interfaces;
using Timescales.Models;

namespace Timescales.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimescalesController : ControllerBase
    {
        private readonly ILogger<TimescalesController> _logger;
        private readonly ITimescaleDataHandler _timescaleDataHandler;

        public TimescalesController(ILogger<TimescalesController> logger,
                                            ITimescaleDataHandler timescaleDataHandler)
        {
            _logger = logger;
            _timescaleDataHandler = timescaleDataHandler;
        }
        
        // GET: api/Timescales      
        [HttpGet]   
        public async Task<IEnumerable<Timescale>> GetTimescales()
        {
            return await _timescaleDataHandler.GetMany();
        }

        // GET: api/Timescales/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTimescale([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var timescale = await _timescaleDataHandler.Get(id);

            if (timescale == null)
            {
                return NotFound();
            }

            return Ok(timescale);
        } 
    }
}