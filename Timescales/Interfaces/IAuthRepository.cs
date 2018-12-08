using System.Threading.Tasks;

namespace Timescales.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> IsAuthedRole(string pid);
    }
}
