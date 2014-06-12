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
    /// Standard student model.
    /// </summary>
    public class StudentBaseModel
    {
        protected const string CacheId = "Students";

        [Key]
        public int Id { get; set; }

        public DateTime Created { get; set; }

        [Display(Name = "Initial Meeting")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? InitialMeeting { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(64)]
        public string FirstName { get; set; }

        [StringLength(64)]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(64)]
        public string LastName { get; set; }

        [Display(Name = "Living")]
        public bool? LivingOnCampus { get; set; }

        [StringLength(32)]
        [Display(Name = "Telephone #")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Range(1900, 3000)]
        [Display(Name = "Graduating Year")]
        public int? GraduatingYear { get; set; }

        [Range(0, 3)]
        [Display(Name = "Classification")]
        public int? Classification { get; set; }

        [StringLength(32)]
        [Display(Name = "Student ID")]
        public string StudentId { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Enrolled as a")]
        public bool? EnrolledFullTime { get; set; }

        public int? Citizenship { get; set; }

        [Display(Name = "Pell Grant?")]
        public bool? PellGrantRecipient { get; set; }

        [Display(Name = "Passport Holder?")]
        public bool? HasPassport { get; set; }

        [Range(0.00, 9.99)]
        [Display(Name = "Current GPA")]
        public double? Gpa { get; set; }

        [StringLength(128)]
        [EmailAddress]
        [Display(Name = "Bennett College Email")]
        [DataType(DataType.EmailAddress)]
        public string CampusEmail { get; set; }

        [StringLength(128)]
        [EmailAddress]
        [Display(Name = "Alternative Email")]
        [DataType(DataType.EmailAddress)]
        public string AlternateEmail { get; set; }

        [Display(Name = "Majors")]
        public IEnumerable<int> SelectedMajors { get; set; }

        [Display(Name = "Minors")]
        public IEnumerable<int> SelectedMinors { get; set; }

        [Display(Name = "Fluency")]
        public IEnumerable<int> SelectedLanguages { get; set; }

        [Display(Name = "Studied")]
        public IEnumerable<int> StudiedLanguages { get; set; }

        [Display(Name = "Country")]
        public IEnumerable<int> StudyAbroadCountry { get; set; }

        [Display(Name = "Year")]
        public IEnumerable<int> StudyAbroadYear { get; set; }

        /// <summary>
        /// Not used by promos.
        /// </summary>
        [Display(Name = "Desired Language Abroad")]
        public IEnumerable<int> SelectedDesiredLanguages { get; set; }

        /// <summary>
        /// Values will be indexes from
        /// StudentStudyAbroadWishlistModel.PeriodValue
        /// </summary>
        [Display(Name = "Semester")]
        public IEnumerable<int> StudyAbroadPeriod { get; set; }

        protected IDictionary<string, string> columns;
        protected IList<NpgsqlParameter> parameters;

        protected static string StringOrDefault(NpgsqlDataReader reader, string column)
        {
            int ord = reader.GetOrdinal(column);

            if (reader.IsDBNull(ord))
            {
                return null;
            }

            return reader.GetString(ord);
        }

        protected static bool? BoolOrDefault(NpgsqlDataReader reader, string column)
        {
            int ord = reader.GetOrdinal(column);

            if (reader.IsDBNull(ord))
            {
                return null;
            }

            return reader.GetBoolean(ord);
        }

        protected static int? IntOrDefault(NpgsqlDataReader reader, string column)
        {
            int ord = reader.GetOrdinal(column);

            if (reader.IsDBNull(ord))
            {
                return null;
            }

            return reader.GetInt32(ord);
        }

        protected void AddParameter(StringBuilder sql, string columnName, NpgsqlTypes.NpgsqlDbType columnType,
            object columnValue, int columnLength)
        {
            string parameterName = String.Format("@{0}", columnName);
            columns.Add(columnName, parameterName);

            if (columnLength > 0)
            {
                parameters.Add(new NpgsqlParameter(parameterName, columnType, columnLength) { Value = columnValue });
            }
            else
            {
                parameters.Add(new NpgsqlParameter(parameterName, columnType) { Value = columnValue });
            }
        }

        protected string CapitalizeFirstLetter(string value)
        {
            value = value.Trim();

            if (value.Length == 1)
            {
                return value.ToUpper();
            }

            return value.Substring(0, 1).ToUpper() + value.Substring(1);
        }

        protected void PrepareColumns(ref StringBuilder sql)
        {
            columns = new Dictionary<string, string>();
            parameters = new List<NpgsqlParameter>();

            AddParameter(sql, "created", NpgsqlTypes.NpgsqlDbType.Timestamp, DateTime.Now.ToUniversalTime(), 0);
            AddParameter(sql, "first_name", NpgsqlTypes.NpgsqlDbType.Varchar, CapitalizeFirstLetter(FirstName), 64);
            AddParameter(sql, "last_name", NpgsqlTypes.NpgsqlDbType.Varchar, CapitalizeFirstLetter(LastName), 64);

            if (InitialMeeting.HasValue)
            {
                AddParameter(sql, "initial_meeting", NpgsqlTypes.NpgsqlDbType.Date, InitialMeeting.Value.ToUniversalTime(), 0);
            }

            if (!String.IsNullOrWhiteSpace(MiddleName))
            {
                AddParameter(sql, "middle_name", NpgsqlTypes.NpgsqlDbType.Varchar, CapitalizeFirstLetter(MiddleName), 64);
            }

            if (LivingOnCampus.HasValue)
            {
                AddParameter(sql, "living_on_campus", NpgsqlTypes.NpgsqlDbType.Boolean, LivingOnCampus, 0);
            }

            if (!String.IsNullOrWhiteSpace(PhoneNumber))
            {
                AddParameter(sql, "phone_number", NpgsqlTypes.NpgsqlDbType.Varchar, PhoneNumber.Trim(), 32);
            }

            if (Classification.HasValue)
            {
                AddParameter(sql, "classification", NpgsqlTypes.NpgsqlDbType.Integer, Classification.Value, 0);
            }

            if (GraduatingYear.HasValue)
            {
                AddParameter(sql, "graduating_year", NpgsqlTypes.NpgsqlDbType.Integer, GraduatingYear.Value, 0);
            }

            if (!String.IsNullOrWhiteSpace(StudentId))
            {
                AddParameter(sql, "student_id", NpgsqlTypes.NpgsqlDbType.Varchar, StudentId.Trim(), 32);
            }

            if (DateOfBirth.HasValue)
            {
                AddParameter(sql, "dob", NpgsqlTypes.NpgsqlDbType.Date, DateOfBirth.Value.ToUniversalTime(), 0);
            }

            if (Citizenship.HasValue)
            {
                AddParameter(sql, "citizenship", NpgsqlTypes.NpgsqlDbType.Integer, Citizenship.Value, 0);
            }

            if (EnrolledFullTime.HasValue)
            {
                AddParameter(sql, "enrolled_full_time", NpgsqlTypes.NpgsqlDbType.Boolean, EnrolledFullTime.Value, 0);
            }

            if (PellGrantRecipient.HasValue)
            {
                AddParameter(sql, "pell_grant_recipient", NpgsqlTypes.NpgsqlDbType.Boolean, PellGrantRecipient.Value, 0);
            }

            if (HasPassport.HasValue)
            {
                AddParameter(sql, "passport_holder", NpgsqlTypes.NpgsqlDbType.Boolean, HasPassport.Value, 0);
            }

            if (Gpa.HasValue)
            {
                string parameterName = String.Format("@{0}", "gpa");
                columns.Add("gpa", parameterName);
                NpgsqlParameter parameter = new NpgsqlParameter(parameterName, NpgsqlTypes.NpgsqlDbType.Double)
                {
                    Scale = 3,
                    Precision = 2,
                    Value = Gpa.Value
                };

                parameters.Add(parameter);
            }

            if (!String.IsNullOrWhiteSpace(CampusEmail))
            {
                AddParameter(sql, "campus_email", NpgsqlTypes.NpgsqlDbType.Varchar, CampusEmail.Trim(), 128);
            }

            if (!String.IsNullOrWhiteSpace(AlternateEmail))
            {
                AddParameter(sql, "alternate_email", NpgsqlTypes.NpgsqlDbType.Varchar, AlternateEmail.Trim(), 128);
            }
        }

        /// <summary>
        /// Create a new student.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userId">
        /// Optional user ID if it turns out that the person using the form
        /// happens to be a registered user who's logged in recently.
        /// </param>
        protected void Save(NpgsqlConnection connection, int? userId)
        {
            int studentId;

            using (NpgsqlCommand command = connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder(@"INSERT INTO students (");

                PrepareColumns(ref sql);

                sql.Append(String.Join(", ", columns.Select(x => x.Key)));
                sql.Append(") VALUES (");
                sql.Append(String.Join(", ", columns.Select(x => x.Value)));
                sql.Append(") ");
                sql.Append("RETURNING id");

                command.CommandText = sql.ToString();
                command.Parameters.AddRange(parameters.ToArray());
                studentId = (int)command.ExecuteScalar();
            }

            if (SelectedMajors != null)
            {
                SaveStudentMajors(connection, studentId, SelectedMajors, true);
            }

            if (SelectedMinors != null)
            {
                SaveStudentMajors(connection, studentId, SelectedMinors, false);
            }

            if (SelectedLanguages != null)
            {
                SaveStudentLanguages(connection, studentId, "student_fluent_languages", SelectedLanguages);
            }

            if (SelectedDesiredLanguages != null)
            {
                SaveStudentLanguages(connection, studentId, "student_desired_languages", SelectedDesiredLanguages);
            }

            if (StudiedLanguages != null)
            {
                SaveStudentLanguages(connection, studentId, "student_studied_languages", StudiedLanguages);
            }

            // Student promo won't use this.
            if (StudyAbroadCountry != null)
            {
                int countriesCount = StudyAbroadCountry.Cast<int>().Count();

                if (countriesCount > 0 &&
                    countriesCount == StudyAbroadYear.Cast<int>().Count() &&
                    countriesCount == StudyAbroadPeriod.Cast<int>().Count())
                {
                    // The default if the user doesn't selecting anything
                    // at all is that all three enumerables will have a
                    // single element of value zero.
                    if (StudyAbroadCountry.ElementAt(0) > 0 && StudyAbroadYear.ElementAt(0) > 0)
                    {
                        SaveStudyAbroadDestinations(connection, studentId, StudyAbroadCountry, StudyAbroadYear,
                            StudyAbroadPeriod);
                    }
                }
            }

            ApplicationCache cacheProvider = new ApplicationCache();
            Dictionary<int, StudentBaseModel> students = cacheProvider.Get(CacheId, () => new Dictionary<int, StudentBaseModel>());
            Id = studentId;
            students.Add(studentId, this);
            cacheProvider.Set(CacheId, students);
        }

        protected void SaveStudyAbroadDestinations(NpgsqlConnection connection, int studentId,
            IEnumerable<int> countries, IEnumerable<int> years, IEnumerable<int> periods)
        {
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    DELETE FROM student_study_abroad_wishlist
                    WHERE student_id = @StudentId";

                command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;
                command.ExecuteNonQuery();
            }

            if (countries != null)
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO student_study_abroad_wishlist
                        (
                            student_id, country_id, year, period
                        )
                        VALUES
                        (
                            @StudentId, @CountryId, @Year, @Period
                        )";

                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;
                    command.Parameters.Add("@CountryId", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Parameters.Add("@Year", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Parameters.Add("@Period", NpgsqlTypes.NpgsqlDbType.Integer);

                    command.Prepare();

                    int countriesCount = countries.Cast<int>().Count();

                    for (int i = 0; i < countriesCount; i++)
                    {
                        command.Parameters[1].Value = countries.ElementAt(i);
                        command.Parameters[2].Value = years.ElementAt(i);
                        command.Parameters[3].Value = periods.ElementAt(i);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        protected void SaveStudentLanguages(NpgsqlConnection connection, int studentId, string tableName,
            IEnumerable<int> languages)
        {
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = String.Format(@"
                    DELETE FROM {0}
                    WHERE student_id = @StudentId",
                    tableName);

                command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;
                command.ExecuteNonQuery();
            }

            if (languages != null && languages.Cast<int>().Count() > 0)
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    IList<string> values = new List<string>();

                    foreach (int languageId in languages)
                    {
                        values.Add(String.Format("({0}, {1})", studentId, languageId));
                    }

                    StringBuilder sql = new StringBuilder();
                    sql.Append("INSERT INTO ").Append(tableName).Append(" (student_id, language_id) VALUES ");
                    sql.Append(String.Join(",", values));

                    command.CommandText = sql.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        protected void SaveStudentMajors(NpgsqlConnection connection, int studentId, IEnumerable<int> majors, bool isMajor)
        {
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    DELETE FROM matriculation
                    WHERE   student_id = @StudentId AND
                            is_major = @IsMajor";

                command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;
                command.Parameters.Add("@IsMajor", NpgsqlTypes.NpgsqlDbType.Boolean).Value = isMajor;
                command.ExecuteNonQuery();
            }

            if (majors != null && majors.Cast<int>().Count() > 0)
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("INSERT INTO matriculation (student_id, major_id, is_major) VALUES ");
                    IList<string> values = new List<string>();

                    foreach (int majorId in majors)
                    {
                        values.Add(String.Format("({0}, {1}, '{2}')", studentId, majorId, isMajor ? 1 : 0));
                    }

                    sql.Append(String.Join(",", values));

                    command.CommandText = sql.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
