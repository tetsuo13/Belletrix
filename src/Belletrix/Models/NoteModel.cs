using Belletrix.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace Belletrix.Models
{
    public class NoteModel
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public UserModel CreatedBy { get; set; }

        public DateTime EntryDate { get; set; }

        [Required]
        [StringLength(16384)]
        public string Note { get; set; }

        public static IEnumerable<NoteModel> GetNotes(int studentId)
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
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                NoteModel note = new NoteModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    StudentId = studentId,
                                    EntryDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("EntryDate"))),
                                    Note = reader.GetString(reader.GetOrdinal("Note"))
                                };

                                note.CreatedBy = new UserModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                };

                                notes.Add(note);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = e;
                throw e;
            }

            return notes;
        }

        public void Save(int userId)
        {
            const string sql = @"
                INSERT INTO [dbo].[StudentNotes]
                ([StudentId], [CreatedBy], [EntryDate], [Note])
                VALUES
                (@StudentId, @CreatedBy, @EntryDate, @Note)";

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = sql;

                            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = StudentId;
                            command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = userId;
                            command.Parameters.Add("@EntryDate", SqlDbType.DateTime).Value = DateTime.Now.ToUniversalTime();
                            command.Parameters.Add("@Note", SqlDbType.NVarChar).Value = Note.Trim();

                            command.ExecuteNonQuery();
                        }

                        EventLogModel eventLog = new EventLogModel();
                        eventLog.AddStudentEvent(connection, userId, StudentId, EventLogModel.EventType.AddStudentNote);

                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }
    }
}
