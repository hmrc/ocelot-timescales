using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Timescales.Controllers.Helpers.Interfaces;
using Timescales.Models;

namespace Timescales.Controllers
{
    public class TimescalesAuthorController : Controller
    {
        private readonly Context _context;
        private readonly ILogger<TimescalesAuthorController> _logger;
        private readonly IAuditHandler _auditHandler;
        private readonly IPublishHandler _publishHandler;
        private readonly ILegacyPublishHandler _legacyPublishHandler;

        public TimescalesAuthorController(Context context, ILogger<TimescalesAuthorController> logger, 
                                            IAuditHandler auditHandler, IPublishHandler publishHandler,
                                            ILegacyPublishHandler legacyPublishHandler)
        {
            _context = context;
            _logger = logger;
            _auditHandler = auditHandler;
            _publishHandler = publishHandler;
            _legacyPublishHandler = legacyPublishHandler;
        }

        // GET: TimescalesAuthor  
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
        public async Task<IActionResult> Create([Bind("Id,Placeholder,Name,Description,Owners,OldestWorkDate,Days,Basis,LineOfBusiness")] Timescale timescale)
        {
            if (!IsAuthedRole(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
            {
                ViewBag.UserMessage = "You are not authorised to create a timescale.";
                return View(timescale);
            }

            if (ModelState.IsValid)
            {
                timescale.Id = Guid.NewGuid();
                timescale.UpdatedDate = DateTime.Now;
                _context.Add(timescale);
                await _context.SaveChangesAsync();
                await _auditHandler.AddAuditLog("Create", timescale, @User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1));
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Placeholder,Name,Description,Owners,OldestWorkDate,Days,Basis,LineOfBusiness")] Timescale timescale)
        {
            if (id != timescale.Id)
            {
                return NotFound();
            }
            else if (!IsAuthedRole(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
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
                await _auditHandler.AddAuditLog("Edit", timescale, @User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1));
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

            if (!IsAuthedRole(@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)))
            {
                ViewBag.UserMessage = "You are not authorised to delete this timescale.";
                return View(timescale);
            }

            _context.Timescales.Remove(timescale);
            await _context.SaveChangesAsync();
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

            var timescale = await _context.Timescales
                                    .Include(a => a.Audit)
                                    .Where(t => t.Id == id)
                                    .FirstOrDefaultAsync();

            if (timescale == null)
            {
                return NotFound();
            }
            return View(timescale);
        }

        private bool TimescaleExists(Guid id)
        {
            return _context.Timescales.Any(e => e.Id == id);
        }

        private bool IsAuthedRole(string pid)
        {
            var file = Environment.GetEnvironmentVariable("StaffList", EnvironmentVariableTarget.Machine);
            XmlDocument xml = new XmlDocument();        
            string textFromPage;

            WebClient web = new WebClient
            {
                Credentials = CredentialCache.DefaultCredentials
            };

            Stream stream = web.OpenRead(file);
            using (StreamReader reader = new StreamReader(stream))
            {
                textFromPage = reader.ReadToEnd();
            }
            xml.LoadXml(textFromPage);          

            var nodelocation = $"dataroot/Entry[PID='{@User.Identity.Name.Substring(@User.Identity.Name.IndexOf(@"\") + 1)}']";
            var entry = xml.SelectSingleNode(nodelocation);

            if(entry == null)
            {
                return false;
            }

            var role = entry.SelectSingleNode("Role").InnerText;

            if (role == "Admin" || role == "IPDM")
            {
                return true;
            }

            return false;
        }
    }
}
