using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Timescales.Interfaces;
using Timescales.Models;

namespace Timescales.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly Context _context;
        private readonly ILogger<AuditRepository> _logger;

        public AuditRepository(Context context, 
                               ILogger<AuditRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Post(string action, Timescale timescale, string user)
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
            return;
        }
    }
}
