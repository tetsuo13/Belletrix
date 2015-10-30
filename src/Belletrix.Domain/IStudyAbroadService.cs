using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IStudyAbroadService
    {
        Task<IEnumerable<StudyAbroadModel>> GetAll(int? studentId = null);
        Task Save(StudyAbroadModel model, int userId);
    }
}
