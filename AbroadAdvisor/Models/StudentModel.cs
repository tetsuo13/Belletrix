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

        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }

        [StringLength(64)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(64)]
        public string LastName { get; set; }

        public bool LivingOnCampus { get; set; }

        [StringLength(128)]
        public string StreetAddress { get; set; }

        [StringLength(128)]
        public string StreetAddress2 { get; set; }

        [StringLength(128)]
        public string City { get; set; }

        [StringLength(32)]
        public string State { get; set; }

        [StringLength(16)]
        public string PostalCode { get; set; }

        [StringLength(32)]
        public string PhoneNumber { get; set; }

        [StringLength(32)]
        public string CellPhoneNumber { get; set; }

        [Range(1000, 9999)]
        public int Classification { get; set; }

        [StringLength(32)]
        public string StudentId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int DormId { get; set; }

        [StringLength(8)]
        public string RoomNumber { get; set; }

        [StringLength(16)]
        public string CampusPoBox { get; set; }

        public bool EnrolledFullTime { get; set; }

        public int Citizenship { get; set; }

        public bool PellGrantRecipient { get; set; }

        public bool HasPassport { get; set; }

        [Range(0.00, 9.99)]
        public decimal Gpa { get; set; }

        [StringLength(128)]
        [EmailAddress]
        public string CampusEmail { get; set; }

        [StringLength(128)]
        [EmailAddress]
        public string AlternateEmail { get; set; }

        private static string StringOrDefault(NpgsqlDataReader reader, string column)
        {
            int ord = reader.GetOrdinal(column);

            if (reader.IsDBNull(ord))
            {
                return default(string);
            }

            return reader.GetString(ord);
        }

        private static bool BoolOrDefault(NpgsqlDataReader reader, string column)
        {
            int ord = reader.GetOrdinal(column);

            if (reader.IsDBNull(ord))
            {
                return false;
            }

            return reader.GetBoolean(ord);
        }

        private static int IntOrDefault(NpgsqlDataReader reader, string column)
        {
            int ord = reader.GetOrdinal(column);

            if (reader.IsDBNull(ord))
            {
                return default(int);
            }

            return reader.GetInt32(ord);
        }

        private static void AddParameter(StringBuilder sql, List<NpgsqlParameter> parameters,
            Dictionary<string, string> columns, string columnName, NpgsqlTypes.NpgsqlDbType columnType,
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

        public static List<StudentModel> GetStudents()
        {
            List<StudentModel> students = new List<StudentModel>();
            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT      *
                        FROM        students
                        ORDER BY    last_name, first_name";
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
                            student.PhoneNumber = StringOrDefault(reader, "phone_number");
                            student.CellPhoneNumber = StringOrDefault(reader, "cell_phone_number");
                            student.Classification = IntOrDefault(reader, "classification");
                            student.StudentId = StringOrDefault(reader, "student_id");
                            student.DormId = IntOrDefault(reader, "dorm_id");
                            student.RoomNumber = StringOrDefault(reader, "room_number");
                            student.CampusPoBox = StringOrDefault(reader, "campus_po_box");
                            student.EnrolledFullTime = BoolOrDefault(reader, "enrolled_full_time");
                            student.Citizenship = IntOrDefault(reader, "citizenship");
                            student.PellGrantRecipient = BoolOrDefault(reader, "pell_grant_citizenship");
                            student.HasPassport = BoolOrDefault(reader, "passport_holder");
                            student.CampusEmail = StringOrDefault(reader, "campus_email");
                            student.AlternateEmail = StringOrDefault(reader, "alternate_email");
                            student.Created = reader.GetDateTime(reader.GetOrdinal("created"));

                            int ord = reader.GetOrdinal("gpa");

                            if (!reader.IsDBNull(ord))
                            {
                                student.Gpa = reader.GetDecimal(ord);
                            }

                            students.Add(student);
                        }
                    }
                }
            }

            return students;
        }

        public static void Create(StudentModel student, int userId)
        {
            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            StringBuilder sql = new StringBuilder(@"INSERT INTO students (");
            Dictionary<string, string> columns = new Dictionary<string, string>();
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

            AddParameter(sql, parameters, columns, "created", NpgsqlTypes.NpgsqlDbType.Timestamp, DateTime.Now.ToUniversalTime(), 0);
            AddParameter(sql, parameters, columns, "first_name", NpgsqlTypes.NpgsqlDbType.Varchar, student.FirstName, 64);
            AddParameter(sql, parameters, columns, "last_name", NpgsqlTypes.NpgsqlDbType.Varchar, student.LastName, 64);

            if (!String.IsNullOrEmpty(student.MiddleName))
            {
                AddParameter(sql, parameters, columns, "middle_name", NpgsqlTypes.NpgsqlDbType.Varchar, student.MiddleName, 64);
            }

            AddParameter(sql, parameters, columns, "living_on_campus", NpgsqlTypes.NpgsqlDbType.Boolean, student.LivingOnCampus, 0);

            if (!String.IsNullOrEmpty(student.StreetAddress))
            {
                AddParameter(sql, parameters, columns, "street_address", NpgsqlTypes.NpgsqlDbType.Varchar, student.StreetAddress, 128);
            }

            if (!String.IsNullOrEmpty(student.StreetAddress2))
            {
                AddParameter(sql, parameters, columns, "street_address2", NpgsqlTypes.NpgsqlDbType.Varchar, student.StreetAddress2, 128);
            }

            if (!String.IsNullOrEmpty(student.City))
            {
                AddParameter(sql, parameters, columns, "city", NpgsqlTypes.NpgsqlDbType.Varchar, student.City, 128);
            }

            if (!String.IsNullOrEmpty(student.State))
            {
                AddParameter(sql, parameters, columns, "state", NpgsqlTypes.NpgsqlDbType.Varchar, student.State, 32);
            }

            if (!String.IsNullOrEmpty(student.PostalCode))
            {
                AddParameter(sql, parameters, columns, "postal_code", NpgsqlTypes.NpgsqlDbType.Varchar, student.PostalCode, 16);
            }

            if (!String.IsNullOrEmpty(student.PhoneNumber))
            {
                AddParameter(sql, parameters, columns, "phone_number", NpgsqlTypes.NpgsqlDbType.Varchar, student.PhoneNumber, 32);
            }

            if (!String.IsNullOrEmpty(student.CellPhoneNumber))
            {
                AddParameter(sql, parameters, columns, "cell_phone_number", NpgsqlTypes.NpgsqlDbType.Varchar, student.CellPhoneNumber, 32);
            }

            if (student.Classification > 0)
            {
                AddParameter(sql, parameters, columns, "classification", NpgsqlTypes.NpgsqlDbType.Smallint, student.Classification, 0);
            }

            if (!String.IsNullOrEmpty(student.StudentId))
            {
                AddParameter(sql, parameters, columns, "student_id", NpgsqlTypes.NpgsqlDbType.Varchar, student.StudentId, 32);
            }

            if (student.DateOfBirth != default(DateTime))
            {
                AddParameter(sql, parameters, columns, "dob", NpgsqlTypes.NpgsqlDbType.Date, student.DateOfBirth, 0);
            }

            if (student.DormId > 0)
            {
                AddParameter(sql, parameters, columns, "dorm_id", NpgsqlTypes.NpgsqlDbType.Integer, student.DormId, 0);
            }

            if (!String.IsNullOrEmpty(student.RoomNumber))
            {
                AddParameter(sql, parameters, columns, "room_number", NpgsqlTypes.NpgsqlDbType.Varchar, student.RoomNumber, 8);
            }

            if (!String.IsNullOrEmpty(student.CampusPoBox))
            {
                AddParameter(sql, parameters, columns, "campus_po_box", NpgsqlTypes.NpgsqlDbType.Varchar, student.CampusPoBox, 16);
            }

            if (student.Citizenship > 0)
            {
                AddParameter(sql, parameters, columns, "citizenship", NpgsqlTypes.NpgsqlDbType.Integer, student.Citizenship, 0);
            }

            AddParameter(sql, parameters, columns, "enrolled_full_time", NpgsqlTypes.NpgsqlDbType.Boolean, student.EnrolledFullTime, 0);
            AddParameter(sql, parameters, columns, "pell_grant_recipient", NpgsqlTypes.NpgsqlDbType.Boolean, student.PellGrantRecipient, 0);
            AddParameter(sql, parameters, columns, "passport_holder", NpgsqlTypes.NpgsqlDbType.Boolean, student.HasPassport, 0);

            if (student.Gpa != default(decimal))
            {
                AddParameter(sql, parameters, columns, "gpa", NpgsqlTypes.NpgsqlDbType.Double, student.Gpa, 0);
            }

            if (!String.IsNullOrEmpty(student.CampusEmail))
            {
                AddParameter(sql, parameters, columns, "campus_email", NpgsqlTypes.NpgsqlDbType.Varchar, student.CampusEmail, 128);
            }

            if (!String.IsNullOrEmpty(student.AlternateEmail))
            {
                AddParameter(sql, parameters, columns, "alternate_email", NpgsqlTypes.NpgsqlDbType.Varchar, student.AlternateEmail, 128);
            }

            sql.Append(String.Join(", ", columns.Select(x => x.Key)));
            sql.Append(") VALUES (");
            sql.Append(String.Join(", ", columns.Select(x => x.Value)));
            sql.Append(")");

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

                    EventLogModel.Add(connection, userId, EventLogModel.EventType.AddStudent,
                            String.Format("Created {0} {1}", student.FirstName, student.LastName));

                    transaction.Commit();
                }
            }
        }
    }
}
