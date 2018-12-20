﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timescales.Interfaces;
using Timescales.Models;

namespace Timescales.Api
{
    [Route("[controller]")]
    [ApiController]
    public class TimescalesDataController : ControllerBase
    {
        private readonly ILogger<TimescalesDataController> _logger;
        private readonly ITimescaleRepository _timescaleRepository;

        public TimescalesDataController(ILogger<TimescalesDataController> logger,
                                    ITimescaleRepository timescaleRepository)
        {
            _logger = logger;
            _timescaleRepository = timescaleRepository;
        }
        
        // GET: api/Timescales      
        [HttpGet]   
        public async Task<IEnumerable<Timescale>> GetTimescales()
        {
            return await _timescaleRepository.GetMany();
        }

        // GET: api/Timescales/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTimescale([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var timescale = await _timescaleRepository.Get(t => t.Id == id);

            if (timescale == null)
            {
                return NotFound();
            }

            return Ok(timescale);
        } 
    }
}