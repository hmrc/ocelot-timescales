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
    public class TimescalesAuthorController : Controller
    {
        private readonly ILogger<TimescalesAuthorController> _logger;        
        private readonly IPublishRepository _publishRepository;
        private readonly ILegacyPublishRepository _legacyPublishRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly ITimescaleRepository _timescaleRepository;

        public TimescalesAuthorController(ILogger<TimescalesAuthorController> logger,                                              
                                          IPublishRepository publishRepository,
                                          ILegacyPublishRepository legacyPublishRepository,
                                          IAuthRepository authRepository,
                                          IAuditRepository auditRepository,
                                          ITimescaleRepository timescaleRepository)
        {            
            _logger = logger;
            _publishRepository = publishRepository;
            _legacyPublishRepository = legacyPublishRepository;
            _authRepository = authRepository;
            _auditRepository = auditRepository;
            _timescaleRepository = timescaleRepository;
        }

        // GET: TimescalesAuthor  
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? page)
        { 
            Expression<Func<Timescale, bool>> where = s => s.Id != null;
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
                where = s => s.Name.ToUpper().Contains(searchString.ToUpper()) ||
                             s.Placeholder.ToUpper().Contains(searchString.ToUpper()) ||
                             s.Site.ToUpper().Contains(searchString.ToUpper()) ||
                             s.LineOfBusiness.ToUpper().Contains(searchString.ToUpper());
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

        // GET: TimescalesAuthor/Details/5
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

        // GET: TimescalesAuthor/Create
        public IActionResult Create()
        {   
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(500, ex.Message, ex);

                return StatusCode(500, ex.Message);
            }
        }

        // POST: TimescalesAuthor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Placeholder,Name,Description,Owners,OldestWorkDate,Days,Basis,Site,LineOfBusiness")] Timescale timescale)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            else if (!ModelState.IsValid)
            {
                return View(timescale);
            }
            else if (!await _authRepository.IsAuthedRole(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
            {
                ViewBag.UserMessage = "You are not authorised to create a timescale.";

                return View(timescale);
            }

                timescale.Id = Guid.NewGuid();
                timescale.UpdatedDate = DateTime.Now;

            try
            {
                await _timescaleRepository.Post(timescale);
                await _auditRepository.Post("Create", timescale, @User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1));
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

        // GET: TimescalesAuthor/Edit/5
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

        // POST: TimescalesAuthor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Placeholder,Name,Description,Owners,OldestWorkDate,Days,Basis,Site,LineOfBusiness")] Timescale timescale)
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
            else if (!await _authRepository.IsAuthedRole(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
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

        // GET: TimescalesAuthor/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
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

        // POST: TimescalesAuthor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Timescale timescale;

            try
            {
                timescale = await _timescaleRepository.Get(id);

                if (timescale == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(500, ex.Message, ex);

                return StatusCode(500, ex.Message);
            }

            if (!await _authRepository.IsAuthedRole(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
            {
                ViewBag.UserMessage = "You are not authorised to delete this timescale.";

                return View(timescale);
            }            

            try
            {
                await _timescaleRepository.Delete(timescale);
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

        public async Task<IActionResult> Audit(Guid? id)
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
    }
}
