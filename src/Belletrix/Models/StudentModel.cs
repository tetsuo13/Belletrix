using Belletrix.Core;
using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Belletrix.Models
{
    /// <summary>
    /// Student model related to student entry by staff.
    /// </summary>
    public class StudentModel : StudentBaseModel, IStudentModel
    {
        [Display(Name = "Phi Beta Delta?")]
        public bool? PhiBetaDeltaMember { get; set; }

        public int NumberOfNotes { get; set; }

        /// <summary>
        /// Set of promos that the student may be associated with.
        /// </summary>
        [Display(Name = "Promo")]
        public IEnumerable<int> PromoIds { get; set; }

        public void SaveChanges(UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    StringBuilder sql = new StringBuilder("UPDATE [dbo].[Students] SET ");

                    PrepareColumns(ref sql);

                    if (PhiBetaDeltaMember.HasValue)
                    {
                        AddParameter(sql, "PhiBetaDeltaMember", SqlDbType.Bit, PhiBetaDeltaMember, 0);
                    }

                    foreach (KeyValuePair<string, string> pair in columns)
                    {
                        sql.Append(String.Format("{0} = {1}, ", pair.Key, pair.Value));
                    }

                    // Remove the trailing comma and space.
                    sql.Length -= 2;

                    sql.Append(" WHERE [Id] = @Id");
                    parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = Id });

                    try
                    {
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = sql.ToString();
                            command.Parameters.AddRange(parameters.ToArray());
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception e)
                    {
                        e.Data["SQL"] = sql;
                        throw e;
                    }

                    // Always call the next two functions. User may be
                    // removing all values from a student which previous has
                    // some selected.
                    SaveStudentMajors(connection, transaction, Id, SelectedMajors, true);
                    SaveStudentMajors(connection, transaction, Id, SelectedMinors, false);
                    SaveStudyAbroadDestinations(connection, transaction, Id, StudyAbroadCountry, StudyAbroadYear, StudyAbroadPeriod);

                    SaveStudentLanguages(connection, transaction, Id, "StudentFluentLanguages", SelectedLanguages);
                    SaveStudentLanguages(connection, transaction, Id, "StudentDesiredLanguages", SelectedDesiredLanguages);
                    SaveStudentLanguages(connection, transaction, Id, "StudentStudiedLanguages", StudiedLanguages);

                    StudentPromoLog.Save(connection, transaction, Id, PromoIds);

                    EventLogModel eventLog = new EventLogModel()
                    {
                        Student = this,
                        ModifiedById = user.Id,
                        ModifiedByFirstName = user.FirstName,
                        ModifiedByLastName = user.LastName
                    };
                    eventLog.AddStudentEvent(connection, transaction, user.Id, Id, EventLogModel.EventType.EditStudent);

                    transaction.Commit();
                }
            }
        }

        public static IEnumerable<StudentModel> GetStudents(int? id = null)
        {
            IDictionary<int, StudentModel> students = new Dictionary<int, StudentModel>();

            const string sql = @"
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
                                    s.Id = n.StudentId
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

            IList<StudentModel> studentList = new List<StudentModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                StudentModel student = new StudentModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                };

                                student.MiddleName = StringOrDefault(reader, "MiddleName");
                                student.LivingOnCampus = BoolOrDefault(reader, "LivingOnCampus");
                                student.StreetAddress = StringOrDefault(reader, "StreetAddress");
                                student.StreetAddress2 = StringOrDefault(reader, "StreetAddress2");
                                student.City = StringOrDefault(reader, "City");
                                student.State = StringOrDefault(reader, "State");
                                student.PostalCode = StringOrDefault(reader, "PostalCode");
                                student.PhoneNumber = StringOrDefault(reader, "PhoneNumber");
                                student.EnteringYear = IntOrDefault(reader, "EnteringYear");
                                student.GraduatingYear = IntOrDefault(reader, "GraduatingYear");
                                student.StudentId = StringOrDefault(reader, "StudentId");
                                student.EnrolledFullTime = BoolOrDefault(reader, "EnrolledFullTime");
                                student.Citizenship = IntOrDefault(reader, "Citizenship");
                                student.PellGrantRecipient = BoolOrDefault(reader, "PellGrantRecipient");
                                student.HasPassport = BoolOrDefault(reader, "PassportHolder");
                                student.CampusEmail = StringOrDefault(reader, "CampusEmail");
                                student.AlternateEmail = StringOrDefault(reader, "AlternateEmail");
                                student.Created = reader.GetDateTime(reader.GetOrdinal("Created"));
                                student.NumberOfNotes = (int)reader.GetInt32(reader.GetOrdinal("NumNotes"));

                                int ord = reader.GetOrdinal("Gpa");
                                if (!reader.IsDBNull(ord))
                                {
                                    student.Gpa = Convert.ToDouble(reader["Gpa"]);
                                }

                                ord = reader.GetOrdinal("Dob");
                                if (!reader.IsDBNull(ord))
                                {
                                    student.DateOfBirth = DateTimeFilter.UtcToLocal(reader.GetDateTime(ord));
                                }

                                ord = reader.GetOrdinal("InitialMeeting");
                                if (!reader.IsDBNull(ord))
                                {
                                    student.InitialMeeting = DateTimeFilter.UtcToLocal(reader.GetDateTime(ord));
                                }

                                student.PromoIds = StudentPromoLog.GetPromoIdsForStudent(student.Id);

                                studentList.Add(student);
                            }
                        }
                    }

                    PopulateStudentMajorsMinors(connection, ref studentList);
                    PopulateStudentLanguages(connection, ref studentList);
                    PopulateDesiredStudentLanguages(connection, ref studentList);
                    PopulateStudiedLanguages(connection, ref studentList);
                    PopulateStudyAbroadDestinations(connection, ref studentList);
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }

            foreach (StudentModel student in studentList)
            {
                students.Add(student.Id, student);
            }

            if (id.HasValue)
            {
                return students.Where(x => x.Value.Id == id.Value).Select(x => x.Value).ToList();
            }

            return students.Select(x => x.Value).ToList();
        }

        public static StudentModel GetStudent(int id)
        {
            IEnumerable<StudentModel> students = GetStudents(id);
            return students.First();
        }

        /// <summary>
        /// All students associated with a specific promo.
        /// </summary>
        /// <param name="promoId">Promo ID.</param>
        /// <returns>Student collection from promo.</returns>
        public static IEnumerable<StudentModel> FromPromo(int promoId)
        {
            IEnumerable<StudentModel> students = GetStudents();
            return students.Where(x => x.PromoIds != null && x.PromoIds.Any(y => y == promoId));
        }

        private static void PopulateStudentLanguages(SqlConnection connection, ref IList<StudentModel> students)
        {
            const string sql = @"
                SELECT  [LanguageId]
                FROM    [dbo].[StudentFluentLanguages]
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
                        ICollection<int> languages = new List<int>();
                        command.Parameters[0].Value = students[i].Id;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                languages.Add(reader.GetInt32(reader.GetOrdinal("LanguageId")));
                            }
                        }

                        students[i].SelectedLanguages = languages;
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        private static void PopulateDesiredStudentLanguages(SqlConnection connection, ref IList<StudentModel> students)
        {
            const string sql = @"
                SELECT  [LanguageId]
                FROM    [StudentDesiredLanguages]
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
                        ICollection<int> languages = new List<int>();
                        command.Parameters[0].Value = students[i].Id;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                languages.Add(reader.GetInt32(reader.GetOrdinal("LanguageId")));
                            }
                        }

                        students[i].SelectedDesiredLanguages = languages;
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        private static void PopulateStudiedLanguages(SqlConnection connection, ref IList<StudentModel> students)
        {
            const string sql = @"
                SELECT  [LanguageId]
                FROM    [StudentStudiedLanguages]
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
                        ICollection<int> languages = new List<int>();
                        command.Parameters[0].Value = students[i].Id;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                languages.Add(reader.GetInt32(reader.GetOrdinal("LanguageId")));
                            }
                        }

                        students[i].StudiedLanguages = languages;
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
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

        public void Save(UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    base.Save(connection, transaction, user.Id);

                    StudentPromoLog.Save(connection, transaction, Id, PromoIds);

                    EventLogModel eventLog = new EventLogModel()
                    {
                        Student = this,
                        ModifiedById = user.Id,
                        ModifiedByFirstName = user.FirstName,
                        ModifiedByLastName = user.LastName
                    };
                    eventLog.AddStudentEvent(connection, transaction, user.Id, Id, EventLogModel.EventType.AddStudent);

                    transaction.Commit();
                }
            }
        }

        private static void PopulateStudyAbroadDestinations(SqlConnection connection, ref IList<StudentModel> students)
        {
            const string sql = @"
                SELECT  [CountryId], [Year], [Period]
                FROM    [StudentStudyAbroadWishlist]
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
                        ICollection<int> countries = new List<int>();
                        ICollection<int> years = new List<int>();
                        ICollection<int> periods = new List<int>();

                        command.Parameters[0].Value = students[i].Id;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                countries.Add(reader.GetInt32(reader.GetOrdinal("CountryId")));
                                years.Add(reader.GetInt32(reader.GetOrdinal("Year")));
                                periods.Add(reader.GetInt32(reader.GetOrdinal("Period")));
                            }
                        }

                        students[i].StudyAbroadCountry = countries;
                        students[i].StudyAbroadYear = years;
                        students[i].StudyAbroadPeriod = periods;
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        public static IEnumerable<StudentModel> Search(StudentSearchModel search)
        {
            bool filterByGraduatingYears = search.SelectedGraduatingYears != null && search.SelectedGraduatingYears.Count<int>() > 0;
            bool filterByMajors = search.SelectedMajors != null && search.SelectedMajors.Count<int>() > 0;
            bool filterByCountries = search.SelectedCountries != null && search.SelectedCountries.Count<int>() > 0;

            if (!filterByGraduatingYears && !filterByMajors && !filterByCountries)
            {
                return Enumerable.Empty<StudentModel>();
            }

            IEnumerable<StudentModel> students = new List<StudentModel>(GetStudents());

            if (filterByGraduatingYears)
            {
                students = students
                    .Where(x => x.GraduatingYear.HasValue)
                    .Where(x => search.SelectedGraduatingYears.Any(y => y == x.GraduatingYear.Value));
            }

            if (filterByMajors)
            {
                students = students
                    .Where(x => x.SelectedMajors.Count<int>() > 0)
                    .Where(x => x.SelectedMajors.Intersect(search.SelectedMajors).Count<int>() > 0);
            }

            if (filterByCountries)
            {
                IEnumerable<StudyAbroadModel> studyAbroad = StudyAbroadModel.GetAll();

                students = studyAbroad
                    .Where(x => search.SelectedCountries.Any(y => y == x.CountryId))
                    .Select(x => x.Student);

                //students = students
                //    .Where(x => x.StudyAbroadCountry.Count<int>() > 0)
                //    .Where(x => x.StudyAbroadCountry.Intersect(search.SelectedCountries).Count<int>() > 0);
            }

            return students;
        }

        public static IEnumerable<StudentModel> SearchByFullName(string firstName, string lastName)
        {
            return GetStudents().Where(x =>
                {
                    return String.Equals(x.FirstName, firstName, StringComparison.InvariantCultureIgnoreCase) &&
                        String.Equals(x.LastName, lastName, StringComparison.InvariantCultureIgnoreCase);
                });
        }
    }
}
