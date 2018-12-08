using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Timescales.Controllers.Helpers;
using Timescales.Interfaces;
using Timescales.Models;

namespace Timescales.Controllers
{
    public class TimescalesBusinessController : Controller
    {
        private readonly ILogger<TimescalesBusinessController> _logger;        
        private readonly IPublishRepository _publishRepository;
        private readonly ILegacyPublishRepository _legacyPublishRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly ITimescaleRepository _timescaleRepository;

        public TimescalesBusinessController(ILogger<TimescalesBusinessController> logger,                                                 
                                            IPublishRepository publishRepository,
                                            ILegacyPublishRepository legacyPublishRepository,
                                            IAuditRepository auditRepository,
                                            ITimescaleRepository timescaleRepository)
        {
            _logger = logger;
            _publishRepository = publishRepository;
            _legacyPublishRepository = legacyPublishRepository;
            _auditRepository = auditRepository;
            _timescaleRepository = timescaleRepository;
        }

        // GET: TimescalesBusiness       
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? page)
        {
            Expression<Func<Timescale, bool>> where = s => s.Owners.Contains(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1));
            Expression<Func<Timescale, string>> orderby = t => t.Name;
            var ascending = true;
            
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                where = s => (s.Name.ToUpper().Contains(searchString.ToUpper()) ||
                              s.Placeholder.ToUpper().Contains(searchString.ToUpper()) ||
                              s.LineOfBusiness.ToUpper().Contains(searchString.ToUpper())
                              ) && 
                              s.Owners.Contains(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1));
            }

            int pageSize = 20;            

            try
            {
                var timescales = _timescaleRepository.GetMany(where, orderby, ascending);

                return View(await PaginatedList<Timescale>.CreateAsync(timescales.AsNoTracking(), page ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(500, ex.Message, ex);

                return StatusCode(500, ex.Message);
            }
        }
        
        // GET: TimescalesBusiness/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }            

            try
            {
                var timescale = await _timescaleRepository.Get(id);

                if (timescale == null)
                {
                    return NotFound();
                }

                return View(timescale);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(500, ex.Message, ex);

                return StatusCode(500, ex.Message);
            }
        }       

        // GET: TimescalesBusiness/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var timescale = await _timescaleRepository.Get(id);

                if (timescale == null)
                {
                    return NotFound();
                }
           
                return View(timescale);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(500, ex.Message, ex);

                return StatusCode(500, ex.Message);
            }
        }

        // POST: TimescalesBusiness/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Placeholder,Name,Owners,OldestWorkDate,Days,Basis,LineOfBusiness")] Timescale timescale)
        {
            if (id != timescale.Id)
            {
                return NotFound();
            }
            else if (!@User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            else if (!ModelState.IsValid)
            {
                return View(timescale);
            }
            else if (!timescale.Owners.Contains(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
            {
                ViewBag.UserMessage = "You are not authorised to edit this timescale.";

                return View(timescale);
            }

            timescale.UpdatedDate = DateTime.Now;

            try
            {     
                await _timescaleRepository.Put(timescale);
                await _auditRepository.Post("Edit", timescale, @User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await _timescaleRepository.Exists(timescale.Id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogCritical(500, ex.Message, ex);
                    return StatusCode(500, ex.Message);
                }
            }

            try
            {
                await _publishRepository.Publish();
                await _legacyPublishRepository.Publish(timescale.LineOfBusiness);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(500, ex.Message, ex);

                return StatusCode(500, ex.Message);
            }
        } 
    }
}
