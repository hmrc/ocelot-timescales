using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Timescales.Controllers.Helpers.Interfaces;
using Timescales.Models;

namespace Timescales.Controllers.Helpers
{
    public class LegacyPublishHandler : ILegacyPublishHandler
    {
        private readonly Context _context;
        private readonly ILogger<LegacyPublishHandler> _logger;
        private readonly IFileHandler _fileHandler;

        public LegacyPublishHandler(Context context, 
                                        ILogger<LegacyPublishHandler> logger, 
                                        IFileHandler fileHandler)
        {
            _context = context;
            _logger = logger;
            _fileHandler = fileHandler;
        }

        public Task<bool> Publish(string lineOfBusiness)
        {
            return Task.Run(() => PublishAsync(lineOfBusiness));
        }

        private bool PublishAsync(string lineOfBusiness)
        {
            var publishFile = $"{Environment.GetEnvironmentVariable("LegacyTimescalesLocation", EnvironmentVariableTarget.Machine)}{lineOfBusiness}Timescales.xml";

            var timescales = _context.Timescales
                                     .Where(t => t.LineOfBusiness == lineOfBusiness)
                                     .ToList();

            XElement export = new XElement("domroot",
                                    new XElement("Entry",
                                        new XElement("WC", "45000"),
                                        from timescale in timescales
                                        select new XElement(timescale.Placeholder, timescale.Days)
                                        )
                                    );

            _fileHandler.CreateFile(publishFile, export.ToString());
          
            return true;
        }
    }
}
