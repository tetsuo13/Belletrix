using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IStudyAbroadRepository
    {
        Task<IEnumerable<StudyAbroadModel>> GetAll(int? studentId = null);
        Task Save(StudyAbroadModel model, int userId);
    }
}
