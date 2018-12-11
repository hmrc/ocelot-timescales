using System.Threading.Tasks;
using Timescales.Models;

namespace Timescales.Interfaces
{
    public interface IPublishRepository
    {
        Task Publish(Timescale timescale);
    }
}
