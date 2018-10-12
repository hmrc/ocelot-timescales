using System.Threading.Tasks;

namespace Timescales.Controllers.Helpers.Interfaces
{
    public interface ILegacyPublishHandler
    {
        Task<bool> Publish(string lineOfBusiness);
    }
}
