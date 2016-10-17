using Belletrix.DAL;
using Belletrix.Entity.Enum;
using Belletrix.Entity.ViewModel;
using System;
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

        public async Task<IEnumerable<StudyAbroadViewModel>> GetAll()
        {
            return await StudyAbroadRepository.GetAll();
        }

        public async Task<IEnumerable<StudyAbroadViewModel>> GetAllForStudent(int studentId)
        {
            return await StudyAbroadRepository.GetAllForStudent(studentId);
        }

        public async Task<StudyAbroadViewModel> FindById(int studyAbroadId)
        {
            return await StudyAbroadRepository.GetById(studyAbroadId);
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

        public async Task<GenericResult> Delete(int studyAbroadId)
        {
            GenericResult result = new GenericResult();
            result.Result = await StudyAbroadRepository.Delete(studyAbroadId);

            if (!result.Result)
            {
                result.Message = "Invalid experience id";
            }

            return result;
        }

        public async Task Update(EditStudyAbroadViewModel model, int userId, string remoteIp)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await StudyAbroadRepository.Update(model);
                    await EventLogService.AddStudentEvent(userId, model.StudentId, EventLogTypes.EditStudentExperience,
                        remoteIp);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
