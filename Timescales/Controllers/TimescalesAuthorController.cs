using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Timescales.Models;

namespace Timescales.Controllers
{
    public class TimescalesAuthorController : Controller
    {
        private readonly Context _context;

        public TimescalesAuthorController(Context context)
        {
            _context = context;
        }

        // GET: TimescalesAuthor
        public async Task<IActionResult> Index()
        {
            return View(await _context.Timescales.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Owners,OldestWorkDate,Days,Basis")] Timescale timescale)
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Description,Owners,OldestWorkDate,Days,Basis")] Timescale timescale)
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
