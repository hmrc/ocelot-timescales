using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Timescales.Controllers.Helpers.Interfaces;
using Timescales.Models;

namespace Timescales.Controllers
{
    public class TimescalesBusinessController : Controller
    {
        private readonly ILogger<TimescalesBusinessController> _logger;        
        private readonly IPublishHandler _publishHandler;
        private readonly ILegacyPublishHandler _legacyPublishHandler;
        private readonly IAuditDataHandler _auditDataHandler;
        private readonly ITimescaleDataHandler _timescaleDataHandler;

        public TimescalesBusinessController(ILogger<TimescalesBusinessController> logger,                                                 
                                                IPublishHandler publishHandler,
                                                ILegacyPublishHandler legacyPublishHandler,
                                                IAuditDataHandler auditDataHandler,
                                                ITimescaleDataHandler timescaleDataHandler)
        {
            _logger = logger;            
            _publishHandler = publishHandler;
            _legacyPublishHandler = legacyPublishHandler;
            _auditDataHandler = auditDataHandler;
            _timescaleDataHandler = timescaleDataHandler;
        }

        // GET: TimescalesBusiness       
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PlaceholderSortParm = sortOrder == "Placeholder" ? "placeholder_desc" : "Placeholder";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "description_desc" : "Description";
            ViewBag.LineOfBusinessParm = sortOrder == "LineOfBusiness" ? "lineOfBusiness_desc" : "LineOfBusiness";

            Expression<Func<Timescale, bool>> where;
            Expression<Func<Timescale, string>> orderby;
            var ascending = true;

            if (sortOrder != null)
            {
                if (sortOrder.Contains("_desc"))
                {
                    ascending = false;
                }
            }                                                         

            if (!String.IsNullOrEmpty(searchString))
            {
                where = s => (s.Name.ToUpper().Contains(searchString.ToUpper()) ||
                                                   s.Description.ToUpper().Contains(searchString.ToUpper()) ||
                                                   s.Placeholder.ToUpper().Contains(searchString.ToUpper()) ||
                                                   s.LineOfBusiness.ToUpper().Contains(searchString.ToUpper())
                                                   ) && s.Owners.Contains(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1));
            }
            else
            {
                where = s => s.Owners.Contains(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    orderby = t => t.Name;
                    break;
                case "Placeholder":
                    orderby = t => t.Placeholder;
                    break;
                case "placeholder_desc":
                    orderby = t => t.Placeholder;
                    break;
                case "Description":
                    orderby = t => t.Description;
                    break;
                case "description_desc":
                    orderby = t => t.Description;
                    break;
                case "LineOfBusiness":
                    orderby = t => t.LineOfBusiness;
                    break;
                case "lineOfBusiness_desc":
                    orderby = t => t.LineOfBusiness;
                    break;
                default:
                    orderby = t => t.Name;
                    break;
            }

            return View(await _timescaleDataHandler.GetMany(where, orderby, ascending));
        }
        
        // GET: TimescalesBusiness/Details/5
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

        // GET: TimescalesBusiness/Edit/5
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

        // POST: TimescalesBusiness/Edit/5
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
            else if (!timescale.Owners.Contains(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
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
    }
}
