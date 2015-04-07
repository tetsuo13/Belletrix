using Belletrix.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
                SELECT      n.id, u.id, u.first_name,
                            u.last_name, entry_date, note,
                            u.id AS user_id
                FROM        student_notes n
                INNER JOIN  users u ON
                            created_by = u.id
                WHERE       n.student_id = @StudentId
                ORDER BY    entry_date DESC";
            ICollection<NoteModel> notes = new List<NoteModel>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;
                        connection.Open();

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                NoteModel note = new NoteModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    StudentId = studentId,
                                    EntryDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("entry_date"))),
                                    Note = reader.GetString(reader.GetOrdinal("note"))
                                };

                                note.CreatedBy = new UserModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("user_id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                                    LastName = reader.GetString(reader.GetOrdinal("last_Name"))
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
                INSERT INTO student_notes
                (
                    student_id, created_by, entry_date,
                    note
                )
                VALUES
                (
                    @StudentId, @CreatedBy, @EntryDate,
                    @Note
                )";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        using (NpgsqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = sql;

                            command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = StudentId;
                            command.Parameters.Add("@CreatedBy", NpgsqlTypes.NpgsqlDbType.Integer).Value = userId;
                            command.Parameters.Add("@EntryDate", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();
                            command.Parameters.Add("@Note", NpgsqlTypes.NpgsqlDbType.Text).Value = Note.Trim();

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
