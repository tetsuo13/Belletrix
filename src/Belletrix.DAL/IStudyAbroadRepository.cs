using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IStudyAbroadRepository
    {
        Task<IEnumerable<StudyAbroadViewModel>> GetAll(int? studentId = null);
        Task Save(AddStudyAbroadViewModel model, int userId);
        Task<bool> Delete(int id);
        Task<bool> DeleteStudent(int id);
    }
}
