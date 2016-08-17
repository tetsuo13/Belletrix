using Belletrix.Core;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using Dapper;
using StackExchange.Exceptional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Belletrix.DAL
{
    public class StudentNoteRepository : IStudentNoteRepository
    {
        private readonly IUnitOfWork UnitOfWork;

        public StudentNoteRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NoteModel>> GetNotes(int studentId)
        {
            const string sql = @"
                SELECT      n.Id AS StudentId, u.Id AS Id, u.FirstName,
                            u.LastName, [EntryDate], [Note],
                            u.Id AS UserId
                FROM        [dbo].[StudentNotes] n
                INNER JOIN  [dbo].[Users] u ON
                            [CreatedBy] = u.[Id]
                WHERE       n.StudentId = @StudentId
                ORDER BY    [EntryDate] DESC";

            List<NoteModel> notes = new List<NoteModel>();

            try
            {
                notes = (await UnitOfWork.Context().QueryAsync<NoteModel>(sql, new { StudentId = studentId })).ToList();
                notes.ForEach(x => x.EntryDate = DateTimeFilter.UtcToLocal(x.EntryDate));
            }
            catch (Exception e)
            {
                e.Data["SQL"] = e;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return notes;
        }

        public async Task InsertNote(int userId, AddStudentNoteViewModel model)
        {
            const string sql = @"
                INSERT INTO [dbo].[StudentNotes]
                ([StudentId], [CreatedBy], [EntryDate], [Note])
                VALUES
                (@StudentId, @CreatedBy, @EntryDate, @Note)";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql,
                    new
                    {
                        StudentId = model.StudentId,
                        CreatedBy = userId,
                        EntryDate = DateTime.Now.ToUniversalTime(),
                        Note = model.Note.Trim()
                    });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }
    }
}
