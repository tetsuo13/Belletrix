using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bennett.AbroadAdvisor.Models
{
    public class UserModel
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(64)]
        public string LastName { get; set; }

        [Required]
        [StringLength(24)]
        [Editable(false)]
        public string Login { get; set; }

        public int PasswordIterations { get; set; }
        public string PasswordSalt { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime LastLogin { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(128)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Administrator?")]
        public bool IsAdmin { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public static void UpdateLastLogin(string username)
        {
            const string sql = @"
                UPDATE  users
                SET     last_login = @LastLogin
                WHERE   login = @Username";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.Add("@LastLogin", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();
                        command.Parameters.Add("@Username", NpgsqlTypes.NpgsqlDbType.Varchar, 24).Value = username;

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        public static IEnumerable<UserModel> GetUsers(string username = null)
        {
            ICollection<UserModel> users = new List<UserModel>();

            string sql = @"
                    SELECT  id, first_name, last_name,
                            created, last_login, email,
                            admin, active, login,
                            password_iterations, password_salt, password_hash
                    FROM    users ";

            if (username != null)
            {
                sql += "WHERE login = @Username";
            }

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;

                        if (username != null)
                        {
                            command.Parameters.Add("@Username", NpgsqlTypes.NpgsqlDbType.Varchar, 24).Value = username;
                        }

                        connection.Open();

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserModel user = new UserModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                                    LastName = reader.GetString(reader.GetOrdinal("last_name")),
                                    Login = reader.GetString(reader.GetOrdinal("login")),
                                    Created = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("created"))),
                                    Email = reader.GetString(reader.GetOrdinal("email")),
                                    IsAdmin = reader.GetBoolean(reader.GetOrdinal("admin")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("active")),
                                    PasswordIterations = reader.GetInt32(reader.GetOrdinal("password_iterations")),
                                    PasswordSalt = reader.GetString(reader.GetOrdinal("password_salt")),
                                    Password = reader.GetString(reader.GetOrdinal("password_hash"))
                                };

                                int ord = reader.GetOrdinal("last_login");

                                if (!reader.IsDBNull(ord))
                                {
                                    user.LastLogin = DateTimeFilter.UtcToLocal(reader.GetDateTime(ord));
                                }

                                users.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }

            return users;
        }

        public static UserModel GetUser(string username)
        {
            IEnumerable<UserModel> users = UserModel.GetUsers(username);

            if (users.Any())
            {
                return users.First();
            }

            throw new Exception("User not found");
        }

        public static UserModel GetUser(int id)
        {
            UserModel user = UserModel.GetUsers().FirstOrDefault(x => x.Id == id);

            if (user != null)
            {
                return user;
            }

            throw new Exception("User not found");
        }

        public void SaveChanges(bool isAdmin)
        {
            bool updatePassword = !String.IsNullOrEmpty(Password);

            StringBuilder sql = new StringBuilder(@"
                UPDATE  users
                SET     first_name = @FirstName,
                        last_name = @LastName,
                        email = @Email ");

            if (updatePassword)
            {
                sql.Append(", password_iterations = @PasswordIterations ");
                sql.Append(", password_salt = @PasswordSalt ");
                sql.Append(", password_hash = @PasswordHash ");
            }

            if (isAdmin)
            {
                sql.Append(", admin = @Admin");
                sql.Append(", active = @Active ");
            }

            sql.Append("WHERE id = @Id");

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql.ToString();
                        command.Parameters.Add("@FirstName", NpgsqlTypes.NpgsqlDbType.Varchar, 64).Value = FirstName.Trim();
                        command.Parameters.Add("@LastName", NpgsqlTypes.NpgsqlDbType.Varchar, 64).Value = LastName.Trim();
                        command.Parameters.Add("@Email", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = Email.Trim();
                        command.Parameters.Add("@Id", NpgsqlTypes.NpgsqlDbType.Integer).Value = Id;

                        if (updatePassword)
                        {
                            string hash = PasswordHash.CreateHash(Password);
                            string[] split = hash.Split(':');

                            command.Parameters.Add("@PasswordIterations", NpgsqlTypes.NpgsqlDbType.Integer).Value =
                                split[PasswordHash.ITERATION_INDEX];
                            command.Parameters.Add("@PasswordSalt", NpgsqlTypes.NpgsqlDbType.Char, 32).Value =
                                split[PasswordHash.SALT_INDEX];
                            command.Parameters.Add("@PasswordHash", NpgsqlTypes.NpgsqlDbType.Char, 32).Value =
                                split[PasswordHash.PBKDF2_INDEX];
                        }

                        if (isAdmin)
                        {
                            command.Parameters.Add("@Admin", NpgsqlTypes.NpgsqlDbType.Boolean).Value = IsAdmin;
                            command.Parameters.Add("@Active", NpgsqlTypes.NpgsqlDbType.Boolean).Value = IsActive;
                        }

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql.ToString();
                throw e;
            }
        }

        public void Save()
        {
            const string sql = @"
                INSERT INTO users
                (
                    first_name, last_name, login,
                    created, email, admin, active,
                    password_iterations, password_salt, password_hash
                )
                VALUES
                (
                    @FirstName, @LastName, @Login,
                    @Created, @Email, @Admin, @Active,
                    @PasswordIterations, @PasswordSalt, @PasswordHash
                )";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;

                        command.Parameters.Add("@FirstName", NpgsqlTypes.NpgsqlDbType.Varchar, 64).Value = FirstName.Trim();
                        command.Parameters.Add("@LastName", NpgsqlTypes.NpgsqlDbType.Varchar, 64).Value = LastName.Trim();
                        command.Parameters.Add("@Login", NpgsqlTypes.NpgsqlDbType.Varchar, 24).Value = Login.Trim();
                        command.Parameters.Add("@Created", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();
                        command.Parameters.Add("@Email", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = Email.Trim();
                        command.Parameters.Add("@Admin", NpgsqlTypes.NpgsqlDbType.Boolean).Value = IsAdmin;
                        command.Parameters.Add("@Active", NpgsqlTypes.NpgsqlDbType.Boolean).Value = IsActive;

                        string hash = PasswordHash.CreateHash(Password);
                        string[] split = hash.Split(':');

                        command.Parameters.Add("@PasswordIterations", NpgsqlTypes.NpgsqlDbType.Integer).Value =
                            split[PasswordHash.ITERATION_INDEX];
                        command.Parameters.Add("@PasswordSalt", NpgsqlTypes.NpgsqlDbType.Char, 32).Value =
                            split[PasswordHash.SALT_INDEX];
                        command.Parameters.Add("@PasswordHash", NpgsqlTypes.NpgsqlDbType.Char, 32).Value =
                            split[PasswordHash.PBKDF2_INDEX];

                        connection.Open();
                        command.ExecuteNonQuery();
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
