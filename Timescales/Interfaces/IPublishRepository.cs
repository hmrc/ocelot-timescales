using System.Threading.Tasks;

namespace Timescales.Interfaces
{
    public interface IPublishRepository
    {
        Task Publish();
    }
}
