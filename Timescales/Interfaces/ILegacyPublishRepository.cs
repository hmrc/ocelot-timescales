using System.Threading.Tasks;
using Timescales.Models;

namespace Timescales.Interfaces
{
    public interface ILegacyPublishRepository
    {
        Task Publish(Timescale timescale);
    }
}
