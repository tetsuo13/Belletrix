using Belletrix.Core;
using Belletrix.Entity.Model;
using Dapper;
using StackExchange.Exceptional;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

            string sql = @"
                SELECT      a.[Id], a.[StudentId], [Semester],
                            [Year], [StartDate], [EndDate],
                            [CreditBearing], [Internship], [CountryId],
                            a.[City], [ProgramId],
                            [FirstName], [MiddleName], [LastName]
                FROM        [dbo].[StudyAbroad] a
                INNER JOIN  [dbo].[Students] s ON
                            a.[StudentId] = s.[Id] ";

            if (studentId.HasValue)
            {
                sql += "WHERE a.[StudentId] = @StudentId ";
            }

            sql += "ORDER BY [Year] DESC, [Semester] DESC";

            try
            {
                IEnumerable<dynamic> rows;

                if (studentId.HasValue)
                {
                    rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql, new { StudentId = studentId.Value });
                }
                else
                {
                    rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql);
                }

                foreach (IReadOnlyDictionary<string, object> row in rows)
                {
                    StudyAbroadModel study = new StudyAbroadModel()
                    {
                        Id = (int)row["Id"],
                        StudentId = (int)row["StudentId"],
                        Semester = (int)row["Semester"],
                        Year = (int)row["Year"],
                        CreditBearing = (bool)row["CreditBearing"],
                        Internship = (bool)row["Internship"],
                        CountryId = (int)row["CountryId"],
                        ProgramId = (int)row["ProgramId"],
                        City = row["City"] as string
                    };

                    if (row.ContainsKey("StartDate") && row["StartDate"] != null)
                    {
                        study.StartDate = DateTimeFilter.UtcToLocal((DateTime)row["StartDate"]);
                    }

                    if (row.ContainsKey("EndDate") && row["EndDate"] != null)
                    {
                        study.EndDate = DateTimeFilter.UtcToLocal((DateTime)row["EndDate"]);
                    }

                    study.Student = new StudentModel()
                    {
                        FirstName = (string)row["FirstName"],
                        MiddleName = row["MiddleName"] as string,
                        LastName = (string)row["LastName"]
                    };

                    studyAbroad.Add(study);
                }

                studyAbroad = await PopulateProgramTypes(studyAbroad);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
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
                for (int i = 0; i < updated.Count; i++)
                {
                    updated[i].ProgramTypes = await UnitOfWork.Context().QueryAsync<int>(sql,
                        new { StudyAbroadId = updated[i].Id });
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
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
            const string sql = @"
                INSERT INTO [dbo].[StudyAbroad]
                (
                    StudentId, Year, Semester, CreditBearing, Internship,
                    CountryId, ProgramId, StartDate, EndDate, City
                )
                OUTPUT INSERTED.Id
                VALUES
                (
                    @StudentId, @Year, @Semester, @CreditBearing, @Internship,
                    @CountryId, @ProgramId, @StartDate, @EndDate, @City
                )";

            if (model.StartDate.HasValue)
            {
                model.StartDate = model.StartDate.Value.ToUniversalTime();
            }

            if (model.EndDate.HasValue)
            {
                model.EndDate = model.EndDate.Value.ToUniversalTime();
            }

            try
            {
                int studyAbroadId = await UnitOfWork.Context().ExecuteAsync(sql,
                    new
                    {
                        StudentId = model.StudentId,
                        Year = model.Year,
                        Semester = model.Semester,
                        CreditBearing = model.CreditBearing,
                        Internship = model.Internship,
                        CountryId = model.CountryId,
                        ProgramId = model.ProgramId,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        City = model.City
                    });

                if (model.ProgramTypes != null && model.ProgramTypes.Any())
                {
                    const string insertSql = @"
                        INSERT INTO [dbo].[StudyAbroadProgramTypes]
                        ([StudyAbroadId], [ProgramTypeId])
                        VALUES
                        (@StudyAbroadId, @ProgramTypeId)";

                    try
                    {
                        await UnitOfWork.Context().ExecuteAsync(insertSql,
                            new
                            {
                                StudyAbroadId = studyAbroadId,
                                ProgramTypeId = model.ProgramTypes
                            });
                    }
                    catch (Exception e)
                    {
                        e.Data["SQL"] = insertSql.ToString();
                        ErrorStore.LogException(e, HttpContext.Current);
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
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }
    }
}
