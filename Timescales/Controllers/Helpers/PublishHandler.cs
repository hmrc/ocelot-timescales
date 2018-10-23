using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Timescales.Controllers.Helpers.Interfaces;

namespace Timescales.Controllers.Helpers
{
    public class PublishHandler : IPublishHandler
    {        
        private readonly ILogger<PublishHandler> _logger;
        private readonly IFileHandler _fileHandler;
        private readonly ITimescaleDataHandler _timescaleDataHandler;

        public PublishHandler(ILogger<PublishHandler> logger,
                                IFileHandler fileHandler,
                                ITimescaleDataHandler timescaleDataHandler)
        {
            _logger = logger;
            _fileHandler = fileHandler;
            _timescaleDataHandler = timescaleDataHandler;
        }

        public Task<bool> Publish() => Task.Run(() => PublishAsync());

        private bool PublishAsync()
        {
            var publishFile = Environment.GetEnvironmentVariable("TimescalesFile", EnvironmentVariableTarget.Machine);
            var timescales = _timescaleDataHandler.GetMany();
            var timescalesJson = JsonConvert.SerializeObject(timescales);

            _fileHandler.CreateFile(publishFile, timescalesJson);

            return true;
        }
    }
}
