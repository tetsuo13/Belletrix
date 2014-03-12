﻿using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;

namespace Bennett.AbroadAdvisor.Models
{
    public class StudyAbroadModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        public StudentModel Student { get; set; }

        [Required]
        [Display(Name = "Semester")]
        public int Semester { get; set; }

        [Required]
        [Range(1900, 3000)]
        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Departure Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Credit Bearing")]
        public bool CreditBearing { get; set; }

        [Required]
        [Display(Name = "Internship")]
        public bool Internship { get; set; }

        [Required]
        [Display(Name = "Country")]
        public int CountryId { get; set; }

        [StringLength(64)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Program")]
        public int ProgramId { get; set; }

        [Display(Name = "Program Types")]
        public IEnumerable<int> ProgramTypes { get; set; }

        private Dictionary<string, string> columns;
        private List<NpgsqlParameter> parameters;

        public static IEnumerable<StudyAbroadModel> GetAllForStudent(int studentId)
        {
            List<StudyAbroadModel> studyAbroad = new List<StudyAbroadModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT      id, student_id, semester,
                                    year, start_date, end_date,
                                    credit_bearing, internship, country_id,
                                    city, program_id
                        FROM        study_abroad
                        WHERE       student_id = @StudentId
                        ORDER BY    year DESC";

                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;
                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudyAbroadModel study = new StudyAbroadModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                StudentId = reader.GetInt32(reader.GetOrdinal("student_id")),
                                Semester = reader.GetInt32(reader.GetOrdinal("semester")),
                                Year = reader.GetInt32(reader.GetOrdinal("year")),
                                CreditBearing = reader.GetBoolean(reader.GetOrdinal("credit_bearing")),
                                Internship = reader.GetBoolean(reader.GetOrdinal("internship")),
                                CountryId = reader.GetInt32(reader.GetOrdinal("country_id")),
                                ProgramId = reader.GetInt32(reader.GetOrdinal("program_id"))
                            };

                            int ord = reader.GetOrdinal("start_date");
                            if (!reader.IsDBNull(ord))
                            {
                                study.StartDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(ord));
                            }

                            ord = reader.GetOrdinal("end_date");
                            if (!reader.IsDBNull(ord))
                            {
                                study.EndDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(ord));
                            }

                            ord = reader.GetOrdinal("city");
                            if (!reader.IsDBNull(ord))
                            {
                                study.City = reader.GetString(ord);
                            }

                            studyAbroad.Add(study);
                        }

                        PopulateProgramTypes(connection, ref studyAbroad);
                    }
                }
            }

            return studyAbroad;
        }

        public static IEnumerable<StudyAbroadModel> GetActive()
        {
            List<StudyAbroadModel> studyAbroad = new List<StudyAbroadModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT      id, student_id, semester,
                                    year, start_date, end_date,
                                    credit_bearing, internship, country_id,
                                    city, program_id
                        FROM        study_abroad
                        WHERE       year = EXTRACT(YEAR FROM CURRENT_TIMESTAMP)
                        ORDER BY    year DESC";

                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudyAbroadModel study = new StudyAbroadModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                StudentId = reader.GetInt32(reader.GetOrdinal("student_id")),
                                Semester = reader.GetInt32(reader.GetOrdinal("semester")),
                                Year = reader.GetInt32(reader.GetOrdinal("year")),
                                CreditBearing = reader.GetBoolean(reader.GetOrdinal("credit_bearing")),
                                Internship = reader.GetBoolean(reader.GetOrdinal("internship")),
                                CountryId = reader.GetInt32(reader.GetOrdinal("country_id")),
                                ProgramId = reader.GetInt32(reader.GetOrdinal("program_id"))
                            };

                            int ord = reader.GetOrdinal("start_date");
                            if (!reader.IsDBNull(ord))
                            {
                                study.StartDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(ord));
                            }

                            ord = reader.GetOrdinal("end_date");
                            if (!reader.IsDBNull(ord))
                            {
                                study.EndDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(ord));
                            }

                            ord = reader.GetOrdinal("city");
                            if (!reader.IsDBNull(ord))
                            {
                                study.City = reader.GetString(ord);
                            }

                            List<StudentModel> student = StudentModel.GetStudents(study.StudentId);

                            if (student.Count == 1)
                            {
                                study.Student = student[0];
                            }

                            studyAbroad.Add(study);
                        }

                        PopulateProgramTypes(connection, ref studyAbroad);
                    }
                }
            }

            return studyAbroad;
        }

        private static void PopulateProgramTypes(NpgsqlConnection connection, ref List<StudyAbroadModel> studyAbroad)
        {
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT  program_type_id
                    FROM    study_abroad_program_types
                    WHERE   study_abroad_id = @StudyAbroadId";

                command.Parameters.Add("@StudyAbroadId", NpgsqlTypes.NpgsqlDbType.Integer);
                command.Prepare();

                for (int i = 0; i < studyAbroad.Count; i++)
                {
                    List<int> programs = new List<int>();
                    command.Parameters[0].Value = studyAbroad[i].Id;

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            programs.Add(reader.GetInt32(reader.GetOrdinal("program_type_id")));
                        }
                    }

                    studyAbroad[i].ProgramTypes = programs;
                }
            }
        }

        private void AddParameter(StringBuilder sql, string columnName, NpgsqlTypes.NpgsqlDbType columnType,
            object columnValue, int columnLength)
        {
            string parameterName = String.Format("@{0}", columnName);
            columns.Add(columnName, parameterName);

            if (columnLength > 0)
            {
                parameters.Add(new NpgsqlParameter(parameterName, columnType, columnLength) { Value = columnValue });
            }
            else
            {
                parameters.Add(new NpgsqlParameter(parameterName, columnType) { Value = columnValue });
            }
        }

        public void Save(int userId)
        {
            StringBuilder sql = new StringBuilder(@"INSERT INTO study_abroad (");
            columns = new Dictionary<string, string>();
            parameters = new List<NpgsqlParameter>();

            AddParameter(sql, "student_id", NpgsqlTypes.NpgsqlDbType.Integer, StudentId, 0);
            AddParameter(sql, "year", NpgsqlTypes.NpgsqlDbType.Integer, Year, 0);
            AddParameter(sql, "semester", NpgsqlTypes.NpgsqlDbType.Integer, Semester, 0);
            AddParameter(sql, "credit_bearing", NpgsqlTypes.NpgsqlDbType.Boolean, CreditBearing, 0);
            AddParameter(sql, "internship", NpgsqlTypes.NpgsqlDbType.Boolean, Internship, 0);
            AddParameter(sql, "country_id", NpgsqlTypes.NpgsqlDbType.Integer, CountryId, 0);
            AddParameter(sql, "program_id", NpgsqlTypes.NpgsqlDbType.Integer, ProgramId, 0);

            if (StartDate.HasValue)
            {
                AddParameter(sql, "start_date", NpgsqlTypes.NpgsqlDbType.Date, StartDate.Value.ToUniversalTime(), 0);
            }

            if (EndDate.HasValue)
            {
                AddParameter(sql, "end_date", NpgsqlTypes.NpgsqlDbType.Date, EndDate.Value.ToUniversalTime(), 0);
            }

            if (!String.IsNullOrEmpty(City))
            {
                AddParameter(sql, "city", NpgsqlTypes.NpgsqlDbType.Varchar, City, 64);
            }

            sql.Append(String.Join(", ", columns.Select(x => x.Key)));
            sql.Append(") VALUES (");
            sql.Append(String.Join(", ", columns.Select(x => x.Value)));
            sql.Append(") ");
            sql.Append("RETURNING id");

            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;
                connection.Open();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    int studyAbroadId;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql.ToString();
                        command.Parameters.AddRange(parameters.ToArray());
                        studyAbroadId = (int)command.ExecuteScalar();
                    }

                    if (ProgramTypes != null && ProgramTypes.Cast<int>().Count() > 0)
                    {
                        using (NpgsqlCommand command = connection.CreateCommand())
                        {
                            List<string> values = new List<string>();

                            foreach (int programTypeId in ProgramTypes)
                            {
                                values.Add(String.Format("({0}, {1})", studyAbroadId, programTypeId));
                            }

                            sql = new StringBuilder();
                            sql.Append("INSERT INTO study_abroad_program_types (study_abroad_id, program_type_id) VALUES ");
                            sql.Append(String.Join(",", values));

                            command.CommandText = sql.ToString();
                            command.ExecuteNonQuery();
                        }
                    }

                    EventLogModel eventLog = new EventLogModel();
                    eventLog.AddStudentEvent(connection, userId, StudentId, EventLogModel.EventType.AddStudentExperience);

                    transaction.Commit();
                }
            }
        }
    }
}
