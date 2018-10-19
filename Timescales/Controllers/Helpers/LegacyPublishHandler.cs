using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Timescales.Controllers.Helpers.Interfaces;

namespace Timescales.Controllers.Helpers
{
    public class LegacyPublishHandler : ILegacyPublishHandler
    {
        private readonly ILogger<LegacyPublishHandler> _logger;
        private readonly IFileHandler _fileHandler;
        private readonly ITimescaleDataHandler _timescaleDataHandler;

        public LegacyPublishHandler(ILogger<LegacyPublishHandler> logger, 
                                        IFileHandler fileHandler,
                                        ITimescaleDataHandler timescaleDataHandler)
        {           
            _logger = logger;
            _fileHandler = fileHandler;
            _timescaleDataHandler = timescaleDataHandler;
        }
              
        public async Task<bool> Publish(string lineOfBusiness)
        {
            var publishFile = $"{Environment.GetEnvironmentVariable("LegacyTimescalesLocation", EnvironmentVariableTarget.Machine)}{lineOfBusiness}Timescales.xml";

            var timescales = await _timescaleDataHandler.GetMany(t => t.LineOfBusiness == lineOfBusiness);

            XElement export = new XElement("domroot",
                                    new XElement("Entry",
                                        new XElement("WC", "45000"),
                                        timescales.Select(t => new XElement(t.Placeholder, t.Days))
                                        )
                                    );

            await _fileHandler.CreateFile(publishFile, export.ToString());
          
            return true;
        }
    }
}
