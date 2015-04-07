using Belletrix.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;
                connection.Open();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    StringBuilder sql = new StringBuilder("UPDATE students SET ");

                    PrepareColumns(ref sql);

                    if (PhiBetaDeltaMember.HasValue)
                    {
                        AddParameter(sql, "phi_beta_delta_member", NpgsqlTypes.NpgsqlDbType.Boolean, PhiBetaDeltaMember, 0);
                    }

                    foreach (KeyValuePair<string, string> pair in columns)
                    {
                        sql.Append(String.Format("{0} = {1}, ", pair.Key, pair.Value));
                    }

                    // Remove the trailing comma and space.
                    sql.Length -= 2;

                    sql.Append(" WHERE id = @Id");
                    parameters.Add(new NpgsqlParameter("@Id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = Id });

                    try
                    {
                        using (NpgsqlCommand command = connection.CreateCommand())
                        {
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
                    SaveStudentMajors(connection, Id, SelectedMajors, true);
                    SaveStudentMajors(connection, Id, SelectedMinors, false);
                    SaveStudyAbroadDestinations(connection, Id, StudyAbroadCountry, StudyAbroadYear, StudyAbroadPeriod);

                    SaveStudentLanguages(connection, Id, "student_fluent_languages", SelectedLanguages);
                    SaveStudentLanguages(connection, Id, "student_desired_languages", SelectedDesiredLanguages);
                    SaveStudentLanguages(connection, Id, "student_studied_languages", StudiedLanguages);

                    StudentPromoLog.Save(connection, Id, PromoIds);

                    EventLogModel eventLog = new EventLogModel()
                    {
                        Student = this,
                        ModifiedBy = user
                    };
                    eventLog.AddStudentEvent(connection, user.Id, Id, EventLogModel.EventType.EditStudent);

                    transaction.Commit();

                    ApplicationCache cacheProvider = new ApplicationCache();
                    IDictionary<int, StudentModel> students = cacheProvider.Get(CacheId, () => new Dictionary<int, StudentModel>());
                    students[Id] = this;
                    cacheProvider.Set(CacheId, students);
                }
            }
        }

        public static IEnumerable<StudentModel> GetStudents(int? id = null)
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            IDictionary<int, StudentModel> students = cacheProvider.Get(CacheId, () => new Dictionary<int, StudentModel>());

            // Select all students by default so they're cached.
            if (students.Count == 0)
            {
                const string sql = @"
                    SELECT              s.id, s.created, s.initial_meeting,
                                        s.first_name, s.middle_name, s.last_name,
                                        s.living_on_campus, s.phone_number, s.student_id,
                                        s.dob, s.enrolled_full_time, s.citizenship,
                                        s.pell_grant_recipient, s.passport_holder, s.gpa,
                                        s.campus_email, s.alternate_email, s.graduating_year,
                                        s.classification, s.street_address, s.street_address2,
                                        s.city, s.state, s.postal_code,
                                        s.entering_year,
                                        COUNT(n.id) AS num_notes
                    FROM                students s
                    LEFT OUTER JOIN     student_notes n ON
                                        s.id = n.student_id
                    GROUP BY            s.id, s.created, s.initial_meeting,
                                        s.first_name, s.middle_name, s.last_name,
                                        s.living_on_campus, s.phone_number, s.student_id,
                                        s.dob, s.enrolled_full_time, s.citizenship,
                                        s.pell_grant_recipient, s.passport_holder, s.gpa,
                                        s.campus_email, s.alternate_email, s.graduating_year,
                                        s.classification, s.street_address, s.street_address2,
                                        s.city, s.state, s.postal_code,
                                        s.entering_year
                    ORDER BY            s.last_name, s.first_name";

                IList<StudentModel> studentList = new List<StudentModel>();

                try
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                    {
                        connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                        using (NpgsqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = sql;
                            connection.Open();

                            using (NpgsqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    StudentModel student = new StudentModel()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                                        FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                                        LastName = reader.GetString(reader.GetOrdinal("last_name"))
                                    };

                                    student.MiddleName = StringOrDefault(reader, "middle_name");
                                    student.LivingOnCampus = BoolOrDefault(reader, "living_on_campus");
                                    student.StreetAddress = StringOrDefault(reader, "street_address");
                                    student.StreetAddress2 = StringOrDefault(reader, "street_address2");
                                    student.City = StringOrDefault(reader, "city");
                                    student.State = StringOrDefault(reader, "state");
                                    student.PostalCode = StringOrDefault(reader, "postal_code");
                                    student.PhoneNumber = StringOrDefault(reader, "phone_number");
                                    student.EnteringYear = IntOrDefault(reader, "entering_year");
                                    student.GraduatingYear = IntOrDefault(reader, "graduating_year");
                                    student.StudentId = StringOrDefault(reader, "student_id");
                                    student.EnrolledFullTime = BoolOrDefault(reader, "enrolled_full_time");
                                    student.Citizenship = IntOrDefault(reader, "citizenship");
                                    student.PellGrantRecipient = BoolOrDefault(reader, "pell_grant_recipient");
                                    student.HasPassport = BoolOrDefault(reader, "passport_holder");
                                    student.CampusEmail = StringOrDefault(reader, "campus_email");
                                    student.AlternateEmail = StringOrDefault(reader, "alternate_email");
                                    student.Created = reader.GetDateTime(reader.GetOrdinal("created"));
                                    student.NumberOfNotes = (int)reader.GetInt64(reader.GetOrdinal("num_notes"));

                                    int ord = reader.GetOrdinal("gpa");
                                    if (!reader.IsDBNull(ord))
                                    {
                                        student.Gpa = Convert.ToDouble(reader["gpa"]);
                                    }

                                    ord = reader.GetOrdinal("dob");
                                    if (!reader.IsDBNull(ord))
                                    {
                                        student.DateOfBirth = DateTimeFilter.UtcToLocal(reader.GetDateTime(ord));
                                    }

                                    ord = reader.GetOrdinal("initial_meeting");
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

                cacheProvider.Set(CacheId, students);
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

        private static void PopulateStudentLanguages(NpgsqlConnection connection, ref IList<StudentModel> students)
        {
            const string sql = @"
                SELECT  language_id
                FROM    student_fluent_languages
                WHERE   student_id = @StudentId";

            try
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Prepare();

                    for (int i = 0; i < students.Count; i++)
                    {
                        ICollection<int> languages = new List<int>();
                        command.Parameters[0].Value = students[i].Id;

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                languages.Add(reader.GetInt32(reader.GetOrdinal("language_id")));
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

        private static void PopulateDesiredStudentLanguages(NpgsqlConnection connection, ref IList<StudentModel> students)
        {
            const string sql = @"
                SELECT  language_id
                FROM    student_desired_languages
                WHERE   student_id = @StudentId";

            try
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Prepare();

                    for (int i = 0; i < students.Count; i++)
                    {
                        ICollection<int> languages = new List<int>();
                        command.Parameters[0].Value = students[i].Id;

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                languages.Add(reader.GetInt32(reader.GetOrdinal("language_id")));
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

        private static void PopulateStudiedLanguages(NpgsqlConnection connection, ref IList<StudentModel> students)
        {
            const string sql = @"
                SELECT  language_id
                FROM    student_studied_languages
                WHERE   student_id = @StudentId";

            try
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Prepare();

                    for (int i = 0; i < students.Count; i++)
                    {
                        ICollection<int> languages = new List<int>();
                        command.Parameters[0].Value = students[i].Id;

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                languages.Add(reader.GetInt32(reader.GetOrdinal("language_id")));
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

        private static void PopulateStudentMajorsMinors(NpgsqlConnection connection, ref IList<StudentModel> students)
        {
            const string sql = @"
                SELECT  major_id, is_major
                FROM    matriculation
                WHERE   student_id = @StudentId";

            try
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Prepare();

                    for (int i = 0; i < students.Count; i++)
                    {
                        ICollection<int> majors = new List<int>();
                        ICollection<int> minors = new List<int>();
                        command.Parameters[0].Value = students[i].Id;

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int majorId = reader.GetInt32(reader.GetOrdinal("major_id"));

                                if (reader.GetBoolean(reader.GetOrdinal("is_major")))
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
            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;
                connection.Open();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    base.Save(connection, user.Id);

                    StudentPromoLog.Save(connection, Id, PromoIds);

                    EventLogModel eventLog = new EventLogModel()
                    {
                        Student = this,
                        ModifiedBy = user
                    };
                    eventLog.AddStudentEvent(connection, user.Id, Id, EventLogModel.EventType.AddStudent);

                    transaction.Commit();
                }
            }
        }

        private static void PopulateStudyAbroadDestinations(NpgsqlConnection connection, ref IList<StudentModel> students)
        {
            const string sql = @"
                SELECT  country_id, year, period
                FROM    student_study_abroad_wishlist
                WHERE   student_id = @StudentId";

            try
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Prepare();

                    for (int i = 0; i < students.Count; i++)
                    {
                        ICollection<int> countries = new List<int>();
                        ICollection<int> years = new List<int>();
                        ICollection<int> periods = new List<int>();

                        command.Parameters[0].Value = students[i].Id;

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                countries.Add(reader.GetInt32(reader.GetOrdinal("country_id")));
                                years.Add(reader.GetInt32(reader.GetOrdinal("year")));
                                periods.Add(reader.GetInt32(reader.GetOrdinal("period")));
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
