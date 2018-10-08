using System.Threading.Tasks;
using Timescales.Models;

namespace Timescales.Controllers.Helpers.Interfaces
{
    public interface IAuditHandler
    {
        Task AddAuditLog(string action, Timescale timescale, string user);
    }
}
