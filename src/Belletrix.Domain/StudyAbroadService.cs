using Belletrix.DAL;
using Belletrix.Entity.Model;
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

        public async Task<IEnumerable<StudyAbroadModel>> GetAll(int? studentId = null)
        {
            return await StudyAbroadRepository.GetAll(studentId);
        }

        public async Task Save(StudyAbroadModel model, int userId)
        {
            await StudyAbroadRepository.Save(model, userId);
        }
    }
}
