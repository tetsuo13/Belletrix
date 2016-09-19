using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IPromoRepository
    {
        Task<IEnumerable<PromoViewModel>> GetPromos();
        Task<PromoViewModel> GetPromo(int id);
        Task<PromoViewModel> GetPromo(string code);
        Task<int> Save(UserPromoModel model, int userId);
        Task<bool> CheckNameForUniqueness(string name);
        Task<IEnumerable<PromoSourceViewModel>> AsSources();
        Task<bool> Delete(int id);
    }
}
