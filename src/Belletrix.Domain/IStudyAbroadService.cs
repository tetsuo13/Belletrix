using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IStudyAbroadService
    {
        Task<IEnumerable<StudyAbroadModel>> GetAll(int? studentId = null);
        Task Save(StudyAbroadModel model, int userId);
        Task<GenericResult> Delete(int id);
    }
}
