using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bennett.AbroadAdvisor.Models
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

        public static List<NoteModel> GetNotes(int studentId)
        {
            List<NoteModel> notes = new List<NoteModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT      n.id, u.id, u.first_name,
                                    u.last_name, entry_date, note,
                                    u.id AS user_id
                        FROM        student_notes n
                        INNER JOIN  users u ON
                                    created_by = u.id
                        WHERE       n.student_id = @StudentId
                        ORDER BY    entry_date DESC";

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

            return notes;
        }

        public void Save(int userId)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
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

                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = StudentId;
                    command.Parameters.Add("@CreatedBy", NpgsqlTypes.NpgsqlDbType.Integer).Value = userId;
                    command.Parameters.Add("@EntryDate", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();
                    command.Parameters.Add("@Note", NpgsqlTypes.NpgsqlDbType.Text).Value = Note.Trim();

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
