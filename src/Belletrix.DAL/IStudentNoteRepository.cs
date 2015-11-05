using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IStudentNoteRepository
    {
        Task<IEnumerable<NoteModel>> GetNotes(int studentId);
        Task InsertNote(int userId, NoteModel model);
        void SaveChanges();
    }
}
