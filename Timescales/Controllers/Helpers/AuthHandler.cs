using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Timescales.Controllers.Helpers.Interfaces;

namespace Timescales.Controllers.Helpers
{
    public class AuthHandler : IAuthHandler
    {
        private readonly ILogger<AuthHandler> _logger;

        public AuthHandler(ILogger<AuthHandler> logger)
        {          
            _logger = logger;
        }

        public Task<bool> IsAuthedRole(string pid)
        {
            return Task.Run(() => IsAuthedRoleAsync(pid));
        }

        private bool IsAuthedRoleAsync(string pid)
        {
            var file = Environment.GetEnvironmentVariable("StaffList", EnvironmentVariableTarget.Machine);
            XmlDocument xml = new XmlDocument();
            string textFromPage;

            WebClient web = new WebClient
            {
                Credentials = CredentialCache.DefaultCredentials
            };

            Stream stream = web.OpenRead(file);

            using (StreamReader reader = new StreamReader(stream))
            {
                textFromPage = reader.ReadToEnd();
            }

            xml.LoadXml(textFromPage);

            var nodelocation = $"dataroot/Entry[PID='{pid}']";
            var entry = xml.SelectSingleNode(nodelocation);

            if (entry == null)
            {
                return false;
            }

            var role = entry.SelectSingleNode("Role").InnerText;

            if (role == "Admin" || role == "IPDM")
            {
                return true;
            }

            return false;
        }
    }
}
