using Belletrix.DAL;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Belletrix.Domain
{
    public class StudentNoteService : IStudentNoteService
    {
        private readonly IStudentNoteRepository StudentNoteRepository;
        private readonly IEventLogService EventLogService;

        public StudentNoteService(IStudentNoteRepository studentNoteRepository,
            IEventLogService eventLogService)
        {
            StudentNoteRepository = studentNoteRepository;
            EventLogService = eventLogService;
        }

        public async Task<IEnumerable<NoteModel>> GetAllNotes(int studentId)
        {
            return await StudentNoteRepository.GetNotes(studentId);
        }

        public async Task InsertNote(int userId, AddStudentNoteViewModel model, string remoteIp)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await StudentNoteRepository.InsertNote(userId, model);
                await EventLogService.AddStudentEvent(userId, model.StudentId, EventLogTypes.AddStudentNote,
                    remoteIp);

                scope.Complete();
            }
        }
    }
}
