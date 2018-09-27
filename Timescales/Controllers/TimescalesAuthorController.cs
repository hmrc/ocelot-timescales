using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Timescales.Models;

namespace Timescales.Controllers
{
    public class TimescalesAuthorController : Controller
    {
        private readonly Context _context;
        private readonly ILogger<TimescalesAuthorController> _logger;

        public TimescalesAuthorController(Context context, ILogger<TimescalesAuthorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: TimescalesAuthor  
        public IActionResult Index(string sortOrder, string searchString)
        {           
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PlaceholderSortParm = sortOrder == "Placeholder" ? "placeholder_desc" : "Placeholder";
            ViewBag.UpdatedDateSortParm = sortOrder == "UpdatedDate" ? "updatedDate_desc" : "UpdatedDate";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "description_desc" : "Description";
            ViewBag.OwnersSortParm = sortOrder == "Owners" ? "owners_desc" : "Owners";
            ViewBag.BasisSortParm = sortOrder == "Basis" ? "basis_desc" : "Basis";
            ViewBag.OldestWorkDateSortParm = sortOrder == "OldestWorkDate" ? "oldestWorkDate_desc" : "OldestWorkDate";
            ViewBag.DaysSortParm = sortOrder == "Days" ? "days_desc" : "Days";


            var timescales = from t in _context.Timescales
                           select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                timescales = timescales.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()) ||
                                                   s.Description.ToUpper().Contains(searchString.ToUpper()) ||
                                                   s.Placeholder.ToUpper().Contains(searchString.ToUpper())
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
                case "UpdatedDate":
                    timescales = timescales.OrderBy(t => t.UpdatedDate);
                    break;
                case "updatedDate_desc":
                    timescales = timescales.OrderByDescending(t => t.UpdatedDate);
                    break;
                case "Description":
                    timescales = timescales.OrderBy(t => t.Description);
                    break;
                case "description_desc":
                    timescales = timescales.OrderByDescending(t => t.Description);
                    break;
                case "Owners":
                    timescales = timescales.OrderBy(t => t.Owners);
                    break;
                case "owners_desc":
                    timescales = timescales.OrderByDescending(t => t.Owners);
                    break;
                case "Basis":
                    timescales = timescales.OrderBy(t => t.Basis);
                    break;
                case "basis_desc":
                    timescales = timescales.OrderByDescending(t => t.Basis);
                    break;
                case "OldestWorkDate":
                    timescales = timescales.OrderBy(t => t.OldestWorkDate);
                    break;
                case "oldestWorkDate_desc":
                    timescales = timescales.OrderByDescending(t => t.OldestWorkDate);
                    break;
                case "Days":
                    timescales = timescales.OrderBy(t => t.Days);
                    break;
                case "days_desc":
                    timescales = timescales.OrderByDescending(t => t.Days);
                    break;
                default:
                    timescales = timescales.OrderBy(t => t.Name);
                    break;
            }
            return View(timescales.ToList());
        }

        // GET: TimescalesAuthor/Details/5
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
        public async Task<IActionResult> Create([Bind("Id,Placeholder,Name,Description,Owners,OldestWorkDate,Days,Basis")] Timescale timescale)
        {
            if (ModelState.IsValid)
            {
                timescale.Id = Guid.NewGuid();
                timescale.UpdatedDate = DateTime.Now;
                _context.Add(timescale);
                await _context.SaveChangesAsync();
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

            var timescale = await _context.Timescales.FindAsync(id);
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Placeholder,Name,Description,Owners,OldestWorkDate,Days,Basis")] Timescale timescale)
        {
            if (id != timescale.Id)
            {
                return NotFound();
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

        // GET: TimescalesAuthor/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
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

        // POST: TimescalesAuthor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var timescale = await _context.Timescales.FindAsync(id);
            _context.Timescales.Remove(timescale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimescaleExists(Guid id)
        {
            return _context.Timescales.Any(e => e.Id == id);
        }
    }
}
