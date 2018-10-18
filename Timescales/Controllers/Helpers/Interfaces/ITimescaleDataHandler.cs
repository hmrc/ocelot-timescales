using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Timescales.Models;

namespace Timescales.Controllers.Helpers.Interfaces
{
    public interface ITimescaleDataHandler
    {
        Task<Timescale> Get(Guid? id);

        Task<Timescale> GetIncludeChildObjects(Guid? id);

        Task<IEnumerable<Timescale>> GetMany();

        Task<IEnumerable<Timescale>> GetMany(Expression<Func<Timescale, bool>> where);

        Task<bool> Post(Timescale timescale);

        Task<bool> Put(Timescale timescale);

        Task<bool> Delete(Timescale timescale);

        Task<bool> Exists(Guid id);
    }
}
