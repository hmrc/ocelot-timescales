using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Timescales.Controllers.Helpers.Interfaces;
using Timescales.Models;

namespace Timescales.Controllers.Helpers
{
    public class LegacyPublishHandler : ILegacyPublishHandler
    {
        private readonly Context _context;
        private readonly ILogger<PublishHandler> _logger;

        public LegacyPublishHandler(Context context, ILogger<PublishHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<bool> Publish(string lineOfBusiness)
        {
            return Task.Run(() => PublishAsync(lineOfBusiness));
        }

        private bool PublishAsync(string lineOfBusiness)
        {
            var publishFile = $"{Environment.GetEnvironmentVariable("LegacyTimescalesLocation", EnvironmentVariableTarget.Machine)}{lineOfBusiness}Timescales.xml";
            var timescales = _context.Timescales.Where(t => t.LineOfBusiness == lineOfBusiness).ToList();

            XElement export = new XElement("domroot",
                                    new XElement("Entry",
                                        new XElement("WC", "45000") ,
                                        from timescale in timescales
                                        select new XElement(timescale.Placeholder, timescale.Days)
                                        )
                                    );

            if (File.Exists(publishFile))
            {
                File.Delete(publishFile);
            }

            using (FileStream fs = File.Create(publishFile))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(export.ToString());
                fs.Write(info, 0, info.Length);
            }
            return true;
        }
    }
}
