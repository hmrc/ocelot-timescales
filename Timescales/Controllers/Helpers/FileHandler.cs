using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Timescales.Controllers.Helpers.Interfaces;

namespace Timescales.Controllers.Helpers
{
    public class FileHandler : IFileHandler
    {
        private readonly ILogger<FileHandler> _logger;       

        public FileHandler(ILogger<FileHandler> logger)
        {           
            _logger = logger;
        }

        public Task<bool> CreateFile(string publishFile, string data)
        {
            return Task.Run(() => CreateFileAsync(publishFile, data));            
        }

        private bool CreateFileAsync(string publishFile, string data)
        {
            if (File.Exists(publishFile))
            {
                File.Delete(publishFile);
            }

            using (FileStream fs = File.Create(publishFile))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(data);
                fs.Write(info, 0, info.Length);
            }
            return true;
        }
    }
}
