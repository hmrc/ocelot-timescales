using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Timescales.Models;

namespace Timescales.Controllers
{
    public class ValidationController : Controller
    {
        private readonly Context _context;
        private readonly ILogger<ValidationController> _logger;

        public ValidationController(Context context, ILogger<ValidationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ActionResult CheckPlaceholderExist(string placeholder)
        {
            var result = _context.Timescales.Where(t => t.Placeholder == placeholder).ToList();

            if (result.Count == 0)
            {
                return Content("true");
            }
            else
            {
                return Content("false");
            }
        }
    }
}