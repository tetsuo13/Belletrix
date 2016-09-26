using Belletrix.Core;
using Belletrix.Entity.Model;
using Dapper;
using StackExchange.Exceptional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Belletrix.DAL
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IUnitOfWork UnitOfWork;

        public StudentRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<StudentModel> GetStudent(int id)
        {
            return (await GetStudents(id)).FirstOrDefault();
        }

        public async Task<IEnumerable<StudentModel>> GetStudents(int? id = null)
        {
            ICollection<StudentModel> students = new List<StudentModel>();

            string sql = @"
                SELECT              s.Id, s.Created, s.InitialMeeting,
                                    (SELECT DISTINCT STUFF((SELECT ',' + CAST([MajorId] AS VARCHAR(3)) FROM [dbo].[Matriculation] WHERE [StudentId] = s.Id AND IsMajor = 1 FOR XML PATH('')),1,1,'')) AS MajorIds,
                                    (SELECT DISTINCT STUFF((SELECT ',' + CAST([MajorId] AS VARCHAR(3)) FROM [dbo].[Matriculation] WHERE [StudentId] = s.Id AND IsMajor = 0 FOR XML PATH('')),1,1,'')) AS MinorIds,
                                    (SELECT DISTINCT STUFF((SELECT ',' + CAST([LanguageId] AS VARCHAR(3)) FROM [dbo].[StudentStudiedLanguages] WHERE [StudentId] = s.Id FOR XML PATH('')),1,1,'')) AS StudiedLanguageIds,
                                    (SELECT DISTINCT STUFF((SELECT ',' + CAST([LanguageId] AS VARCHAR(3)) FROM [dbo].[StudentDesiredLanguages] WHERE [StudentId] = s.Id FOR XML PATH('')),1,1,'')) AS DesiredLanguageIds,
                                    (SELECT DISTINCT STUFF((SELECT ',' + CAST([LanguageId] AS VARCHAR(3)) FROM [dbo].[StudentFluentLanguages] WHERE [StudentId] = s.Id FOR XML PATH('')),1,1,'')) AS FluentLanguageIds,
                                    (SELECT DISTINCT STUFF((SELECT ',' + CAST([CountryId] AS VARCHAR(3)) FROM [dbo].[StudentStudyAbroadWishlist] WHERE [StudentId] = s.Id FOR XML PATH('')),1,1,'')) AS StudyAbroadCountryIds,
                                    (SELECT DISTINCT STUFF((SELECT ',' + CAST([Year] AS VARCHAR(4)) FROM [dbo].[StudentStudyAbroadWishlist] WHERE [StudentId] = s.Id FOR XML PATH('')),1,1,'')) AS StudyAbroadYearIds,
                                    (SELECT DISTINCT STUFF((SELECT ',' + CAST([Period] AS VARCHAR(3)) FROM [dbo].[StudentStudyAbroadWishlist] WHERE [StudentId] = s.Id FOR XML PATH('')),1,1,'')) AS StudyAbroadPeriodIds,
                                    (SELECT DISTINCT STUFF((SELECT ',' + CAST([PromoId] AS VARCHAR(3)) FROM [dbo].[StudentPromoLog] WHERE [StudentId] = s.Id FOR XML PATH('')),1,1,'')) AS StudentPromoLogIds,
                                    s.FirstName, s.MiddleName, s.LastName,
                                    s.LivingOnCampus, s.PhoneNumber, s.StudentId,
                                    s.Dob, s.EnrolledFullTime, s.Citizenship,
                                    s.PellGrantRecipient, s.PassportHolder, s.Gpa,
                                    s.CampusEmail, s.AlternateEmail, s.GraduatingYear,
                                    s.Classification, s.StreetAddress, s.StreetAddress2,
                                    s.City, s.State, s.PostalCode,
                                    s.EnteringYear,
                                    COUNT(n.Id) AS NumNotes
                FROM                [dbo].[Students] s
                LEFT OUTER JOIN     [dbo].[StudentNotes] n ON
                                    s.Id = n.StudentId ";

            if (id.HasValue)
            {
                sql += "WHERE s.Id = @StudentId ";
            }

            sql += @"
                GROUP BY            s.Id, s.Created, s.InitialMeeting,
                                    s.FirstName, s.MiddleName, s.LastName,
                                    s.LivingOnCampus, s.PhoneNumber, s.StudentId,
                                    s.Dob, s.EnrolledFullTime, s.Citizenship,
                                    s.PellGrantRecipient, s.PassportHolder, s.Gpa,
                                    s.CampusEmail, s.AlternateEmail, s.GraduatingYear,
                                    s.Classification, s.StreetAddress, s.StreetAddress2,
                                    s.City, s.State, s.PostalCode,
                                    s.EnteringYear
                ORDER BY            s.LastName, s.FirstName";

            try
            {
                IEnumerable<dynamic> rows;

                if (id.HasValue)
                {
                    rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql, new { StudentId = id.Value });
                }
                else
                {
                    rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql);
                }

                foreach (IDictionary<string, object> row in rows)
                {
                    StudentModel student = new StudentModel()
                    {
                        Id = (int)row["Id"],
                        FirstName = (string)row["FirstName"],
                        LastName = (string)row["LastName"],
                        MiddleName = row["MiddleName"] as string,
                        LivingOnCampus = row["LivingOnCampus"] as bool?,
                        StreetAddress = row["StreetAddress"] as string,
                        StreetAddress2 = row["StreetAddress2"] as string,
                        City = row["City"] as string,
                        State = row["State"] as string,
                        PostalCode = row["PostalCode"] as string,
                        PhoneNumber = row["PhoneNumber"] as string,
                        EnteringYear = row["EnteringYear"] as int?,
                        GraduatingYear = row["GraduatingYear"] as int?,
                        StudentId = row["StudentId"] as string,
                        EnrolledFullTime = row["EnrolledFullTime"] as bool?,
                        Citizenship = row["Citizenship"] as int?,
                        PellGrantRecipient = row["PellGrantRecipient"] as bool?,
                        HasPassport = row["PassportHolder"] as bool?,
                        CampusEmail = row["CampusEmail"] as string,
                        AlternateEmail = row["AlternateEmail"] as string,
                        Created = DateTimeFilter.UtcToLocal((DateTime)row["Created"]),
                        NumberOfNotes = (int)row["NumNotes"],
                        Gpa = row["Gpa"] as decimal?,
                    };

                    if (row.ContainsKey("Dob") && row["Dob"] != null)
                    {
                        student.DateOfBirth = DateTimeFilter.UtcToLocal((DateTime)row["Dob"]);
                    }

                    if (row.ContainsKey("InitialMeeting") && row["InitialMeeting"] != null)
                    {
                        student.InitialMeeting = DateTimeFilter.UtcToLocal((DateTime)row["InitialMeeting"]);
                    }

                    if (row.ContainsKey("MajorIds") && row["MajorIds"] != null)
                    {
                        student.SelectedMajors = Array.ConvertAll(((string)row["MajorIds"]).Split(','), int.Parse);
                    }

                    if (row.ContainsKey("MinorIds") && row["MinorIds"] != null)
                    {
                        student.SelectedMinors = Array.ConvertAll(((string)row["MinorIds"]).Split(','), int.Parse);
                    }

                    if (row.ContainsKey("StudiedLanguageIds") && row["StudiedLanguageIds"] != null)
                    {
                        student.StudiedLanguages = Array.ConvertAll(((string)row["StudiedLanguageIds"]).Split(','), int.Parse);
                    }

                    if (row.ContainsKey("DesiredLanguageIds") && row["DesiredLanguageIds"] != null)
                    {
                        student.SelectedDesiredLanguages = Array.ConvertAll(((string)row["DesiredLanguageIds"]).Split(','), int.Parse);
                    }

                    if (row.ContainsKey("FluentLanguageIds") && row["FluentLanguageIds"] != null)
                    {
                        student.SelectedLanguages = Array.ConvertAll(((string)row["FluentLanguageIds"]).Split(','), int.Parse);
                    }

                    if (row.ContainsKey("StudentPromoLogIds") && row["StudentPromoLogIds"] != null)
                    {
                        student.PromoIds = Array.ConvertAll(((string)row["StudentPromoLogIds"]).Split(','), int.Parse);
                    }

                    // View and JavaScript expects these collections
                    // to never be null.
                    if (row.ContainsKey("StudyAbroadCountryIds") && row["StudyAbroadCountryIds"] != null)
                    {
                        student.StudyAbroadCountry = Array.ConvertAll(((string)row["StudyAbroadCountryIds"]).Split(','), int.Parse);
                        student.StudyAbroadYear = Array.ConvertAll(((string)row["StudyAbroadYearIds"]).Split(','), int.Parse);
                        student.StudyAbroadPeriod = Array.ConvertAll(((string)row["StudyAbroadPeriodIds"]).Split(','), int.Parse);
                    }
                    else
                    {
                        student.StudyAbroadCountry = new List<int>();
                        student.StudyAbroadYear = new List<int>();
                        student.StudyAbroadPeriod = new List<int>();
                    }

                    students.Add(student);
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return students;
        }

        public async Task<IEnumerable<CountryModel>> GetCountries()
        {
            const string sql = @"
                SELECT      [Id], [Name], [Abbreviation]
                FROM        [dbo].[Countries]
                WHERE       [IsRegion] = 0
                ORDER BY    CASE [Abbreviation]
                                WHEN 'US' THEN 1
                                WHEN '' THEN 2
                                ELSE 3
                            END, [Name]";

            try
            {
                return await UnitOfWork.Context().QueryAsync<CountryModel>(sql);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return new List<CountryModel>();
        }

        public async Task<IEnumerable<CountryModel>> GetRegions()
        {
            const string sql = @"
                SELECT      [Id], [Name], [Abbreviation]
                FROM        [dbo].[Countries]
                WHERE       [IsRegion] = 1
                ORDER BY    [Name]";

            try
            {
                return await UnitOfWork.Context().QueryAsync<CountryModel>(sql);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return new List<CountryModel>();
        }

        public async Task<IEnumerable<LanguageModel>> GetLanguages()
        {
            const string sql = @"
                SELECT      [Id], [Name]
                FROM        [dbo].[Languages]
                ORDER BY    [Name]";

            try
            {
                return await UnitOfWork.Context().QueryAsync<LanguageModel>(sql);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return new List<LanguageModel>();
        }

        public async Task<IEnumerable<MajorsModel>> GetMajors()
        {
            const string sql = @"
                SELECT      [Id], [Name]
                FROM        [Majors]
                ORDER BY    [Name]";

            try
            {
                return await UnitOfWork.Context().QueryAsync<MajorsModel>(sql);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return new List<MajorsModel>();
        }

        public async Task<IEnumerable<MinorsModel>> GetMinors()
        {
            const string sql = @"
                SELECT      [Id], [Name]
                FROM        [Minors]
                ORDER BY    [Name]";

            try
            {
                return await UnitOfWork.Context().QueryAsync<MinorsModel>(sql);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return new List<MinorsModel>();
        }

        public async Task<IEnumerable<ProgramModel>> GetPrograms()
        {
            const string sql = @"
                SELECT      [Id], [Name], [Abbreviation]
                FROM        [Programs]
                ORDER BY    [Name]";

            try
            {
                return await UnitOfWork.Context().QueryAsync<ProgramModel>(sql);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return new List<ProgramModel>();
        }

        public async Task<IEnumerable<ProgramTypeModel>> GetProgramTypes()
        {
            const string sql = @"
                SELECT      [Id], [Name], [ShortTerm]
                FROM        [ProgramTypes]
                ORDER BY    [Name]";

            try
            {
                return await UnitOfWork.Context().QueryAsync<ProgramTypeModel>(sql);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return new List<ProgramTypeModel>();
        }

        public async Task<int> InsertStudent(object model)
        {
            int studentId;

            const string sql = @"
                INSERT INTO [dbo].[Students]
                (
                    [Created], [InitialMeeting], [FirstName], [MiddleName],
                    [LastName], [StreetAddress], [StreetAddress2], [City],
                    [State], [PostalCode], [Classification], [StudentId],
                    [PhoneNumber], [LivingOnCampus], [EnrolledFullTime], [Citizenship],
                    [PellGrantRecipient], [PassportHolder], [Gpa], [CampusEmail],
                    [AlternateEmail], [EnteringYear], [GraduatingYear], [Dob], [PhiBetaDeltaMember]
                )
                OUTPUT INSERTED.Id
                VALUES
                (
                    @Created, @InitialMeeting, @FirstName, @MiddleName,
                    @LastName, @StreetAddress, @StreetAddress2, @City,
                    @State, @PostalCode, @Classification, @StudentId,
                    @PhoneNumber, @LivingOnCampus, @EnrolledFullTime, @Citizenship,
                    @PellGrantRecipient, @PassportHolder, @Gpa, @CampusEmail,
                    @AlternateEmail, @EnteringYear, @GraduatingYear, @DateOfBirth, @PhiBetaDeltaMember
                )";

            try
            {
                StudentBaseModel baseModel = model as StudentBaseModel;

                if (baseModel.InitialMeeting.HasValue)
                {
                    baseModel.InitialMeeting = baseModel.InitialMeeting.Value.ToUniversalTime();
                }

                if (!string.IsNullOrEmpty(baseModel.MiddleName))
                {
                    baseModel.MiddleName = baseModel.MiddleName.CapitalizeFirstLetter();
                }

                if (!string.IsNullOrEmpty(baseModel.PhoneNumber))
                {
                    baseModel.PhoneNumber = baseModel.PhoneNumber.Trim();
                }

                if (!string.IsNullOrEmpty(baseModel.StudentId))
                {
                    baseModel.StudentId = baseModel.StudentId.Trim();
                }

                if (baseModel.DateOfBirth.HasValue)
                {
                    baseModel.DateOfBirth = baseModel.DateOfBirth.Value.ToUniversalTime();
                }

                if (!string.IsNullOrEmpty(baseModel.CampusEmail))
                {
                    baseModel.CampusEmail = baseModel.CampusEmail.Trim();
                }

                if (!string.IsNullOrEmpty(baseModel.AlternateEmail))
                {
                    baseModel.AlternateEmail = baseModel.AlternateEmail.Trim();
                }

                bool? phiBetaDeltaMember = null;

                if (model is StudentModel)
                {
                    phiBetaDeltaMember = (model as StudentModel).PhiBetaDeltaMember;
                }

                studentId = (await UnitOfWork.Context().QueryAsync<int>(sql,
                    new
                    {
                        Created = DateTime.Now.ToUniversalTime(),
                        FirstName = baseModel.FirstName.CapitalizeFirstLetter(),
                        LastName = baseModel.LastName.CapitalizeFirstLetter(),
                        InitialMeeting = baseModel.InitialMeeting,
                        MiddleName = baseModel.MiddleName,
                        LivingOnCampus = baseModel.LivingOnCampus,
                        StreetAddress = baseModel.StreetAddress,
                        StreetAddress2 = baseModel.StreetAddress2,
                        City = baseModel.City,
                        State = baseModel.State,
                        PostalCode = baseModel.PostalCode,
                        PhoneNumber = baseModel.PhoneNumber,
                        Classification = baseModel.Classification,
                        EnteringYear = baseModel.EnteringYear,
                        GraduatingYear = baseModel.GraduatingYear,
                        StudentId = baseModel.StudentId,
                        DateOfBirth = baseModel.DateOfBirth,
                        Citizenship = baseModel.Citizenship,
                        EnrolledFullTime = baseModel.EnrolledFullTime,
                        PellGrantRecipient = baseModel.PellGrantRecipient,
                        PassportHolder = baseModel.HasPassport,
                        CampusEmail = baseModel.CampusEmail,
                        AlternateEmail = baseModel.AlternateEmail,
                        PhiBetaDeltaMember = phiBetaDeltaMember,
                        Gpa = baseModel.Gpa
                    })).Single();

                await SaveAssociatedTables(studentId, baseModel.SelectedMajors, baseModel.SelectedMinors,
                    baseModel.StudyAbroadCountry, baseModel.StudyAbroadYear, baseModel.StudyAbroadPeriod,
                    baseModel.SelectedLanguages, baseModel.SelectedDesiredLanguages, baseModel.StudiedLanguages);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql.ToString();
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            return studentId;
        }

        public async Task UpdateStudent(StudentModel model)
        {
            const string sql = @"
                UPDATE  [dbo].[Students]
                SET     [Created] = @Created,
                        [InitialMeeting] = @InitialMeeting,
                        [FirstName] = @FirstName,
                        [MiddleName] = @MiddleName,
                        [LastName] = @LastName,
                        [StreetAddress] = @StreetAddress,
                        [StreetAddress2] = @StreetAddress2,
                        [City] = @City,
                        [State] = @State,
                        [PostalCode] = @PostalCode,
                        [Classification] = @Classification,
                        [StudentId] = @StudentId,
                        [PhoneNumber] = @PhoneNumber,
                        [LivingOnCampus] = @LivingOnCampus,
                        [EnrolledFullTime] = @EnrolledFullTime,
                        [Citizenship] = @Citizenship,
                        [PellGrantRecipient] = @PellGrantRecipient,
                        [PassportHolder] = @PassportHolder,
                        [PhiBetaDeltaMember] = @PhiBetaDeltaMember,
                        [Gpa] = @Gpa,
                        [CampusEmail] = @CampusEmail,
                        [AlternateEmail] = @AlternateEmail,
                        [EnteringYear] = @EnteringYear,
                        [GraduatingYear] = @GraduatingYear,
                        [Dob] = @DateOfBirth
                WHERE   [Id] = @Id";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql,
                    new
                    {
                        Created = DateTime.Now.ToUniversalTime(),
                        FirstName = model.FirstName.CapitalizeFirstLetter(),
                        LastName = model.LastName.CapitalizeFirstLetter(),
                        InitialMeeting = model.InitialMeeting,
                        MiddleName = model.MiddleName,
                        LivingOnCampus = model.LivingOnCampus,
                        StreetAddress = model.StreetAddress,
                        StreetAddress2 = model.StreetAddress2,
                        City = model.City,
                        State = model.State,
                        PostalCode = model.PostalCode,
                        PhoneNumber = model.PhoneNumber,
                        Classification = model.Classification,
                        EnteringYear = model.EnteringYear,
                        GraduatingYear = model.GraduatingYear,
                        StudentId = model.StudentId,
                        DateOfBirth = model.DateOfBirth,
                        Citizenship = model.Citizenship,
                        EnrolledFullTime = model.EnrolledFullTime,
                        PellGrantRecipient = model.PellGrantRecipient,
                        PassportHolder = model.HasPassport,
                        CampusEmail = model.CampusEmail,
                        AlternateEmail = model.AlternateEmail,
                        PhiBetaDeltaMember = model.PhiBetaDeltaMember,
                        Gpa = model.Gpa,
                        Id = model.Id
                    });

                await SaveAssociatedTables(model.Id, model.SelectedMajors, model.SelectedMinors,
                    model.StudyAbroadCountry, model.StudyAbroadYear, model.StudyAbroadPeriod,
                    model.SelectedLanguages, model.SelectedDesiredLanguages, model.StudiedLanguages);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql.ToString();
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        private async Task SaveAssociatedTables(int studentId, IEnumerable<int> majors, IEnumerable<int> minors,
            IEnumerable<int> studyAbroadCountry, IEnumerable<int> studyAbroadYear, IEnumerable<int> studyAbroadPeriod,
            IEnumerable<int> languages, IEnumerable<int> desiredLanguages, IEnumerable<int> studiedLanguages)
        {
            // Always call the next two functions. User may be removing
            // all values from a student which previous has some selected.
            await SaveStudentMajors(studentId, majors, true);
            await SaveStudentMajors(studentId, minors, false);

            await SaveStudyAbroadDestinations(studentId, studyAbroadCountry, studyAbroadYear, studyAbroadPeriod);

            await SaveStudentLanguages(studentId, "StudentFluentLanguages", languages);
            await SaveStudentLanguages(studentId, "StudentDesiredLanguages", desiredLanguages);
            await SaveStudentLanguages(studentId, "StudentStudiedLanguages", studiedLanguages);
        }

        private async Task SaveStudentLanguages(int studentId, string tableName, IEnumerable<int> languages)
        {
            string sql = string.Format(@"
                DELETE FROM [dbo].[{0}]
                WHERE       [StudentId] = @StudentId",
                tableName);

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql, new { StudentId = studentId });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            if (languages != null && languages.Any())
            {
                string insertSql = string.Format(@"
                    INSERT INTO [dbo].[{0}]
                    ([StudentId], [LanguageId])
                    VALUES
                    (@StudentId, @LanguageId)",
                    tableName);

                try
                {
                    foreach (int languageId in languages)
                    {
                        await UnitOfWork.Context().ExecuteAsync(insertSql,
                            new
                            {
                                StudentId = studentId,
                                LanguageId = languageId
                            });
                    }
                }
                catch (Exception e)
                {
                    e.Data["SQL"] = insertSql.ToString();
                    ErrorStore.LogException(e, HttpContext.Current);
                    throw e;
                }
            }
        }

        private async Task SaveStudyAbroadDestinations(int studentId, IEnumerable<int> countries,
            IEnumerable<int> years, IEnumerable<int> periods)
        {
            const string sql = @"
                DELETE FROM [dbo].[StudentStudyAbroadWishlist]
                WHERE       [StudentId] = @StudentId";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql, new { StudentId = studentId });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            if (countries == null || years == null || periods == null)
            {
                return;
            }

            int countriesCount = countries.Count();

            // Each collection should have the same number of elements.
            if (countriesCount == 0 || countriesCount != years.Count() || countriesCount != periods.Count())
            {
                return;
            }

            // The default if the user doesn't selecting anything at all is
            // that all three enumerables will have a single element of value
            // zero.
            if (countries.ElementAt(0) == 0 && years.ElementAt(0) == 0 && periods.ElementAt(0) == 0)
            {
                return;
            }

            const string insertSql = @"
                INSERT INTO [dbo].[StudentStudyAbroadWishlist]
                ([StudentId], [CountryId], [Year], [Period])
                VALUES
                (@StudentId, @CountryId, @Year, @Period)";

            try
            {
                for (int i = 0; i < countries.Count(); i++)
                {
                    await UnitOfWork.Context().ExecuteAsync(insertSql,
                        new
                        {
                            StudentId = studentId,
                            CountryId = countries.ElementAt(i),
                            Year = years.ElementAt(i),
                            Period = periods.ElementAt(i)
                        });
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = insertSql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        private async Task SaveStudentMajors(int studentId, IEnumerable<int> majors, bool isMajor)
        {
            const string sql = @"
                DELETE FROM [dbo].[Matriculation]
                WHERE       [StudentId] = @StudentId AND
                            [IsMajor] = @IsMajor";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql,
                    new
                    {
                        StudentId = studentId,
                        IsMajor = isMajor
                    });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            if (majors != null && majors.Any())
            {
                const string insertSql = @"
                    INSERT INTO [dbo].[Matriculation]
                    ([StudentId], [MajorId], [IsMajor])
                    VALUES
                    (@StudentId, @MajorId, @IsMajor)";

                try
                {
                    foreach (int majorId in majors)
                    {
                        await UnitOfWork.Context().ExecuteAsync(insertSql,
                            new
                            {
                                StudentId = studentId,
                                MajorId = majorId,
                                IsMajor = isMajor
                            });
                    }
                }
                catch (Exception e)
                {
                    e.Data["SQL"] = insertSql.ToString();
                    ErrorStore.LogException(e, HttpContext.Current);
                    throw e;
                }
            }
        }
    }
}
