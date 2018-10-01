using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Timescales.Models;

namespace Timescales.Controllers
{    
    public class TimescalesBusinessController : Controller
    {
        private readonly Context _context;
        private readonly ILogger<TimescalesBusinessController> _logger;

        public TimescalesBusinessController(Context context, ILogger<TimescalesBusinessController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: TimescalesBusiness       
        public IActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PlaceholderSortParm = sortOrder == "Placeholder" ? "placeholder_desc" : "Placeholder";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "description_desc" : "Description";
            ViewBag.LineOfBusinessParm = sortOrder == "LineOfBusiness" ? "lineOfBusiness_desc" : "LineOfBusiness";

            var timescales = from t in _context.Timescales
                             select t;

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
            return View(timescales.ToList());
        }


        // GET: TimescalesBusiness/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timescale = await _context.Timescales
                .FirstOrDefaultAsync(m => m.Id == id);
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

            var timescale = await _context.Timescales.FindAsync(id);
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
                    _context.Update(timescale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimescaleExists(timescale.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(timescale);
        }  

        private bool TimescaleExists(Guid id)
        {
            return _context.Timescales.Any(e => e.Id == id);
        }
    }
}
