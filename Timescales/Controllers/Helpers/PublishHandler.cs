using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Timescales.Controllers.Helpers.Interfaces;
using Timescales.Models;

namespace Timescales.Controllers.Helpers
{
    public class PublishHandler : IPublishHandler
    {
        private readonly Context _context;
        private readonly ILogger<PublishHandler> _logger;
        private readonly IFileHandler _fileHandler;

        public PublishHandler(Context context, 
                                ILogger<PublishHandler> logger,
                                IFileHandler fileHandler)
        {
            _context = context;
            _logger = logger;
            _fileHandler = fileHandler;
        }

        public Task<bool> Publish()
        {
            return Task.Run(() => PublishAsync());
        }

        private bool PublishAsync()
        {
            var publishFile = Environment.GetEnvironmentVariable("TimescalesFile", EnvironmentVariableTarget.Machine);
            var timescales = _context.Timescales.ToList();
            var timescalesJson = JsonConvert.SerializeObject(timescales);

            _fileHandler.CreateFile(publishFile, timescalesJson);

            return true;
        }
    }
}
