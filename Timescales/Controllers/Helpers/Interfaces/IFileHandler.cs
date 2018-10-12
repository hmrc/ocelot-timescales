using System.Threading.Tasks;

namespace Timescales.Controllers.Helpers.Interfaces
{
    public interface IFileHandler
    {
        Task<bool> CreateFile(string publishFile, string data);
    }
}
