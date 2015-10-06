﻿using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IPromoService
    {
        Task<IEnumerable<PromoModel>> GetPromos();
        Task<PromoModel> GetPromo(int id);
        Task<PromoModel> GetPromo(string code);
        Task Save(PromoModel model, int userId);
        Task<bool> CheckNameForUniqueness(string name);
        Task<IEnumerable<PromoModel>> AsSources();
    }
}
