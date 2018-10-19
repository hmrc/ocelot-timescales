using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Timescales.Controllers.Helpers.Interfaces;
using Timescales.Models;

namespace Timescales.Controllers.Helpers
{
    public class AuditDataHandler : IAuditDataHandler
    {
        private readonly Context _context;
        private readonly ILogger<AuditDataHandler> _logger;

        public AuditDataHandler(Context context, 
                                    ILogger<AuditDataHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<bool> Post(string action, Timescale timescale, string user)
        {
            return Task.Run(() => PostAsync(action, timescale, user));            
        }

        private bool PostAsync(string action, Timescale timescale, string user)
        {
            var audit = new Audit()
            {
                TimescaleId = timescale.Id,
                User = user,
                Action = action,
                DateTime = DateTime.Now,
                OldestWorkDate = timescale.OldestWorkDate,
                Days = timescale.Days
            };

            _context.Add(audit);
            _context.SaveChanges();

            return true;
        }
    }
}
