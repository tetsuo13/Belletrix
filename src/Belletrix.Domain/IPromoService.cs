using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IPromoService
    {
        Task<IEnumerable<PromoViewModel>> GetPromos();
        Task<PromoViewModel> GetPromo(int id);
        Task<PromoViewModel> GetPromo(Guid token);
        Task<int> Save(PromoCreateViewModel model, int userId);
        Task<IEnumerable<PromoSourceViewModel>> AsSources();
        Task<GenericResult> Delete(int id);
        Task Update(PromoEditViewModel promo);
    }
}
