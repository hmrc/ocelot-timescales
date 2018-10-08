using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timescales.Controllers.Helpers.Interfaces;
using Timescales.Models;

namespace Timescales.Controllers.Helpers
{
    public class PublishHandler : IPublishHandler
    {
        private readonly Context _context;
        private readonly ILogger<PublishHandler> _logger;

        public PublishHandler(Context context, ILogger<PublishHandler> logger)
        {
            _context = context;
            _logger = logger;
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

            if (File.Exists(publishFile))
            {
                File.Delete(publishFile);
            }

            using (FileStream fs = File.Create(publishFile))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(timescalesJson);              
                fs.Write(info, 0, info.Length);
            }
            return true;
        }
    }
}
