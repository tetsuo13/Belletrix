using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IStudentPromoRepository
    {
        Task<IEnumerable<StudentPromoLog>> Get();
        void Save(SqlTransaction transaction, int studentId, IEnumerable<int> promoIds);
        Task<IEnumerable<int>> GetPromoIdsForStudent(int studentId);
        void Save(SqlTransaction transaction, int studentId, string promoCode);
        Task<IEnumerable<StudentPromoLog>> GetLogsForPromo(int id);
    }
}
