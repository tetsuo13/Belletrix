using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IStudyAbroadService
    {
        Task<IEnumerable<StudyAbroadViewModel>> GetAll(int? studentId = null);
        Task Save(AddStudyAbroadViewModel model, int userId, string remoteIp);
        Task<GenericResult> Delete(int id);
    }
}
