using Belletrix.DAL;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class PromoService : IPromoService
    {
        private IPromoRepository PromoRepository;

        public PromoService(IPromoRepository promoRepository)
        {
            PromoRepository = promoRepository;
        }

        public async Task<IEnumerable<PromoViewModel>> GetPromos()
        {
            return await PromoRepository.GetPromos();
        }

        public async Task<PromoViewModel> GetPromo(int id)
        {
            return await PromoRepository.GetPromo(id);
        }

        public async Task<PromoViewModel> GetPromo(string code)
        {
            return await PromoRepository.GetPromo(code);
        }

        public async Task Save(PromoCreateViewModel model, int userId)
        {
            await PromoRepository.Save(model, userId);
        }

        public async Task<bool> CheckNameForUniqueness(string name)
        {
            return await PromoRepository.CheckNameForUniqueness(name);
        }

        public async Task<IEnumerable<PromoSourceViewModel>> AsSources()
        {
            return await PromoRepository.AsSources();
        }
    }
}
