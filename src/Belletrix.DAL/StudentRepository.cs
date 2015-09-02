using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Belletrix.Core;
using System.Data;

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
    }
}
