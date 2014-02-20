using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Bennett.AbroadAdvisor.Models
{
    public class StudentModel
    {
        [Key]
        public int Id { get; set; }

        public DateTime Created { get; set; }

        [Display(Name = "Initial Meeting")]
        public DateTime? InitialMeeting { get; set; }

        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }

        [StringLength(64)]
        public string MiddleName { get; set; }

        [Required]
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

        [StringLength(32)]
        [Display(Name = "Cell Phone #")]
        [DataType(DataType.PhoneNumber)]
        public string CellPhoneNumber { get; set; }

        [Range(1000, 9999)]
        [Display(Name = "Entering Year")]
        public int? Classification { get; set; }

        [StringLength(32)]
        [Display(Name = "Student ID")]
        public string StudentId { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Dorm Hall")]
        public int? DormId { get; set; }

        [StringLength(8)]
        [Display(Name = "Room #")]
        public string RoomNumber { get; set; }

        [StringLength(16)]
        [Display(Name = "Campus PO Box")]
        public string CampusPoBox { get; set; }

        [Display(Name = "Enrolled as a")]
        public bool? EnrolledFullTime { get; set; }

        public int? Citizenship { get; set; }

        [Display(Name = "Are you a Pell Grant Recipient?")]
        public bool? PellGrantRecipient { get; set; }

        [Display(Name = "Do you have a passport?")]
        public bool? HasPassport { get; set; }

        [Range(0.00, 9.99)]
        [Display(Name = "Current GPA")]
        public double? Gpa { get; set; }

        [StringLength(128)]
        [EmailAddress]
        [Display(Name = "Bennett College Email")]
        public string CampusEmail { get; set; }

        [StringLength(128)]
        [EmailAddress]
        [Display(Name = "Alternative Email")]
        public string AlternateEmail { get; set; }

        [Display(Name = "Major")]
        public int? Major { get; set; }

        [Display(Name = "Minor")]
        public int? Minor { get; set; }

        private Dictionary<string, string> columns;
        private List<NpgsqlParameter> parameters;

        private static string StringOrDefault(NpgsqlDataReader reader, string column)
        {
            int ord = reader.GetOrdinal(column);

            if (reader.IsDBNull(ord))
            {
                return null;
            }

            return reader.GetString(ord);
        }

        private static bool? BoolOrDefault(NpgsqlDataReader reader, string column)
        {
            int ord = reader.GetOrdinal(column);

            if (reader.IsDBNull(ord))
            {
                return null;
            }

            return reader.GetBoolean(ord);
        }

        private static int? IntOrDefault(NpgsqlDataReader reader, string column)
        {
            int ord = reader.GetOrdinal(column);

            if (reader.IsDBNull(ord))
            {
                return null;
            }

            return reader.GetInt32(ord);
        }

        private void AddParameter(StringBuilder sql, string columnName, NpgsqlTypes.NpgsqlDbType columnType,
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

        public void SaveChanges(int userId)
        {
            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            StringBuilder sql = new StringBuilder(@"UPDATE students SET ");

            PrepareColumns(ref sql);

            foreach (KeyValuePair<string, string> pair in columns)
            {
                sql.Append(String.Format("{0} = {1}, ", pair.Key, pair.Value));
            }

            // Remove the trailing comma and space.
            sql.Length -= 2;

            sql.Append(" WHERE id = @Id");
            parameters.Add(new NpgsqlParameter("@Id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = Id });

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                connection.Open();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql.ToString();
                        command.Parameters.AddRange(parameters.ToArray());
                        command.ExecuteNonQuery();
                    }

                    EventLogModel.AddStudentEvent(connection, userId, Id, EventLogModel.EventType.EditStudent);

                    transaction.Commit();
                }
            }
        }

        public static List<StudentModel> GetStudents(int? id = null)
        {
            List<StudentModel> students = new List<StudentModel>();
            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"
                        SELECT  *
                        FROM    students ");

                    if (id != null)
                    {
                        sql.Append("WHERE id = @Id ");
                    }

                    sql.Append("ORDER BY last_name, first_name");

                    command.CommandText = sql.ToString();
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
                            student.CellPhoneNumber = StringOrDefault(reader, "cell_phone_number");
                            student.Classification = IntOrDefault(reader, "classification");
                            student.StudentId = StringOrDefault(reader, "student_id");
                            student.DormId = IntOrDefault(reader, "dorm_id");
                            student.RoomNumber = StringOrDefault(reader, "room_number");
                            student.CampusPoBox = StringOrDefault(reader, "campus_po_box");
                            student.EnrolledFullTime = BoolOrDefault(reader, "enrolled_full_time");
                            student.Citizenship = IntOrDefault(reader, "citizenship");
                            student.PellGrantRecipient = BoolOrDefault(reader, "pell_grant_recipient");
                            student.HasPassport = BoolOrDefault(reader, "passport_holder");
                            student.CampusEmail = StringOrDefault(reader, "campus_email");
                            student.AlternateEmail = StringOrDefault(reader, "alternate_email");
                            student.Created = reader.GetDateTime(reader.GetOrdinal("created"));
                            student.Major = IntOrDefault(reader, "major_id");
                            student.Minor = IntOrDefault(reader, "minor_id");

                            int ord = reader.GetOrdinal("gpa");
                            if (!reader.IsDBNull(ord))
                            {
                                student.Gpa = Convert.ToDouble(reader["gpa"]);
                            }

                            ord = reader.GetOrdinal("dob");
                            if (!reader.IsDBNull(ord))
                            {
                                student.DateOfBirth = reader.GetDateTime(ord).ToLocalTime();
                            }

                            ord = reader.GetOrdinal("initial_meeting");
                            if (!reader.IsDBNull(ord))
                            {
                                student.InitialMeeting = reader.GetDateTime(ord).ToLocalTime();
                            }

                            students.Add(student);
                        }
                    }
                }
            }

            return students;
        }

        public void Save(int userId)
        {
            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            StringBuilder sql = new StringBuilder(@"INSERT INTO students (");

            PrepareColumns(ref sql);

            sql.Append(String.Join(", ", columns.Select(x => x.Key)));
            sql.Append(") VALUES (");
            sql.Append(String.Join(", ", columns.Select(x => x.Value)));
            sql.Append(") ");
            sql.Append("RETURNING id");

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                connection.Open();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    int studentId;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql.ToString();
                        command.Parameters.AddRange(parameters.ToArray());
                        studentId = (int)command.ExecuteScalar();
                    }

                    EventLogModel.AddStudentEvent(connection, userId, studentId, EventLogModel.EventType.AddStudent);

                    transaction.Commit();
                }
            }
        }

        private string CapitalizeFirstLetter(string value)
        {
            value = value.Trim();

            if (value.Length == 1)
            {
                return value.ToUpper();
            }

            return value.Substring(0, 1).ToUpper() + value.Substring(1);
        }

        private void PrepareColumns(ref StringBuilder sql)
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

            if (!String.IsNullOrWhiteSpace(StreetAddress))
            {
                AddParameter(sql, "street_address", NpgsqlTypes.NpgsqlDbType.Varchar, CapitalizeFirstLetter(StreetAddress), 128);
            }

            if (!String.IsNullOrWhiteSpace(StreetAddress2))
            {
                AddParameter(sql, "street_address2", NpgsqlTypes.NpgsqlDbType.Varchar, CapitalizeFirstLetter(StreetAddress2), 128);
            }

            if (!String.IsNullOrWhiteSpace(City))
            {
                AddParameter(sql, "city", NpgsqlTypes.NpgsqlDbType.Varchar, CapitalizeFirstLetter(City), 128);
            }

            if (!String.IsNullOrWhiteSpace(State))
            {
                AddParameter(sql, "state", NpgsqlTypes.NpgsqlDbType.Varchar, CapitalizeFirstLetter(State), 32);
            }

            if (!String.IsNullOrWhiteSpace(PostalCode))
            {
                AddParameter(sql, "postal_code", NpgsqlTypes.NpgsqlDbType.Varchar, PostalCode.Trim(), 16);
            }

            if (!String.IsNullOrWhiteSpace(PhoneNumber))
            {
                AddParameter(sql, "phone_number", NpgsqlTypes.NpgsqlDbType.Varchar, PhoneNumber.Trim(), 32);
            }

            if (!String.IsNullOrWhiteSpace(CellPhoneNumber))
            {
                AddParameter(sql, "cell_phone_number", NpgsqlTypes.NpgsqlDbType.Varchar, CellPhoneNumber.Trim(), 32);
            }

            if (Classification.HasValue)
            {
                AddParameter(sql, "classification", NpgsqlTypes.NpgsqlDbType.Smallint, Classification.Value, 0);
            }

            if (!String.IsNullOrWhiteSpace(StudentId))
            {
                AddParameter(sql, "student_id", NpgsqlTypes.NpgsqlDbType.Varchar, StudentId.Trim(), 32);
            }

            if (DateOfBirth.HasValue)
            {
                AddParameter(sql, "dob", NpgsqlTypes.NpgsqlDbType.Date, DateOfBirth.Value.ToUniversalTime(), 0);
            }

            if (DormId.HasValue)
            {
                AddParameter(sql, "dorm_id", NpgsqlTypes.NpgsqlDbType.Integer, DormId.Value, 0);
            }

            if (!String.IsNullOrWhiteSpace(RoomNumber))
            {
                AddParameter(sql, "room_number", NpgsqlTypes.NpgsqlDbType.Varchar, RoomNumber.Trim(), 8);
            }

            if (!String.IsNullOrWhiteSpace(CampusPoBox))
            {
                AddParameter(sql, "campus_po_box", NpgsqlTypes.NpgsqlDbType.Varchar, CampusPoBox.Trim(), 16);
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

            if (Major.HasValue)
            {
                AddParameter(sql, "major_id", NpgsqlTypes.NpgsqlDbType.Integer, Major.Value, 0);
            }

            if (Minor.HasValue)
            {
                AddParameter(sql, "minor_id", NpgsqlTypes.NpgsqlDbType.Integer, Minor.Value, 0);
            }
        }
    }
}
