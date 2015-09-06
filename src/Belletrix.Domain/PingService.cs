using Belletrix.DAL;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class PingService : IPingService
    {
        private IPingRepository PingRepository;

        public PingService(IPingRepository pingRepository)
        {
            PingRepository = pingRepository;
        }

        public async Task<string> Ping()
        {
            return await PingRepository.Ping();
        }
    }
}
