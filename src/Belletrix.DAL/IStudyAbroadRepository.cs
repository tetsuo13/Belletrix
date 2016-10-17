using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IStudyAbroadRepository
    {
        Task<IEnumerable<StudyAbroadViewModel>> GetAll();
        Task<IEnumerable<StudyAbroadViewModel>> GetAllForStudent(int studentId);
        Task<StudyAbroadViewModel> GetById(int studyAbroadId);
        Task Save(AddStudyAbroadViewModel model);
        Task<bool> Delete(int id);
        Task<bool> DeleteStudent(int id);
        Task Update(EditStudyAbroadViewModel model);
    }
}
