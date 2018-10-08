using System.Threading.Tasks;

namespace Timescales.Controllers.Helpers.Interfaces
{
    public interface IPublishHandler
    {
        Task<bool> Publish();
    }
}
