using Belletrix.Core;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
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

        private IEnumerable<StudyAbroadViewModel> ProcessRecords(IEnumerable<dynamic> rows)
        {
            ICollection<StudyAbroadViewModel> studyAbroad = new List<StudyAbroadViewModel>();

            foreach (IDictionary<string, object> row in rows)
            {
                StudyAbroadViewModel study = new StudyAbroadViewModel()
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

                if (row.ContainsKey("ProgramTypeIds") && row["ProgramTypeIds"] != null)
                {
                    study.ProgramTypes = Array.ConvertAll(((string)row["ProgramTypeIds"]).Split(','), int.Parse);
                }

                study.Student = new StudentModel()
                {
                    FirstName = (string)row["FirstName"],
                    MiddleName = row["MiddleName"] as string,
                    LastName = (string)row["LastName"]
                };

                studyAbroad.Add(study);
            }

            return studyAbroad;
        }

        private string PrepareQuery(bool limitByStudentId, bool limitByStudyAbroadId)
        {
            string sql = @"
                SELECT      a.[Id], a.[StudentId], [Semester],
                            [Year], [StartDate], [EndDate],
                            [CreditBearing], [Internship], [CountryId],
                            a.[City], [ProgramId],
                            (SELECT DISTINCT STUFF((SELECT ',' + CAST([ProgramTypeId] AS VARCHAR(3)) FROM [dbo].[StudyAbroadProgramTypes] WHERE [StudyAbroadId] = a.Id FOR XML PATH('')),1,1,'')) AS ProgramTypeIds,
                            [FirstName], [MiddleName], [LastName]
                FROM        [dbo].[StudyAbroad] a
                INNER JOIN  [dbo].[Students] s ON
                            a.[StudentId] = s.[Id] ";

            if (limitByStudentId)
            {
                sql += "WHERE a.[StudentId] = @StudentId ";
            }
            else if (limitByStudyAbroadId)
            {
                sql += "WHERE a.[Id] = @StudyAbroadId ";
            }

            sql += "ORDER BY [Year] DESC, [Semester] DESC";

            return sql;
        }

        public async Task<IEnumerable<StudyAbroadViewModel>> GetAll()
        {
            string sql = PrepareQuery(false, false);

            try
            {
                IEnumerable<dynamic> rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql);
                return ProcessRecords(rows);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return Enumerable.Empty<StudyAbroadViewModel>();
        }

        public async Task<IEnumerable<StudyAbroadViewModel>> GetAllForStudent(int studentId)
        {
            string sql = PrepareQuery(true, false);

            try
            {
                IEnumerable<dynamic> rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql, new { StudentId = studentId });
                return ProcessRecords(rows);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return Enumerable.Empty<StudyAbroadViewModel>();
        }

        public async Task<StudyAbroadViewModel> GetById(int studyAbroadId)
        {
            string sql = PrepareQuery(false, true);

            try
            {
                IEnumerable<dynamic> rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql, new { StudyAbroadId = studyAbroadId });
                return ProcessRecords(rows).FirstOrDefault();
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return null;
        }

        private void AddParameter(ref Dictionary<string, string> columns, ref List<SqlParameter> parameters,
            StringBuilder sql, string columnName, SqlDbType columnType, object columnValue, int columnLength)
        {
            string parameterName = string.Format("@{0}", columnName);
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

        public async Task Save(AddStudyAbroadViewModel model)
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

            int studyAbroadId;

            try
            {
                studyAbroadId = (await UnitOfWork.Context().QueryAsync<int>(sql,
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
                    })).Single();
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            try
            {
                await ReplaceProgramTypes(studyAbroadId, model.ProgramTypes);
            }
            catch (Exception)
            {
                // Caught and logged already.
            }
        }

        private async Task ReplaceProgramTypes(int studyAbroadId, IEnumerable<int> programTypes)
        {
            string sql = @"
                DELETE  FROM [dbo].[StudyAbroadProgramTypes]
                WHERE   [StudyAbroadId] = @StudyAbroadId";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql, new { StudyAbroadId = studyAbroadId });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            if (programTypes != null && programTypes.Any())
            {
                sql = @"
                    INSERT INTO [dbo].[StudyAbroadProgramTypes]
                    ([StudyAbroadId], [ProgramTypeId])
                    VALUES
                    (@StudyAbroadId, @ProgramTypeId)";

                try
                {
                    foreach (int programTypeId in programTypes)
                    {
                        await UnitOfWork.Context().ExecuteAsync(sql,
                            new
                            {
                                StudyAbroadId = studyAbroadId,
                                ProgramTypeId = programTypeId
                            });
                    }
                }
                catch (Exception e)
                {
                    e.Data["SQL"] = sql;
                    ErrorStore.LogException(e, HttpContext.Current);
                    throw e;
                }
            }
        }

        public async Task<bool> Delete(int id)
        {
            int rowsDeleted;
            const string sql = @"
                DELETE FROM [dbo].[StudyAbroad]
                WHERE   [Id] = @Id";

            try
            {
                rowsDeleted = await UnitOfWork.Context().ExecuteAsync(sql, new { Id = id });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                return false;
            }

            try
            {
                await ReplaceProgramTypes(id, null);
            }
            catch (Exception)
            {
                return false;
            }

            return rowsDeleted == 1;
        }

        public async Task<bool> DeleteStudent(int id)
        {
            string sql = @"
                DELETE FROM [dbo].[StudyAbroadProgramTypes]
                WHERE       [StudyAbroadId] IN (
                                SELECT  [Id]
                                FROM    [StudyAbroad]
                                WHERE   [StudentId] = @StudentId
                            )";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql, new { StudentId = id });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                return false;
            }

            sql = @"
                DELETE FROM [dbo].[StudyAbroad]
                WHERE   [StudentId] = @Id";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql, new { Id = id });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                return false;
            }

            return true;
        }

        public async Task Update(EditStudyAbroadViewModel model)
        {
            const string sql = @"
                UPDATE  [dbo].[StudyAbroad]
                SET     [StudentId] = @StudentId,
                        [Semester] = @Semester,
                        [Year] = @Year,
                        [StartDate] = @StartDate,
                        [EndDate] = @EndDate,
                        [CreditBearing] = @CreditBearing,
                        [Internship] = @Internship,
                        [CountryId] = @CountryId,
                        [City] = @City,
                        [ProgramId] = @ProgramId
                WHERE   Id = @Id";

            try
            {
                if (model.StartDate.HasValue)
                {
                    model.StartDate = model.StartDate.Value.ToUniversalTime();
                }

                if (model.EndDate.HasValue)
                {
                    model.EndDate = model.EndDate.Value.ToUniversalTime();
                }

                await UnitOfWork.Context().ExecuteAsync(sql, model);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            await ReplaceProgramTypes(model.Id, model.ProgramTypes);
        }
    }
}
