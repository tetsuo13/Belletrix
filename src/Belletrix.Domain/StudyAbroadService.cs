using Belletrix.DAL;
using Belletrix.Entity.Enum;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Belletrix.Domain
{
    public class StudyAbroadService : IStudyAbroadService
    {
        private readonly IStudyAbroadRepository StudyAbroadRepository;
        private readonly IEventLogService EventLogService;

        public StudyAbroadService(IStudyAbroadRepository studyAbroadRepository, IEventLogService eventLogService)
        {
            StudyAbroadRepository = studyAbroadRepository;
            EventLogService = eventLogService;
        }

        public async Task<IEnumerable<StudyAbroadViewModel>> GetAll(int? studentId = null)
        {
            return await StudyAbroadRepository.GetAll(studentId);
        }

        public async Task Save(AddStudyAbroadViewModel model, int userId, string remoteIp)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await StudyAbroadRepository.Save(model);
                await EventLogService.AddStudentEvent(userId, model.StudentId, EventLogTypes.AddStudentExperience,
                    remoteIp);
            }
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
