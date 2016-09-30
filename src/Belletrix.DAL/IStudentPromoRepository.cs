using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IStudentPromoRepository
    {
        Task Save(int studentId, IEnumerable<int> promoIds);
        Task Save(int studentId, Guid promoToken);
        Task<bool> Delete(int id);
    }
}
