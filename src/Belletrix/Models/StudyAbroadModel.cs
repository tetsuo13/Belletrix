using Belletrix.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Belletrix.Models
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
        private List<SqlParameter> parameters;

        public static IEnumerable<StudyAbroadModel> GetAll(int? studentId = null)
        {
            List<StudyAbroadModel> studyAbroad = new List<StudyAbroadModel>();

            StringBuilder sql = new StringBuilder(@"
                SELECT  [Id], [StudentId], [Semester],
                        [Year], [StartDate], [EndDate],
                        [CreditBearing], [Internship], [CountryId],
                        [City], [ProgramId]
                FROM    [StudyAbroad] ");

            if (studentId.HasValue)
            {
                sql.Append("WHERE [StudentId] = @StudentId ");
            }

            sql.Append("ORDER BY [Year] DESC, [Semester] DESC");

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql.ToString();

                        if (studentId.HasValue)
                        {
                            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId.Value;
                        }

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                StudyAbroadModel study = new StudyAbroadModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                    Semester = reader.GetInt32(reader.GetOrdinal("Semester")),
                                    Year = reader.GetInt32(reader.GetOrdinal("Year")),
                                    CreditBearing = reader.GetBoolean(reader.GetOrdinal("CreditBearing")),
                                    Internship = reader.GetBoolean(reader.GetOrdinal("Internship")),
                                    CountryId = reader.GetInt32(reader.GetOrdinal("CountryId")),
                                    ProgramId = reader.GetInt32(reader.GetOrdinal("ProgramId"))
                                };

                                int ord = reader.GetOrdinal("StartDate");
                                if (!reader.IsDBNull(ord))
                                {
                                    study.StartDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(ord));
                                }

                                ord = reader.GetOrdinal("EndDate");
                                if (!reader.IsDBNull(ord))
                                {
                                    study.EndDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(ord));
                                }

                                ord = reader.GetOrdinal("City");
                                if (!reader.IsDBNull(ord))
                                {
                                    study.City = reader.GetString(ord);
                                }

                                try
                                {
                                    study.Student = StudentModel.GetStudent(study.StudentId);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }

                                studyAbroad.Add(study);
                            }
                        }
                    }

                    PopulateProgramTypes(connection, ref studyAbroad);
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql.ToString();
                throw e;
            }

            return studyAbroad;
        }

        private static void PopulateProgramTypes(SqlConnection connection, ref List<StudyAbroadModel> studyAbroad)
        {
            const string sql = @"
                SELECT  [ProgramTypeId]
                FROM    [dbo].[StudyAbroadProgramTypes]
                WHERE   [StudyAbroadId] = @StudyAbroadId";

            try
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudyAbroadId", SqlDbType.Int);
                    command.Prepare();

                    for (int i = 0; i < studyAbroad.Count; i++)
                    {
                        ICollection<int> programs = new List<int>();
                        command.Parameters[0].Value = studyAbroad[i].Id;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                programs.Add(reader.GetInt32(reader.GetOrdinal("ProgramTypeId")));
                            }
                        }

                        studyAbroad[i].ProgramTypes = programs;
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        private void AddParameter(StringBuilder sql, string columnName, SqlDbType columnType, object columnValue,
            int columnLength)
        {
            string parameterName = String.Format("@{0}", columnName);
            columns.Add(columnName, parameterName);

            if (columnLength > 0)
            {
                parameters.Add(new SqlParameter(parameterName, columnType, columnLength) { Value = columnValue });
            }
            else
            {
                parameters.Add(new SqlParameter(parameterName, columnType) { Value = columnValue });
            }
        }

        public void Save(int userId)
        {
            StringBuilder sql = new StringBuilder(@"INSERT INTO study_abroad (");
            columns = new Dictionary<string, string>();
            parameters = new List<SqlParameter>();

            AddParameter(sql, "StudentId", SqlDbType.Int, StudentId, 0);
            AddParameter(sql, "Year", SqlDbType.Int, Year, 0);
            AddParameter(sql, "Semester", SqlDbType.Int, Semester, 0);
            AddParameter(sql, "CreditBearing", SqlDbType.Bit, CreditBearing, 0);
            AddParameter(sql, "Internship", SqlDbType.Bit, Internship, 0);
            AddParameter(sql, "CountryId", SqlDbType.Int, CountryId, 0);
            AddParameter(sql, "ProgramId", SqlDbType.Int, ProgramId, 0);

            if (StartDate.HasValue)
            {
                AddParameter(sql, "StartDate", SqlDbType.Date, StartDate.Value.ToUniversalTime(), 0);
            }

            if (EndDate.HasValue)
            {
                AddParameter(sql, "EndDate", SqlDbType.Date, EndDate.Value.ToUniversalTime(), 0);
            }

            if (!String.IsNullOrEmpty(City))
            {
                AddParameter(sql, "City", SqlDbType.NVarChar, City, 64);
            }

            sql.Append(String.Join(", ", columns.Select(x => x.Key)));
            sql.Append(") OUTPUT INSERTED.Id VALUES (");
            sql.Append(String.Join(", ", columns.Select(x => x.Value)));
            sql.Append(")");

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        int studyAbroadId;

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = sql.ToString();
                            command.Parameters.AddRange(parameters.ToArray());
                            studyAbroadId = (int)command.ExecuteScalar();
                        }

                        if (ProgramTypes != null && ProgramTypes.Cast<int>().Count() > 0)
                        {
                            ICollection<string> values = new List<string>();

                            foreach (int programTypeId in ProgramTypes)
                            {
                                values.Add(String.Format("({0}, {1})", studyAbroadId, programTypeId));
                            }

                            StringBuilder insertSql = new StringBuilder();
                            insertSql.Append("INSERT INTO [dbo].[StudyAbroadProgramTypes] ([StudyAbroadId], [ProgramTypeId]) VALUES ");
                            insertSql.Append(String.Join(",", values));

                            try
                            {
                                using (SqlCommand command = connection.CreateCommand())
                                {
                                    command.Transaction = transaction;
                                    command.CommandText = insertSql.ToString();
                                    command.ExecuteNonQuery();
                                }
                            }
                            catch (Exception e)
                            {
                                e.Data["SQL"] = insertSql.ToString();
                                throw e;
                            }
                        }

                        EventLogModel eventLog = new EventLogModel();
                        eventLog.AddStudentEvent(connection, transaction, userId, StudentId,
                            EventLogModel.EventType.AddStudentExperience);

                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql.ToString();
                throw e;
            }
        }
    }
}
