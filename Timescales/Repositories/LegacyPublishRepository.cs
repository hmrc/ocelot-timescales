using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Timescales.Interfaces;
using Timescales.Models;

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
              
        public async Task Publish(Timescale timescale)
        {
            if(timescale.Site != "CSG")
            {
                return;
            }

            var publishFile = $"{Environment.GetEnvironmentVariable("TimescalesLocation", EnvironmentVariableTarget.Machine)}" +
                                    $"{timescale.Site}-{timescale.LineOfBusiness}-Timescales.xml";
          
            var timescales = await _timescaleRepository.GetMany(t => t.LineOfBusiness == timescale.LineOfBusiness &&
                                                                     t.Site == timescale.Site);

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
