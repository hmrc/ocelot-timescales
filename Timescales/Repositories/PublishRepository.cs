using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Timescales.Interfaces;

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

        public Task Publish() => Task.Run(() => PublishAsync());

        private void PublishAsync()
        {
            //todo

            //var publishFile = Environment.GetEnvironmentVariable("TimescalesFile", EnvironmentVariableTarget.Machine);
            //var timescales = _timescaleRepository.GetMany();
           // var timescalesJson = JsonConvert.SerializeObject(timescales);

            //_fileRepository.CreateFile(publishFile, timescalesJson);

            return;
        }
    }
}
