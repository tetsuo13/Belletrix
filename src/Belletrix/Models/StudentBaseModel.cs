using Belletrix.Core;
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

        [StringLength(128)]
        [Display(Name = "Local Address")]
        public string StreetAddress { get; set; }

        [StringLength(128)]
        public string StreetAddress2 { get; set; }

        [StringLength(128)]
        public string City { get; set; }

        [StringLength(32)]
        public string State { get; set; }

        [StringLength(16)]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [StringLength(32)]
        [Display(Name = "Telephone #")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Range(1900, 3000)]
        [Display(Name = "Entering Year")]
        public int? EnteringYear { get; set; }

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
        protected ICollection<SqlParameter> parameters;

        protected static string StringOrDefault(SqlDataReader reader, string column)
        {
            int ord = reader.GetOrdinal(column);

            if (reader.IsDBNull(ord))
            {
                return null;
            }

            return reader.GetString(ord);
        }

        protected static bool? BoolOrDefault(SqlDataReader reader, string column)
        {
            int ord = reader.GetOrdinal(column);

            if (reader.IsDBNull(ord))
            {
                return null;
            }

            return reader.GetBoolean(ord);
        }

        protected static int? IntOrDefault(SqlDataReader reader, string column)
        {
            int ord = reader.GetOrdinal(column);

            if (reader.IsDBNull(ord))
            {
                return null;
            }

            return reader.GetInt32(ord);
        }

        protected void AddParameter(StringBuilder sql, string columnName, SqlDbType columnType,
            object columnValue, int columnLength)
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
            parameters = new List<SqlParameter>();

            FirstName = CapitalizeFirstLetter(FirstName);
            LastName = CapitalizeFirstLetter(LastName);

            AddParameter(sql, "created", SqlDbType.DateTime, DateTime.Now.ToUniversalTime(), 0);
            AddParameter(sql, "first_name", SqlDbType.NVarChar, FirstName, 64);
            AddParameter(sql, "last_name", SqlDbType.NVarChar, LastName, 64);

            if (InitialMeeting.HasValue)
            {
                AddParameter(sql, "initial_meeting", SqlDbType.Date, InitialMeeting.Value.ToUniversalTime(), 0);
            }

            if (!String.IsNullOrWhiteSpace(MiddleName))
            {
                MiddleName = CapitalizeFirstLetter(MiddleName);
                AddParameter(sql, "middle_name", SqlDbType.NVarChar, MiddleName, 64);
            }

            if (LivingOnCampus.HasValue)
            {
                AddParameter(sql, "living_on_campus", SqlDbType.Bit, LivingOnCampus, 0);
            }

            if (!String.IsNullOrEmpty(StreetAddress))
            {
                AddParameter(sql, "street_address", SqlDbType.NVarChar, StreetAddress, 128);
            }

            if (!String.IsNullOrEmpty(StreetAddress2))
            {
                AddParameter(sql, "street_address2", SqlDbType.NVarChar, StreetAddress2, 128);
            }

            if (!String.IsNullOrEmpty(City))
            {
                AddParameter(sql, "city", SqlDbType.NVarChar, City, 128);
            }

            if (!String.IsNullOrEmpty(State))
            {
                AddParameter(sql, "state", SqlDbType.NVarChar, State, 32);
            }

            if (!String.IsNullOrEmpty(PostalCode))
            {
                AddParameter(sql, "postal_code", SqlDbType.NVarChar, PostalCode, 16);
            }

            if (!String.IsNullOrWhiteSpace(PhoneNumber))
            {
                AddParameter(sql, "phone_number", SqlDbType.NVarChar, PhoneNumber.Trim(), 32);
            }

            if (Classification.HasValue)
            {
                AddParameter(sql, "classification", SqlDbType.Int, Classification.Value, 0);
            }

            if (EnteringYear.HasValue)
            {
                AddParameter(sql, "entering_year", SqlDbType.Int, EnteringYear.Value, 0);
            }

            if (GraduatingYear.HasValue)
            {
                AddParameter(sql, "graduating_year", SqlDbType.Int, GraduatingYear.Value, 0);
            }

            if (!String.IsNullOrWhiteSpace(StudentId))
            {
                AddParameter(sql, "student_id", SqlDbType.VarChar, StudentId.Trim(), 32);
            }

            if (DateOfBirth.HasValue)
            {
                AddParameter(sql, "dob", SqlDbType.Date, DateOfBirth.Value.ToUniversalTime(), 0);
            }

            if (Citizenship.HasValue)
            {
                AddParameter(sql, "citizenship", SqlDbType.Int, Citizenship.Value, 0);
            }

            if (EnrolledFullTime.HasValue)
            {
                AddParameter(sql, "enrolled_full_time", SqlDbType.Bit, EnrolledFullTime.Value, 0);
            }

            if (PellGrantRecipient.HasValue)
            {
                AddParameter(sql, "pell_grant_recipient", SqlDbType.Bit, PellGrantRecipient.Value, 0);
            }

            if (HasPassport.HasValue)
            {
                AddParameter(sql, "passport_holder", SqlDbType.Bit, HasPassport.Value, 0);
            }

            if (Gpa.HasValue)
            {
                string parameterName = String.Format("@{0}", "gpa");
                columns.Add("gpa", parameterName);
                SqlParameter parameter = new SqlParameter(parameterName, SqlDbType.Decimal)
                {
                    Scale = 3,
                    Precision = 2,
                    Value = Gpa.Value
                };

                parameters.Add(parameter);
            }

            if (!String.IsNullOrWhiteSpace(CampusEmail))
            {
                AddParameter(sql, "campus_email", SqlDbType.VarChar, CampusEmail.Trim(), 128);
            }

            if (!String.IsNullOrWhiteSpace(AlternateEmail))
            {
                AddParameter(sql, "alternate_email", SqlDbType.VarChar, AlternateEmail.Trim(), 128);
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
        protected void Save(SqlConnection connection, int? userId)
        {
            int studentId;

            StringBuilder sql = new StringBuilder("INSERT INTO [dbo].[Students] (");

            try
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    PrepareColumns(ref sql);

                    sql.Append(String.Join(", ", columns.Select(x => x.Key)));
                    sql.Append(") OUTPUT INSERTED.Id VALUES (");
                    sql.Append(String.Join(", ", columns.Select(x => x.Value)));
                    sql.Append(")");

                    command.CommandText = sql.ToString();
                    command.Parameters.AddRange(parameters.ToArray());
                    studentId = (int)command.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql.ToString();
                throw e;
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

            SaveStudyAbroadDestinations(connection, studentId, StudyAbroadCountry, StudyAbroadYear,
                StudyAbroadPeriod);

            ApplicationCache cacheProvider = new ApplicationCache();
            Dictionary<int, StudentBaseModel> students = cacheProvider.Get(CacheId, () => new Dictionary<int, StudentBaseModel>());
            Id = studentId;
            students.Add(studentId, this);
            cacheProvider.Set(CacheId, students);
        }

        protected void SaveStudyAbroadDestinations(SqlConnection connection, int studentId,
            IEnumerable<int> countries, IEnumerable<int> years, IEnumerable<int> periods)
        {
            const string sql = @"
                DELETE FROM [dbo].[StudentStudyAbroadWishlist]
                WHERE       [StudentId] = @StudentId";

            try
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
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
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertSql;

                    command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                    command.Parameters.Add("@CountryId", SqlDbType.Int);
                    command.Parameters.Add("@Year", SqlDbType.Int);
                    command.Parameters.Add("@Period", SqlDbType.Int);

                    command.Prepare();

                    for (int i = 0; i < countriesCount; i++)
                    {
                        command.Parameters[1].Value = countries.ElementAt(i);
                        command.Parameters[2].Value = years.ElementAt(i);
                        command.Parameters[3].Value = periods.ElementAt(i);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = insertSql;
                throw e;
            }
        }

        protected void SaveStudentLanguages(SqlConnection connection, int studentId, string tableName,
            IEnumerable<int> languages)
        {
            string sql = String.Format(@"
                DELETE FROM [dbo].[{0}]
                WHERE       [StudentId] = @StudentId",
                tableName);

            try
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }

            if (languages != null && languages.Cast<int>().Count() > 0)
            {
                ICollection<string> values = new List<string>();

                foreach (int languageId in languages)
                {
                    values.Add(String.Format("({0}, {1})", studentId, languageId));
                }

                StringBuilder insertSql = new StringBuilder();
                insertSql.Append("INSERT INTO [dbo].[").Append(tableName).Append("] ([StudentId], [LanguageId]) VALUES ");
                insertSql.Append(String.Join(",", values));

                try
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = insertSql.ToString();
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    e.Data["SQL"] = insertSql.ToString();
                    throw e;
                }
            }
        }

        protected void SaveStudentMajors(SqlConnection connection, int studentId, IEnumerable<int> majors, bool isMajor)
        {
            const string sql = @"
                DELETE FROM [dbo].[Matriculation]
                WHERE       [StudentId] = @StudentId AND
                            [IsMajor] = @IsMajor";

            try
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                    command.Parameters.Add("@IsMajor", SqlDbType.Bit).Value = isMajor;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }

            if (majors != null && majors.Cast<int>().Count() > 0)
            {
                StringBuilder insertSql = new StringBuilder("INSERT INTO [dbo].[Matriculation] ([StudentId], [MajorId], [IsMajor]) VALUES ");
                ICollection<string> values = new List<string>();

                foreach (int majorId in majors)
                {
                    values.Add(String.Format("({0}, {1}, '{2}')", studentId, majorId, isMajor ? 1 : 0));
                }

                insertSql.Append(String.Join(",", values));

                try
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = insertSql.ToString();
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    e.Data["SQL"] = insertSql.ToString();
                    throw e;
                }
            }
        }
    }
}
