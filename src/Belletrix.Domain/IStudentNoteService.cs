using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IStudentNoteService
    {
        Task<IEnumerable<NoteModel>> GetAllNotes(int studentId);
        void InsertNote(int userId, NoteModel model);
        void SaveChanges();
    }
}
