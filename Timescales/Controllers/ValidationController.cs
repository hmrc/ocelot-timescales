using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Timescales.Interfaces;

namespace Timescales.Controllers
{
    public class ValidationController : Controller
    {        
        private readonly ILogger<ValidationController> _logger;
        private readonly ITimescaleRepository _timescaleRepository;

        public ValidationController(ILogger<ValidationController> logger,
                                    ITimescaleRepository timescaleRepository)
        {           
            _logger = logger;
            _timescaleRepository = timescaleRepository;
        }

        public async Task<IActionResult> CheckPlaceholderExist(string placeholder)
        {
            var result = await _timescaleRepository.GetMany(t => t.Placeholder == placeholder);
           
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