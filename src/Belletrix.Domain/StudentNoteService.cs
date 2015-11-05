using Belletrix.DAL;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class StudentNoteService : IStudentNoteService
    {
        private readonly IStudentNoteRepository StudentNoteRepository;
        private readonly IEventLogRepository EventLogRepository;

        public StudentNoteService(IStudentNoteRepository studentNoteRepository,
            IEventLogRepository eventLogRepository)
        {
            StudentNoteRepository = studentNoteRepository;
            EventLogRepository = eventLogRepository;
        }

        public async Task<IEnumerable<NoteModel>> GetAllNotes(int studentId)
        {
            return await StudentNoteRepository.GetNotes(studentId);
        }

        public async Task InsertNote(int userId, AddStudentNoteViewModel model)
        {
            await StudentNoteRepository.InsertNote(userId, model);

            await EventLogRepository.AddStudentEvent(userId, model.StudentId, EventLogTypes.AddStudentNote);
            EventLogRepository.SaveChanges();
        }

        public void SaveChanges()
        {
            StudentNoteRepository.SaveChanges();
        }
    }
}
