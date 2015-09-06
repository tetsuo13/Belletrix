using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IPingRepository
    {
        Task<string> Ping();
    }
}
