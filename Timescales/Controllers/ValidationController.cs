using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Timescales.Controllers.Helpers.Interfaces;

namespace Timescales.Controllers
{
    public class ValidationController : Controller
    {        
        private readonly ILogger<ValidationController> _logger;
        private readonly ITimescaleDataHandler _timescaleDataHandler;

        public ValidationController(ILogger<ValidationController> logger,
                                            ITimescaleDataHandler timescaleDataHandler)
        {           
            _logger = logger;
            _timescaleDataHandler = timescaleDataHandler;
        }

        public async Task<ActionResult> CheckPlaceholderExist(string placeholder)
        {
            var result = await _timescaleDataHandler.GetMany(t => t.Placeholder == placeholder);
           
            if (result.Count() == 0)
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