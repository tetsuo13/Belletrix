using Belletrix.Core;
using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    if (id.HasValue)
                    {
                        command.Parameters.Add("@StudentId", SqlDbType.Int).Value = id.Value;
                    }

                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            StudentModel student = new StudentModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };

                            student.MiddleName = await reader.GetValueOrDefault<string>("MiddleName");
                            student.LivingOnCampus = await reader.GetValueOrDefault<bool?>("LivingOnCampus");
                            student.StreetAddress = await reader.GetValueOrDefault<string>("StreetAddress");
                            student.StreetAddress2 = await reader.GetValueOrDefault<string>("StreetAddress2");
                            student.City = await reader.GetValueOrDefault<string>("City");
                            student.State = await reader.GetValueOrDefault<string>("State");
                            student.PostalCode = await reader.GetValueOrDefault<string>("PostalCode");
                            student.PhoneNumber = await reader.GetValueOrDefault<string>("PhoneNumber");
                            student.EnteringYear = await reader.GetValueOrDefault<int?>("EnteringYear");
                            student.GraduatingYear = await reader.GetValueOrDefault<int?>("GraduatingYear");
                            student.StudentId = await reader.GetValueOrDefault<string>("StudentId");
                            student.EnrolledFullTime = await reader.GetValueOrDefault<bool?>("EnrolledFullTime");
                            student.Citizenship = await reader.GetValueOrDefault<int?>("Citizenship");
                            student.PellGrantRecipient = await reader.GetValueOrDefault<bool?>("PellGrantRecipient");
                            student.HasPassport = await reader.GetValueOrDefault<bool?>("PassportHolder");
                            student.CampusEmail = await reader.GetValueOrDefault<string>("CampusEmail");
                            student.AlternateEmail = await reader.GetValueOrDefault<string>("AlternateEmail");
                            student.Created = await reader.GetFieldValueAsync<DateTime>(reader.GetOrdinal("Created"));
                            student.NumberOfNotes = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("NumNotes"));
                            student.Gpa = await reader.GetValueOrDefault<double?>("Gpa");

                            int ord = reader.GetOrdinal("Dob");
                            if (!reader.IsDBNull(ord))
                            {
                                student.DateOfBirth = DateTimeFilter.UtcToLocal(await reader.GetFieldValueAsync<DateTime>(ord));
                            }

                            ord = reader.GetOrdinal("InitialMeeting");
                            if (!reader.IsDBNull(ord))
                            {
                                student.InitialMeeting = DateTimeFilter.UtcToLocal(await reader.GetFieldValueAsync<DateTime>(ord));
                            }

                            ord = reader.GetOrdinal("MajorIds");
                            if (!reader.IsDBNull(ord))
                            {
                                student.SelectedMajors = (await reader.GetFieldValueAsync<string>(ord)).Split(',').Cast<int>();
                            }

                            ord = reader.GetOrdinal("MinorIds");
                            if (!reader.IsDBNull(ord))
                            {
                                student.SelectedMinors = (await reader.GetFieldValueAsync<string>(ord)).Split(',').Cast<int>();
                            }

                            ord = reader.GetOrdinal("StudiedLanguageIds");
                            if (!reader.IsDBNull(ord))
                            {
                                student.StudiedLanguages = (await reader.GetFieldValueAsync<string>(ord)).Split(',').Cast<int>();
                            }

                            ord = reader.GetOrdinal("DesiredLanguageIds");
                            if (!reader.IsDBNull(ord))
                            {
                                student.SelectedDesiredLanguages = (await reader.GetFieldValueAsync<string>(ord)).Split(',').Cast<int>();
                            }

                            ord = reader.GetOrdinal("FluentLanguageIds");
                            if (!reader.IsDBNull(ord))
                            {
                                student.SelectedLanguages = (await reader.GetFieldValueAsync<string>(ord)).Split(',').Cast<int>();
                            }

                            ord = reader.GetOrdinal("StudyAbroadCountryIds");
                            if (!reader.IsDBNull(ord))
                            {
                                student.StudyAbroadCountry = (await reader.GetFieldValueAsync<string>(ord)).Split(',').Cast<int>();
                                student.StudyAbroadYear = (await reader.GetFieldValueAsync<string>(reader.GetOrdinal("StudyAbroadYearIds"))).Split(',').Cast<int>();
                                student.StudyAbroadPeriod = (await reader.GetFieldValueAsync<string>(reader.GetOrdinal("StudyAbroadPeriodIds"))).Split(',').Cast<int>();
                            }

                            student.PromoIds = StudentPromoLog.GetPromoIdsForStudent(student.Id);

                            students.Add(student);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
            }

            return students;
        }

        public async Task<IEnumerable<CountryModel>> GetCountries()
        {
            ICollection<CountryModel> countries = new List<CountryModel>();

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
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            countries.Add(new CountryModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                                Name = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Name")),
                                Abbreviation = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Abbreviation"))
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
            }

            return countries;
        }

        public async Task<IEnumerable<CountryModel>> GetRegions()
        {
            ICollection<CountryModel> regions = new List<CountryModel>();

            const string sql = @"
                SELECT      [Id], [Name], [Abbreviation]
                FROM        [dbo].[Countries]
                WHERE       [IsRegion] = 1
                ORDER BY    [Name]";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            regions.Add(new CountryModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                                Name = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Name")),
                                Abbreviation = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Abbreviation"))
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
            }

            return regions;
        }

        public async Task<IEnumerable<LanguageModel>> GetLanguages()
        {
            ICollection<LanguageModel> languages = new List<LanguageModel>();

            const string sql = @"
                SELECT      [Id], [Name]
                FROM        [dbo].[Languages]
                ORDER BY    [Name]";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            languages.Add(new LanguageModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                                Name = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Name"))
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
            }

            return languages;
        }

        public async Task<IEnumerable<MajorsModel>> GetMajors()
        {
            ICollection<MajorsModel> majors = new List<MajorsModel>();

            const string sql = @"
                SELECT      [Id], [Name]
                FROM        [Majors]
                ORDER BY    [Name]";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            majors.Add(new MajorsModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                                Name = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Name"))
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
            }

            return majors;
        }

        public async Task<IEnumerable<MinorsModel>> GetMinors()
        {
            ICollection<MinorsModel> minors = new List<MinorsModel>();

            const string sql = @"
                SELECT      [Id], [Name]
                FROM        [Minors]
                ORDER BY    [Name]";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            minors.Add(new MinorsModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                                Name = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Name"))
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
            }

            return minors;
        }

        public async Task<IEnumerable<ProgramModel>> GetPrograms()
        {
            ICollection<ProgramModel> programs = new List<ProgramModel>();

            const string sql = @"
                SELECT      [Id], [Name], [Abbreviation]
                FROM        [Programs]
                ORDER BY    [Name]";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ProgramModel program = new ProgramModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                                Name = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Name")),
                                Abbreviation = await reader.GetValueOrDefault<string>("Abbreviation")
                            };

                            programs.Add(program);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
            }

            return programs;
        }

        public async Task<IEnumerable<ProgramTypeModel>> GetProgramTypes()
        {
            ICollection<ProgramTypeModel> programTypes = new List<ProgramTypeModel>();
            const string sql = @"
                SELECT      [Id], [Name], [ShortTerm]
                FROM        [ProgramTypes]
                ORDER BY    [Name]";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            programTypes.Add(new ProgramTypeModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                                Name = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Name")),
                                ShortTerm = await reader.GetFieldValueAsync<bool>(reader.GetOrdinal("ShortTerm"))
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
            }

            return programTypes;
        }
    }
}
