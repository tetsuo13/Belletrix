using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IStudentNoteRepository
    {
        Task<IEnumerable<NoteModel>> GetNotes(int studentId);
        Task InsertNote(int userId, AddStudentNoteViewModel model);
        Task<bool> TransferOwnership(int oldId, int newId);
    }
}
