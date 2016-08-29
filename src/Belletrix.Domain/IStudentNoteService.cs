using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IStudentNoteService
    {
        Task<IEnumerable<NoteModel>> GetAllNotes(int studentId);
        Task InsertNote(int userId, AddStudentNoteViewModel model, string remoteIp);
    }
}
