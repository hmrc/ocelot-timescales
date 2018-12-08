using System.Threading.Tasks;

namespace Timescales.Interfaces
{
    public interface IFileRepository
    {
        Task CreateFile(string publishFile, string data);
    }
}
