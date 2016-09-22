using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IStudentPromoRepository
    {
        Task<IEnumerable<StudentPromoLog>> Get();
        Task Save(int studentId, IEnumerable<int> promoIds);
        Task Save(int studentId, Guid promoToken);
        Task<IEnumerable<int>> GetPromoIdsForStudent(int studentId);
        Task<IEnumerable<StudentPromoLog>> GetLogsForPromo(int id);
    }
}
