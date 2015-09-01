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

                            student.PromoIds = StudentPromoLog.GetPromoIdsForStudent(student.Id);

                            students.Add(student);
                        }
                    }
                }

                PopulateStudentMajorsMinors(connection, ref studentList);
                PopulateStudentLanguages(connection, ref studentList);
                PopulateDesiredStudentLanguages(connection, ref studentList);
                PopulateStudiedLanguages(connection, ref studentList);
                PopulateStudyAbroadDestinations(connection, ref studentList);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
            }

            return students;
        }

        private static void PopulateStudentMajorsMinors(SqlConnection connection, ref IList<StudentModel> students)
        {
            const string sql = @"
                SELECT  [MajorId], [IsMajor]
                FROM    [dbo].[Matriculation]
                WHERE   [StudentId] = @StudentId";

            try
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", SqlDbType.Int);
                    command.Prepare();

                    for (int i = 0; i < students.Count; i++)
                    {
                        ICollection<int> majors = new List<int>();
                        ICollection<int> minors = new List<int>();
                        command.Parameters[0].Value = students[i].Id;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int majorId = reader.GetInt32(reader.GetOrdinal("MajorId"));

                                if (reader.GetBoolean(reader.GetOrdinal("IsMajor")))
                                {
                                    majors.Add(majorId);
                                }
                                else
                                {
                                    minors.Add(majorId);
                                }
                            }
                        }

                        students[i].SelectedMajors = majors;
                        students[i].SelectedMinors = minors;
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
