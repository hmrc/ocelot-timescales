using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Timescales.Interfaces;

namespace Timescales.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly ILogger<FileRepository> _logger;       

        public FileRepository(ILogger<FileRepository> logger)
        {           
            _logger = logger;
        }

        public Task CreateFile(string publishFile, string data) => Task.Run(() => CreateFileAsync(publishFile, data));

        private void CreateFileAsync(string publishFile, string data)
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

            return;
        }
    }
}
