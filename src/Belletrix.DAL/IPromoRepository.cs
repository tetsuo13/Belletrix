using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IPromoRepository
    {
        Task<IEnumerable<PromoViewModel>> GetPromos();
        Task<PromoViewModel> GetPromo(int id);
        Task<PromoViewModel> GetPromo(Guid token);
        Task<int> Save(UserPromoModel model, int userId);
        Task<IEnumerable<PromoSourceViewModel>> AsSources();
        Task<bool> Delete(int id);
        Task Update(PromoEditViewModel promo);
        Task<bool> TransferOwnership(int oldId, int newId);
    }
}
