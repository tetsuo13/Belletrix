using Belletrix.DAL;
using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class StudentNoteService : IStudentNoteService
    {
        private readonly IStudentNoteRepository StudentNoteRepository;

        public StudentNoteService(IStudentNoteRepository studentNoteRepository)
        {
            StudentNoteRepository = studentNoteRepository;
        }

        public async Task<IEnumerable<NoteModel>> GetAllNotes(int studentId)
        {
            return await StudentNoteRepository.GetNotes(studentId);
        }

        public void InsertNote(int userId, NoteModel model)
        {
            StudentNoteRepository.InsertNote(userId, model);
        }

        public void SaveChanges()
        {
            StudentNoteRepository.SaveChanges();
        }
    }
}
