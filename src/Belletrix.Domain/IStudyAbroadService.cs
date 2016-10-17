using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IStudyAbroadService
    {
        Task<IEnumerable<StudyAbroadViewModel>> GetAll();
        Task<IEnumerable<StudyAbroadViewModel>> GetAllForStudent(int studentId);
        Task<StudyAbroadViewModel> FindById(int studyAbroadId);
        Task Save(AddStudyAbroadViewModel model, int userId, string remoteIp);
        Task<GenericResult> Delete(int studyAbroadId);
        Task Update(EditStudyAbroadViewModel model, int userId, string remoteIp);
    }
}
