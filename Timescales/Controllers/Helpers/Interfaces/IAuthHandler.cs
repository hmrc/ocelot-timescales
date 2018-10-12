using System.Threading.Tasks;

namespace Timescales.Controllers.Helpers.Interfaces
{
    public interface IAuthHandler
    {
        Task<bool> IsAuthedRole(string pid);
    }
}
