using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Timescales.Controllers.Helpers.Interfaces;
using Timescales.Models;

namespace Timescales.Controllers
{
    public class TimescalesAuthorController : Controller
    {        
        private readonly ILogger<TimescalesAuthorController> _logger;        
        private readonly IPublishHandler _publishHandler;
        private readonly ILegacyPublishHandler _legacyPublishHandler;
        private readonly IAuthHandler _authHandler;
        private readonly IAuditDataHandler _auditDataHandler;
        private readonly ITimescaleDataHandler _timescaleDataHandler;

        public TimescalesAuthorController(ILogger<TimescalesAuthorController> logger,                                              
                                            IPublishHandler publishHandler,
                                            ILegacyPublishHandler legacyPublishHandler,
                                            IAuthHandler authHandler,
                                            IAuditDataHandler auditDataHandler,
                                            ITimescaleDataHandler timescaleDataHandler)
        {            
            _logger = logger;            
            _publishHandler = publishHandler;
            _legacyPublishHandler = legacyPublishHandler;
            _authHandler = authHandler;
            _auditDataHandler = auditDataHandler;
            _timescaleDataHandler = timescaleDataHandler;
        }

        // GET: TimescalesAuthor  
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PlaceholderSortParm = sortOrder == "Placeholder" ? "placeholder_desc" : "Placeholder";           
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "description_desc" : "Description";       
            ViewBag.LineOfBusinessParm = sortOrder == "LineOfBusiness" ? "lineOfBusiness_desc" : "LineOfBusiness";           

            var timescales = await _timescaleDataHandler.GetMany();

            if (!String.IsNullOrEmpty(searchString))
            {
                timescales = timescales.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()) ||
                                                   s.Description.ToUpper().Contains(searchString.ToUpper()) ||
                                                   s.Placeholder.ToUpper().Contains(searchString.ToUpper()) ||
                                                   s.LineOfBusiness.ToUpper().Contains(searchString.ToUpper())
                                                   );
            }

            switch (sortOrder)
            {
                case "name_desc":
                    timescales = timescales.OrderByDescending(t => t.Name);
                    break;
                case "Placeholder":
                    timescales = timescales.OrderBy(t => t.Placeholder);
                    break;
                case "placeholder_desc":
                    timescales = timescales.OrderByDescending(t => t.Placeholder);
                    break;                
                case "Description":
                    timescales = timescales.OrderBy(t => t.Description);
                    break;
                case "description_desc":
                    timescales = timescales.OrderByDescending(t => t.Description);
                    break; 
                case "LineOfBusiness":
                    timescales = timescales.OrderBy(t => t.LineOfBusiness);
                    break;
                case "lineOfBusiness_desc":
                    timescales = timescales.OrderByDescending(t => t.LineOfBusiness);
                    break;               
                default:
                    timescales = timescales.OrderBy(t => t.Name);
                    break;
            }

            return View(timescales);
        }

        // GET: TimescalesAuthor/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timescale = await _timescaleDataHandler.Get(id);

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
            if (!await _authHandler.IsAuthedRole(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
            {
                ViewBag.UserMessage = "You are not authorised to create a timescale.";

                return View(timescale);
            }

            if (ModelState.IsValid)
            {
                timescale.Id = Guid.NewGuid();
                timescale.UpdatedDate = DateTime.Now;

                await _timescaleDataHandler.Post(timescale);              
                await _auditDataHandler.Post("Create", timescale, @User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1));
                await _publishHandler.Publish();
                await _legacyPublishHandler.Publish(timescale.LineOfBusiness);

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

            var timescale = await _timescaleDataHandler.Get(id);

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
            else if (!await _authHandler.IsAuthedRole(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
            {
                ViewBag.UserMessage = "You are not authorised to edit this timescale.";

                return View(timescale);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    timescale.UpdatedDate = DateTime.Now;

                    await _timescaleDataHandler.Put(timescale);                  
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _timescaleDataHandler.Exists(timescale.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                await _auditDataHandler.Post("Edit", timescale, @User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1));
                await _publishHandler.Publish();
                await _legacyPublishHandler.Publish(timescale.LineOfBusiness);

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

            var timescale = await _timescaleDataHandler.Get(id);

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
            var timescale = await _timescaleDataHandler.Get(id);

            if (!await _authHandler.IsAuthedRole(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
            {
                ViewBag.UserMessage = "You are not authorised to delete this timescale.";

                return View(timescale);
            }

            await _timescaleDataHandler.Delete(timescale);     
            await _publishHandler.Publish();
            await _legacyPublishHandler.Publish(timescale.LineOfBusiness);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Audit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timescale = await _timescaleDataHandler.GetIncludeChildObjects(id);

            if (timescale == null)
            {
                return NotFound();
            }

            return View(timescale);
        }
    }
}
