using System.Threading.Tasks;

namespace Timescales.Interfaces
{
    public interface ILegacyPublishRepository
    {
        Task Publish(string lineOfBusiness);
    }
}
