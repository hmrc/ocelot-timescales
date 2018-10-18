using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Timescales.Controllers.Helpers.Interfaces;
using Timescales.Models;

namespace Timescales.Controllers.Helpers
{
    public class TimescaleDataHandler : ITimescaleDataHandler
    {
        private readonly Context _context;
        private readonly ILogger<TimescaleDataHandler> _logger;

        public TimescaleDataHandler(Context context,
                                                ILogger<TimescaleDataHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<Timescale> Get(Guid? id)
        {
            return Task.Run(() => GetAsync(id));           
        }

        public Task<Timescale> GetIncludeChildObjects(Guid? id)
        {
            return Task.Run(() => GetIncludeChildObjectsAsync(id));
        }

        public Task<IEnumerable<Timescale>> GetMany()
        {
            return Task.Run(() => GetManyAsync());            
        }

        public Task<IEnumerable<Timescale>> GetMany(Expression<Func<Timescale, bool>> where)
        {
            return Task.Run(() => GetManyAsync(where));            
        }

        public Task<bool> Post(Timescale timescale)
        {
            return Task.Run(() => PostAsync(timescale));            
        }

        public Task<bool> Put(Timescale timescale)
        {
            return Task.Run(() => PutAsync(timescale));
        }

        public Task<bool> Delete(Timescale timescale)
        {
            return Task.Run(() => DeleteAsync(timescale));
        }

        public Task<bool> Exists(Guid id)
        {
            return Task.Run(() => ExistsAsync(id));
        }

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
            _context.Timescales.Remove(timescale);
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
