using Belletrix.DAL;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
            List<PromoViewModel> promos = (await PromoRepository.GetPromos()).ToList();

            for (int i = 0; i < promos.Count; i++)
            {
                promos[i] = PrepPromo(promos[i]);
            }

            return promos;
        }

        public async Task<PromoViewModel> GetPromo(int id)
        {
            PromoViewModel promo = await PromoRepository.GetPromo(id);
            return PrepPromo(promo);
        }

        public async Task<PromoViewModel> GetPromo(Guid token)
        {
            PromoViewModel promo = await PromoRepository.GetPromo(token);
            return PrepPromo(promo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="promo">May be <see langword="null"/>.</param>
        /// <returns></returns>
        private PromoViewModel PrepPromo(PromoViewModel promo)
        {
            if (promo != null)
            {
                promo.CanDelete = promo.TotalStudents == 0;
            }

            return promo;
        }

        public async Task<int> Save(PromoCreateViewModel model, int userId)
        {
            UserPromoModel promo = new UserPromoModel()
            {
                Description = model.Description,
                Created = DateTime.Now.ToUniversalTime(),
                CreatedBy = userId,
                Active = true,
                PublicToken = Guid.NewGuid()
            };

            return await PromoRepository.Save(promo, userId);
        }

        public async Task<IEnumerable<PromoSourceViewModel>> AsSources()
        {
            return await PromoRepository.AsSources();
        }

        public async Task<GenericResult> Delete(int id)
        {
            GenericResult result = new GenericResult();
            result.Result = await PromoRepository.Delete(id);

            if (!result.Result)
            {
                result.Message = "Error deleting promo";
            }

            return result;
        }
    }
}
