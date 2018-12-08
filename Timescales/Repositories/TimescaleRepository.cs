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

        public Task<Timescale> Get(Guid? id) => Task.Run(() => GetAsync(id));

        public Task<Timescale> GetIncludeChildObjects(Guid? id) => Task.Run(() => GetIncludeChildObjectsAsync(id));

        public Task<IEnumerable<Timescale>> GetMany() => Task.Run(() => GetManyAsync());

        public Task<IEnumerable<Timescale>> GetMany(Expression<Func<Timescale, bool>> where) => Task.Run(() => GetManyAsync(where));

        public Task<IQueryable<Timescale>> GetMany(Expression<Func<Timescale, bool>> where, Expression<Func<Timescale, string>> orderBy, bool ascending) => Task.Run(() => GetManyAsync(where, orderBy, ascending));

        public Task<bool> Post(Timescale timescale) => Task.Run(() => PostAsync(timescale));

        public Task<bool> Put(Timescale timescale) => Task.Run(() => PutAsync(timescale));

        public Task<bool> Delete(Timescale timescale) => Task.Run(() => DeleteAsync(timescale));

        public Task<bool> Exists(Guid id) => Task.Run(() => ExistsAsync(id));

        private Timescale GetAsync(Guid? id)
        {
            if (id == null)
            {
                return null;
            }

            return _context.Timescales
                           .Where(t => t.Id == id)
                           .FirstOrDefault();
        }

        private Timescale GetIncludeChildObjectsAsync(Guid? id)
        {
            if (id == null)
            {
                return null;
            }

            return _context.Timescales
                           .Include(a => a.Audit)
                           .Where(t => t.Id == id)
                           .FirstOrDefault();
        }

        private IEnumerable<Timescale> GetManyAsync()
        {
            return _context.Timescales
                           .ToList();
        }

        private IEnumerable<Timescale> GetManyAsync(Expression<Func<Timescale, bool>> where)
        {
            return _context.Timescales
                           .Where(where)
                           .ToList();
        }

        private IQueryable<Timescale> GetManyAsync(Expression<Func<Timescale, bool>> where, Expression<Func<Timescale, string>> orderBy, bool ascending)
        {
            if (ascending)
            {
                return _context.Timescales
                               .Where(where)
                               .OrderBy(orderBy);
            }
            else
            {
                return _context.Timescales
                               .Where(where)
                               .OrderByDescending(orderBy);
            }            
        }

        private bool PostAsync(Timescale timescale)
        {
            _context.Add(timescale);
            _context.SaveChanges();

            return true;
        }

        private bool PutAsync(Timescale timescale)
        {
            _context.Update(timescale);
            _context.SaveChanges();

            return true;
        }

        private bool DeleteAsync(Timescale timescale)
        {
            _context.Timescales
                    .Remove(timescale);

            _context.SaveChanges();

            return true;
        }

        private bool ExistsAsync(Guid id)
        {
            return _context.Timescales
                           .Any(e => e.Id == id);
        }
    }
}
