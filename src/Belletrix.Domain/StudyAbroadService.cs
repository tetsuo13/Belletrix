using Belletrix.DAL;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class StudyAbroadService : IStudyAbroadService
    {
        private readonly IStudyAbroadRepository StudyAbroadRepository;

        public StudyAbroadService(IStudyAbroadRepository studyAbroadRepository)
        {
            StudyAbroadRepository = studyAbroadRepository;
        }

        public async Task<IEnumerable<StudyAbroadViewModel>> GetAll(int? studentId = null)
        {
            return await StudyAbroadRepository.GetAll(studentId);
        }

        public async Task Save(AddStudyAbroadViewModel model, int userId)
        {
            await StudyAbroadRepository.Save(model, userId);
        }

        public async Task<GenericResult> Delete(int id)
        {
            GenericResult result = new GenericResult();
            result.Result = await StudyAbroadRepository.Delete(id);

            if (!result.Result)
            {
                result.Message = "Invalid experience id";
            }

            return result;
        }
    }
}
