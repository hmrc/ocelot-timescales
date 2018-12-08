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
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PlaceholderSortParm"] = sortOrder == "Placeholder" ? "placeholder_desc" : "Placeholder";
            ViewData["DescriptionSortParm"] = sortOrder == "Description" ? "description_desc" : "Description";
            ViewData["LineOfBusinessParm"] = sortOrder == "LineOfBusiness" ? "lineOfBusiness_desc" : "LineOfBusiness";

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

            if (sortOrder != null)
            {
                if (sortOrder.Contains("_desc"))
                {
                    ascending = false;
                }
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                where = s => s.Name.ToUpper().Contains(searchString.ToUpper()) ||
                             s.Description.ToUpper().Contains(searchString.ToUpper()) ||
                             s.Placeholder.ToUpper().Contains(searchString.ToUpper()) ||
                             s.LineOfBusiness.ToUpper().Contains(searchString.ToUpper());
            }

            switch (sortOrder)
            {
                case string val when val.ToLower().Contains("placeholder"):
                    orderby = t => t.Placeholder;
                    break;
                case string val when val.ToLower().Contains("description"):
                    orderby = t => t.Description;
                    break;
                case string val when val.ToLower().Contains("lineofbusiness"):
                    orderby = t => t.LineOfBusiness;
                    break;   
            }

            int pageSize = 20;
            var timescales = _timescaleRepository.GetMany(where, orderby, ascending);

            return View(await PaginatedList<Timescale>.CreateAsync(timescales.AsNoTracking(), page ?? 1, pageSize));          
        }

        // GET: TimescalesAuthor/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timescale = await _timescaleRepository.Get(id);

            if (timescale == null)
            {
                return NotFound();
            }

            return View(timescale);
        }

        // GET: TimescalesAuthor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TimescalesAuthor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Placeholder,Name,Description,Owners,OldestWorkDate,Days,Basis,LineOfBusiness")] Timescale timescale)
        {
            if (!await _authRepository.IsAuthedRole(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
            {
                ViewBag.UserMessage = "You are not authorised to create a timescale.";

                return View(timescale);
            }

            if (ModelState.IsValid)
            {
                timescale.Id = Guid.NewGuid();
                timescale.UpdatedDate = DateTime.Now;

                await _timescaleRepository.Post(timescale);              
                await _auditRepository.Post("Create", timescale, @User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1));
                await _publishRepository.Publish();
                await _legacyPublishRepository.Publish(timescale.LineOfBusiness);

                return RedirectToAction(nameof(Index));
            }

            return View(timescale);
        }

        // GET: TimescalesAuthor/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timescale = await _timescaleRepository.Get(id);

            if (timescale == null)
            {
                return NotFound();
            }

            return View(timescale);
        }

        // POST: TimescalesAuthor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Placeholder,Name,Description,Owners,OldestWorkDate,Days,Basis,LineOfBusiness")] Timescale timescale)
        {
            if (id != timescale.Id)
            {
                return NotFound();
            }
            else if (!await _authRepository.IsAuthedRole(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
            {
                ViewBag.UserMessage = "You are not authorised to edit this timescale.";

                return View(timescale);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    timescale.UpdatedDate = DateTime.Now;

                    await _timescaleRepository.Put(timescale);                  
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _timescaleRepository.Exists(timescale.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                await _auditRepository.Post("Edit", timescale, @User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1));
                await _publishRepository.Publish();
                await _legacyPublishRepository.Publish(timescale.LineOfBusiness);

                return RedirectToAction(nameof(Index));
            }

            return View(timescale);
        }

        // GET: TimescalesAuthor/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timescale = await _timescaleRepository.Get(id);

            if (timescale == null)
            {
                return NotFound();
            }

            return View(timescale);
        }

        // POST: TimescalesAuthor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var timescale = await _timescaleRepository.Get(id);

            if (!await _authRepository.IsAuthedRole(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
            {
                ViewBag.UserMessage = "You are not authorised to delete this timescale.";

                return View(timescale);
            }

            await _timescaleRepository.Delete(timescale);     
            await _publishRepository.Publish();
            await _legacyPublishRepository.Publish(timescale.LineOfBusiness);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Audit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timescale = await _timescaleRepository.Get(id);

            if (timescale == null)
            {
                return NotFound();
            }

            return View(timescale);
        }
    }
}
