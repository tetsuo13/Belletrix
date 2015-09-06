using Belletrix.Core;
using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public class StudyAbroadRepository : IStudyAbroadRepository
    {
        private readonly IUnitOfWork UnitOfWork;

        public StudyAbroadRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StudyAbroadModel>> GetAll(int? studentId = null)
        {
            List<StudyAbroadModel> studyAbroad = new List<StudyAbroadModel>();

            StringBuilder sql = new StringBuilder(@"
                SELECT      a.[Id], a.[StudentId], [Semester],
                            [Year], [StartDate], [EndDate],
                            [CreditBearing], [Internship], [CountryId],
                            a.[City], [ProgramId],
                            [FirstName], [MiddleName], [LastName]
                FROM        [dbo].[StudyAbroad] a
                INNER JOIN  [dbo].[Students] s ON
                            a.[StudentId] = s.[Id]");

            if (studentId.HasValue)
            {
                sql.Append("WHERE a.[StudentId] = @StudentId ");
            }

            sql.Append("ORDER BY [Year] DESC, [Semester] DESC");

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql.ToString();

                    if (studentId.HasValue)
                    {
                        command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId.Value;
                    }

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            StudyAbroadModel study = new StudyAbroadModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                                StudentId = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("StudentId")),
                                Semester = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Semester")),
                                Year = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Year")),
                                CreditBearing = await reader.GetFieldValueAsync<bool>(reader.GetOrdinal("CreditBearing")),
                                Internship = await reader.GetFieldValueAsync<bool>(reader.GetOrdinal("Internship")),
                                CountryId = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("CountryId")),
                                ProgramId = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("ProgramId")),
                                City = await reader.GetValueOrDefault<string>("City")
                            };

                            int ord = reader.GetOrdinal("StartDate");
                            if (!reader.IsDBNull(ord))
                            {
                                study.StartDate = DateTimeFilter.UtcToLocal(await reader.GetFieldValueAsync<DateTime>(ord));
                            }

                            ord = reader.GetOrdinal("EndDate");
                            if (!reader.IsDBNull(ord))
                            {
                                study.EndDate = DateTimeFilter.UtcToLocal(await reader.GetFieldValueAsync<DateTime>(ord));
                            }

                            study.Student = new StudentModel()
                            {
                                FirstName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("FirstName")),
                                MiddleName = await reader.GetValueOrDefault<string>("MiddleName"),
                                LastName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("LastName"))
                            };

                            studyAbroad.Add(study);
                        }
                    }
                }

                studyAbroad = await PopulateProgramTypes(studyAbroad);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql.ToString();
                throw e;
            }

            return studyAbroad;
        }

        private async Task<List<StudyAbroadModel>> PopulateProgramTypes(List<StudyAbroadModel> studyAbroad)
        {
            List<StudyAbroadModel> updated = new List<StudyAbroadModel>(studyAbroad);
            const string sql = @"
                SELECT  [ProgramTypeId]
                FROM    [dbo].[StudyAbroadProgramTypes]
                WHERE   [StudyAbroadId] = @StudyAbroadId";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudyAbroadId", SqlDbType.Int);
                    command.Prepare();

                    for (int i = 0; i < updated.Count; i++)
                    {
                        ICollection<int> programs = new List<int>();
                        command.Parameters[0].Value = updated[i].Id;

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                programs.Add(await reader.GetFieldValueAsync<int>(reader.GetOrdinal("ProgramTypeId")));
                            }
                        }

                        updated[i].ProgramTypes = programs;
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }

            return updated;
        }

        private void AddParameter(ref Dictionary<string, string> columns, ref List<SqlParameter> parameters,
            StringBuilder sql, string columnName, SqlDbType columnType, object columnValue, int columnLength)
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

        public async Task Save(StudyAbroadModel model, int userId)
        {
            StringBuilder sql = new StringBuilder(@"INSERT INTO [dbo].[StudyAbroad] (");
            Dictionary<string, string> columns = new Dictionary<string, string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            AddParameter(ref columns, ref parameters, sql, "StudentId", SqlDbType.Int, model.StudentId, 0);
            AddParameter(ref columns, ref parameters, sql, "Year", SqlDbType.Int, model.Year, 0);
            AddParameter(ref columns, ref parameters, sql, "Semester", SqlDbType.Int, model.Semester, 0);
            AddParameter(ref columns, ref parameters, sql, "CreditBearing", SqlDbType.Bit, model.CreditBearing, 0);
            AddParameter(ref columns, ref parameters, sql, "Internship", SqlDbType.Bit, model.Internship, 0);
            AddParameter(ref columns, ref parameters, sql, "CountryId", SqlDbType.Int, model.CountryId, 0);
            AddParameter(ref columns, ref parameters, sql, "ProgramId", SqlDbType.Int, model.ProgramId, 0);

            if (model.StartDate.HasValue)
            {
                AddParameter(ref columns, ref parameters, sql, "StartDate", SqlDbType.Date, model.StartDate.Value.ToUniversalTime(), 0);
            }

            if (model.EndDate.HasValue)
            {
                AddParameter(ref columns, ref parameters, sql, "EndDate", SqlDbType.Date, model.EndDate.Value.ToUniversalTime(), 0);
            }

            if (!String.IsNullOrEmpty(model.City))
            {
                AddParameter(ref columns, ref parameters, sql, "City", SqlDbType.NVarChar, model.City, 64);
            }

            sql.Append(String.Join(", ", columns.Select(x => x.Key)));
            sql.Append(") OUTPUT INSERTED.Id VALUES (");
            sql.Append(String.Join(", ", columns.Select(x => x.Value)));
            sql.Append(")");

            try
            {
                int studyAbroadId;

                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql.ToString();
                    command.Parameters.AddRange(parameters.ToArray());
                    studyAbroadId = (int)(await command.ExecuteScalarAsync());
                }

                if (model.ProgramTypes != null && model.ProgramTypes.Any())
                {
                    ICollection<string> values = new List<string>();

                    foreach (int programTypeId in model.ProgramTypes)
                    {
                        values.Add(String.Format("({0}, {1})", studyAbroadId, programTypeId));
                    }

                    StringBuilder insertSql = new StringBuilder();
                    insertSql.Append("INSERT INTO [dbo].[StudyAbroadProgramTypes] ([StudyAbroadId], [ProgramTypeId]) VALUES ");
                    insertSql.Append(String.Join(",", values));

                    try
                    {
                        using (SqlCommand command = UnitOfWork.CreateCommand())
                        {
                            command.CommandText = insertSql.ToString();
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        e.Data["SQL"] = insertSql.ToString();
                        throw e;
                    }
                }

                //EventLogModel eventLog = new EventLogModel();
                //eventLog.AddStudentEvent(connection, transaction, userId, StudentId,
                //    EventLogModel.EventType.AddStudentExperience);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql.ToString();
                throw e;
            }
        }
    }
}
