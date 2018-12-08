using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Timescales.Models;

namespace Timescales.Interfaces
{
    public interface ITimescaleRepository
    {
        Task<Timescale> Get(Guid? id);

        Task<Timescale> Get(string placeholder);

        Task<IEnumerable<Timescale>> GetMany();

        Task<IEnumerable<Timescale>> GetMany(Expression<Func<Timescale, bool>> where);

        IQueryable<Timescale> GetMany(Expression<Func<Timescale, bool>> where, Expression<Func<Timescale, string>> orderBy, bool ascending);       

        Task Post(Timescale timescale);

        Task Put(Timescale timescale);

        Task Delete(Timescale timescale);

        Task<bool> Exists(Guid id);
    }
}
