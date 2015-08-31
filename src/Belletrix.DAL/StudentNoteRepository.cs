using Belletrix.Core;
using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

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
                SELECT      n.Id, u.Id, u.FirstName,
                            u.LastName, [EntryDate], [Note],
                            u.Id AS UserId
                FROM        [dbo].[StudentNotes] n
                INNER JOIN  [dbo].[Users] u ON
                            [CreatedBy] = u.[Id]
                WHERE       n.StudentId = @StudentId
                ORDER BY    [EntryDate] DESC";

            ICollection<NoteModel> notes = new List<NoteModel>();

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            NoteModel note = new NoteModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                                StudentId = studentId,
                                EntryDate = DateTimeFilter.UtcToLocal(await reader.GetFieldValueAsync<DateTime>(reader.GetOrdinal("EntryDate"))),
                                Note = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Note")),
                                CreatedById = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("UserId")),
                                CreatedByFirstName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("FirstName")),
                                CreatedByLastName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("LastName"))
                            };

                            notes.Add(note);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = e;
            }

            return notes;
        }

        public void InsertNote(int userId, NoteModel model)
        {
            const string sql = @"
                INSERT INTO [dbo].[StudentNotes]
                ([StudentId], [CreatedBy], [EntryDate], [Note])
                VALUES
                (@StudentId, @CreatedBy, @EntryDate, @Note)";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.Add("@StudentId", SqlDbType.Int).Value = model.StudentId;
                    command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@EntryDate", SqlDbType.DateTime).Value = DateTime.Now.ToUniversalTime();
                    command.Parameters.Add("@Note", SqlDbType.NVarChar).Value = model.Note.Trim();

                    command.ExecuteNonQuery();
                }

                // TODO: Replace all references of EventLogModel
                //EventLogModel eventLog = new EventLogModel();
                //eventLog.AddStudentEvent(connection, transaction, userId, StudentId, EventLogModel.EventType.AddStudentNote);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        public void SaveChanges()
        {
            UnitOfWork.SaveChanges();
        }
    }
}
