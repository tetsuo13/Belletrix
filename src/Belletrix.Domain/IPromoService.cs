using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IPromoService
    {
        Task<IEnumerable<PromoViewModel>> GetPromos();
        Task<PromoViewModel> GetPromo(int id);
        Task<PromoViewModel> GetPromo(string code);
        Task Save(PromoCreateViewModel model, int userId);
        Task<bool> CheckNameForUniqueness(string name);
        Task<IEnumerable<PromoSourceViewModel>> AsSources();
    }
}
