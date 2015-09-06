using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IPingService
    {
        Task<string> Ping();
    }
}
