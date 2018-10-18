using System.Threading.Tasks;
using Timescales.Models;

namespace Timescales.Controllers.Helpers.Interfaces
{
    public interface IAuditDataHandler
    {
        Task<bool> Post(string action, Timescale timescale, string user);
    }
}
