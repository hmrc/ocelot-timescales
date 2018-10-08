using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Timescales.Controllers.Helpers.Interfaces;
using Timescales.Models;

namespace Timescales.Controllers.Helpers
{
    public class AuditHandler : IAuditHandler
    {
        private readonly Context _context;
        private readonly ILogger<AuditHandler> _logger;

        public AuditHandler(Context context, ILogger<AuditHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAuditLog(string action, Timescale timescale, string user)
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
            await _context.SaveChangesAsync();
        }
    }
}
