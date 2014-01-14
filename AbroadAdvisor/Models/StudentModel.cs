using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.ComponentModel;

namespace Bennett.AbroadAdvisor.Models
{
    public class StudentModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }

        [StringLength(64)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(64)]
        public string LastName { get; set; }
        
        [StringLength(128)]
        [EmailAddress]
        public string Email { get; set; }
        
        [Range(1000, 9999)]
        public short EnteringYear { get; set; }

        [Range(1000, 9999)]
        public short GraduatingYear { get; set; }
        
        [Range(0.00, 9.99)]
        public decimal Gpa { get; set; }

        [Required]
        public bool HasPassport { get; set; }

        [StringLength(10)]
        public string PhoneNumber { get; set; }

        public static List<StudentModel> GetStudents()
        {
            List<StudentModel> students = new List<StudentModel>();
            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                const string sql = @"
                    SELECT      id, first_name, middle_name,
                                last_name, email, entering_year,
                                graduating_year, gpa, passport_holder,
                                phone_number
                    FROM        students
                    ORDER BY    last_name, first_name";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudentModel student = new StudentModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                                HasPassport = reader.GetBoolean(reader.GetOrdinal("passport_holder"))
                            };

                            int ord = reader.GetOrdinal("middle_name");
                            if (!reader.IsDBNull(ord))
                            {
                                student.MiddleName = reader.GetString(ord);
                            }

                            ord = reader.GetOrdinal("email");
                            if (!reader.IsDBNull(ord))
                            {
                                student.Email = reader.GetString(ord);
                            }

                            ord = reader.GetOrdinal("entering_year");
                            if (!reader.IsDBNull(ord))
                            {
                                student.EnteringYear = reader.GetInt16(ord);
                            }

                            ord = reader.GetOrdinal("graduating_year");
                            if (!reader.IsDBNull(ord))
                            {
                                student.GraduatingYear = reader.GetInt16(ord);
                            }

                            ord = reader.GetOrdinal("gpa");
                            if (!reader.IsDBNull(ord))
                            {
                                student.Gpa = reader.GetDecimal(ord);
                            }

                            ord = reader.GetOrdinal("phone_number");
                            if (!reader.IsDBNull(ord))
                            {
                                student.PhoneNumber = reader.GetString(ord);
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

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                const string sql = @"
                    INSERT INTO students
                    (
                        first_name, middle_name, last_name,
                        email, entering_year, graduating_year,
                        gpa, passport_holder, phone_number
                    )
                    VALUES
                    (
                        @FirstName, @MiddleName, @LastName,
                        @Email, @EnteringYear, @GraduatingYear,
                        @Gpa, @PassportHolder, @PhoneNumber
                    )";

                connection.Open();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Transaction = transaction;
                        command.Parameters.Add("@FirstName", NpgsqlTypes.NpgsqlDbType.Varchar, 64).Value = student.FirstName;
                        command.Parameters.Add("@LastName", NpgsqlTypes.NpgsqlDbType.Varchar, 64).Value = student.LastName;
                        command.Parameters.Add("@PassportHolder", NpgsqlTypes.NpgsqlDbType.Boolean).Value = student.HasPassport;

                        if (!String.IsNullOrEmpty(student.MiddleName))
                        {
                            command.Parameters.Add("@MiddleName", NpgsqlTypes.NpgsqlDbType.Varchar, 64).Value = student.MiddleName;
                        }
                        else
                        {
                            command.Parameters.Add("@MiddleName", NpgsqlTypes.NpgsqlDbType.Varchar, 64).Value = DBNull.Value;
                        }

                        if (!String.IsNullOrEmpty(student.Email))
                        {
                            command.Parameters.Add("@Email", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = student.Email;
                        }
                        else
                        {
                            command.Parameters.Add("@Email", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = DBNull.Value;
                        }

                        if (student.EnteringYear > 0)
                        {
                            command.Parameters.Add("@EnteringYear", NpgsqlTypes.NpgsqlDbType.Smallint).Value = student.EnteringYear;
                        }
                        else
                        {
                            command.Parameters.Add("@EnteringYear", NpgsqlTypes.NpgsqlDbType.Smallint).Value = DBNull.Value;
                        }

                        if (student.GraduatingYear > 0)
                        {
                            command.Parameters.Add("@GraduatingYear", NpgsqlTypes.NpgsqlDbType.Smallint).Value = student.GraduatingYear;
                        }
                        else
                        {
                            command.Parameters.Add("@GraduatingYear", NpgsqlTypes.NpgsqlDbType.Smallint).Value = DBNull.Value;
                        }

                        if (student.Gpa > 0.0m)
                        {
                            command.Parameters.Add("@Gpa", NpgsqlTypes.NpgsqlDbType.Numeric).Value = student.Gpa;
                        }
                        else
                        {
                            command.Parameters.Add("@Gpa", NpgsqlTypes.NpgsqlDbType.Double).Value = DBNull.Value;
                        }

                        if (!String.IsNullOrEmpty(student.PhoneNumber))
                        {
                            command.Parameters.Add("@PhoneNumber", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Value = student.PhoneNumber;
                        }
                        else
                        {
                            command.Parameters.Add("@PhoneNumber", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Value = DBNull.Value;
                        }

                        command.ExecuteNonQuery();
                    }

                    EventLogModel.Add(connection, transaction, userId, EventLogModel.EventType.AddStudent,
                            String.Format("Created {0} {1}", student.FirstName, student.LastName));

                    transaction.Commit();
                }
            }
        }
    }
}
