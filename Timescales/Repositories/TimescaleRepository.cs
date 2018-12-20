using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Timescales.Interfaces;
using Timescales.Models;

namespace Timescales.Repositories
{
    public class TimescaleRepository : ITimescaleRepository
    {
        private readonly Context _context;
        private readonly ILogger<TimescaleRepository> _logger;

        public TimescaleRepository(Context context,
                                   ILogger<TimescaleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Timescale> Get(Expression<Func<Timescale, bool>> where)
        {
            return await _context.Timescales
                                 .Include(t => t.Audit)
                                 .Where(where)
                                 .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Timescale>> GetMany()
        {
            return await _context.Timescales
                                 .Include(t => t.Audit)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Timescale>> GetMany(Expression<Func<Timescale, bool>> where)
        {
            return await _context.Timescales
                                 .Include(t => t.Audit)
                                 .Where(where)
                                 .ToListAsync();
        }

        public IQueryable<Timescale> GetMany(Expression<Func<Timescale, bool>> where, Expression<Func<Timescale, string>> orderBy, bool ascending)
        {
            if (ascending)
            {
                return _context.Timescales
                               .Include(t => t.Audit)
                               .Where(where)
                               .OrderBy(orderBy);
            }
            else
            {
                return _context.Timescales
                               .Include(t => t.Audit)
                               .Where(where)
                               .OrderByDescending(orderBy);
            }
        }

        public async Task Post(Timescale timescale)
        {
            _context.Add(timescale);
            await _context.SaveChangesAsync();
            return;
        }


        public async Task Put(Timescale timescale)
        {
            _context.Update(timescale);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task Delete(Timescale timescale)
        {
            _context.Timescales
                    .Remove(timescale);

            await _context.SaveChangesAsync();

            return;
        }

        public Task<bool> Exists(Guid id)
        {
            return _context.Timescales
                           .Where(p => p.Id == id)
                           .AnyAsync();
        }
    }
}
