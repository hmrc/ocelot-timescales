using System.Threading.Tasks;
using Timescales.Models;

namespace Timescales.Interfaces
{
    public interface IAuditRepository
    {
        Task Post(string action, Timescale timescale, string user);
    }
}
