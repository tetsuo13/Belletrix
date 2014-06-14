using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bennett.AbroadAdvisor.Models
{
    /// <summary>
    /// Student model related to student entry by staff.
    /// </summary>
    public class StudentModel : StudentBaseModel, IStudentModel
    {
        [Display(Name = "Phi Beta Delta?")]
        public bool? PhiBetaDeltaMember { get; set; }

        public int NumberOfNotes { get; set; }

        public void SaveChanges(UserModel user)
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

            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;
                connection.Open();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql.ToString();
                        command.Parameters.AddRange(parameters.ToArray());
                        command.ExecuteNonQuery();
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

        public static List<StudentModel> GetStudents(int? id = null)
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            IDictionary<int, StudentModel> students = cacheProvider.Get(CacheId, () => new Dictionary<int, StudentModel>());

            // Select all students by default so they're cached.
            if (students.Count == 0)
            {
                IList<StudentModel> studentList = new List<StudentModel>();

                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            SELECT              s.id, s.created, s.initial_meeting,
                                                s.first_name, s.middle_name, s.last_name,
                                                s.living_on_campus, s.phone_number, s.student_id,
                                                s.dob, s.enrolled_full_time, s.citizenship,
                                                s.pell_grant_recipient, s.passport_holder, s.gpa,
                                                s.campus_email, s.alternate_email, s.graduating_year,
                                                s.classification,
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
                                                s.classification
                            ORDER BY            s.last_name, s.first_name";

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
                                student.PhoneNumber = StringOrDefault(reader, "phone_number");
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
            List<StudentModel> students = GetStudents(id);
            return students[0];
        }

        private static void PopulateStudentLanguages(NpgsqlConnection connection, ref IList<StudentModel> students)
        {
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT  language_id
                    FROM    student_fluent_languages
                    WHERE   student_id = @StudentId";

                command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer);
                command.Prepare();

                for (int i = 0; i < students.Count; i++)
                {
                    List<int> languages = new List<int>();
                    command.Parameters[0].Value = students[i].Id;

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            languages.Add(reader.GetInt32(reader.GetOrdinal("language_id")));
                        }
                    }

                    students[i].SelectedLanguages = languages.AsEnumerable();
                }
            }
        }

        private static void PopulateDesiredStudentLanguages(NpgsqlConnection connection, ref IList<StudentModel> students)
        {
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT  language_id
                    FROM    student_desired_languages
                    WHERE   student_id = @StudentId";

                command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer);
                command.Prepare();

                for (int i = 0; i < students.Count; i++)
                {
                    List<int> languages = new List<int>();
                    command.Parameters[0].Value = students[i].Id;

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            languages.Add(reader.GetInt32(reader.GetOrdinal("language_id")));
                        }
                    }

                    students[i].SelectedDesiredLanguages = languages.AsEnumerable();
                }
            }
        }

        private static void PopulateStudiedLanguages(NpgsqlConnection connection, ref IList<StudentModel> students)
        {
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT  language_id
                    FROM    student_studied_languages
                    WHERE   student_id = @StudentId";

                command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer);
                command.Prepare();

                for (int i = 0; i < students.Count; i++)
                {
                    IList<int> languages = new List<int>();
                    command.Parameters[0].Value = students[i].Id;

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            languages.Add(reader.GetInt32(reader.GetOrdinal("language_id")));
                        }
                    }

                    students[i].StudiedLanguages = languages.AsEnumerable();
                }
            }
        }

        private static void PopulateStudentMajorsMinors(NpgsqlConnection connection, ref IList<StudentModel> students)
        {
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT  major_id, is_major
                    FROM    matriculation
                    WHERE   student_id = @StudentId";

                command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer);
                command.Prepare();

                for (int i = 0; i < students.Count; i++)
                {
                    List<int> majors = new List<int>();
                    List<int> minors = new List<int>();
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

                    students[i].SelectedMajors = majors.AsEnumerable();
                    students[i].SelectedMinors = minors.AsEnumerable();
                }
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
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT  country_id, year, period
                    FROM    student_study_abroad_wishlist
                    WHERE   student_id = @StudentId";

                command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer);
                command.Prepare();

                for (int i = 0; i < students.Count; i++)
                {
                    IList<int> countries = new List<int>();
                    IList<int> years = new List<int>();
                    IList<int> periods = new List<int>();

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

                    students[i].StudyAbroadCountry = countries.AsEnumerable();
                    students[i].StudyAbroadYear = years.AsEnumerable();
                    students[i].StudyAbroadPeriod = periods.AsEnumerable();
                }
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

            IEnumerable<StudentModel> students = new List<StudentModel>(GetStudents()).ToList();

            if (filterByGraduatingYears)
            {
                students = students
                    .Where(x => x.GraduatingYear.HasValue)
                    .Where(x => search.SelectedGraduatingYears.Any(y => y == x.GraduatingYear.Value))
                    .ToList();
            }

            if (filterByMajors)
            {
                students = students
                    .Where(x => x.SelectedMajors.Count<int>() > 0)
                    .Where(x => x.SelectedMajors.Intersect(search.SelectedMajors).Count<int>() > 0)
                    .ToList();
            }

            if (filterByCountries)
            {
                IEnumerable<StudyAbroadModel> studyAbroad = StudyAbroadModel.GetAll();

                students = studyAbroad
                    .Where(x => search.SelectedCountries.Any(y => y == x.CountryId))
                    .Select(x => x.Student)
                    .ToList();

                //students = students
                //    .Where(x => x.StudyAbroadCountry.Count<int>() > 0)
                //    .Where(x => x.StudyAbroadCountry.Intersect(search.SelectedCountries).Count<int>() > 0)
                //    .ToList();
            }

            return students;
        }
    }
}
