using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Timescales.Interfaces;
using Timescales.Models;

namespace Timescales.Repositories
{
    public class PublishRepository : IPublishRepository
    {        
        private readonly ILogger<PublishRepository> _logger;
        private readonly IFileRepository _fileRepository;
        private readonly ITimescaleRepository _timescaleRepository;

        public PublishRepository(ILogger<PublishRepository> logger,
                                 IFileRepository fileRepository,
                                 ITimescaleRepository timescaleRepository)
        {
            _logger = logger;
            _fileRepository = fileRepository;
            _timescaleRepository = timescaleRepository;
        }

        public Task Publish(Timescale timescale) => Task.Run(() => PublishAsync(timescale));

        private async void PublishAsync(Timescale timescale)
        {
            var publishFile = $"{Environment.GetEnvironmentVariable("TimescalesLocation", EnvironmentVariableTarget.Machine)}" +
                                    $"{timescale.Site}-Timescales.json";

            var timescales = await _timescaleRepository.GetMany(t => t.Site == timescale.Site);
            var timescalesJson = JsonConvert.SerializeObject(timescales);

            await _fileRepository.CreateFile(publishFile, timescalesJson);

            return;
        }
    }
}
