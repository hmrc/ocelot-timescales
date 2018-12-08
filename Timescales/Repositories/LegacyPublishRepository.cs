using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Timescales.Interfaces;

namespace Timescales.Repositories
{
    public class LegacyPublishRepository : ILegacyPublishRepository
    {
        private readonly ILogger<LegacyPublishRepository> _logger;
        private readonly IFileRepository _fileRepository;
        private readonly ITimescaleRepository _timescaleRepository;

        public LegacyPublishRepository(ILogger<LegacyPublishRepository> logger,
                                       IFileRepository fileRepository,
                                       ITimescaleRepository timescaleRepository)
        {           
            _logger = logger;
            _fileRepository = fileRepository;
            _timescaleRepository = timescaleRepository;
        }
              
        public async Task Publish(string lineOfBusiness)
        {
            var publishFile = $"{Environment.GetEnvironmentVariable("LegacyTimescalesLocation", EnvironmentVariableTarget.Machine)}{lineOfBusiness}Timescales.xml";

            var timescales = await _timescaleRepository.GetMany(t => t.LineOfBusiness == lineOfBusiness);

            XElement export = new XElement("domroot",
                                    new XElement("Entry",
                                        new XElement("WC", "45000"),
                                        timescales.Select(t => new XElement(t.Placeholder, t.Days))
                                        )
                                    );

            await _fileRepository.CreateFile(publishFile, export.ToString());
          
            return;
        }
    }
}
