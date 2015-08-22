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
    public class UserModel
    {
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

        [Display(Name = "Last Login")]
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
                UPDATE  [dbo].[Users]
                SET     [LastLogin] = @LastLogin
                WHERE   [Login] = @Username";

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.Add("@LastLogin", SqlDbType.DateTime).Value = DateTime.Now.ToUniversalTime();
                        command.Parameters.Add("@Username", SqlDbType.VarChar, 24).Value = username;

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
                SELECT  [Id], [FirstName], [LastName],
                        [Created], [LastLogin], [Email],
                        [Admin], [Active], [Login],
                        [PasswordIterations], [PasswordSalt], [PasswordHash]
                FROM    [dbo].[Users] ";

            if (username != null)
            {
                sql += "WHERE [Login] = @Username";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;

                        if (username != null)
                        {
                            command.Parameters.Add("@Username", SqlDbType.VarChar, 24).Value = username;
                        }

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserModel user = new UserModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    Login = reader.GetString(reader.GetOrdinal("Login")),
                                    Created = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("Created"))),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    IsAdmin = reader.GetBoolean(reader.GetOrdinal("Admin")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("Active")),
                                    PasswordIterations = reader.GetInt32(reader.GetOrdinal("PasswordIterations")),
                                    PasswordSalt = reader.GetString(reader.GetOrdinal("PasswordSalt")),
                                    Password = reader.GetString(reader.GetOrdinal("PasswordHash"))
                                };

                                int ord = reader.GetOrdinal("LastLogin");

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
                UPDATE  [dbo].[Users]
                SET     [FirstName] = @FirstName,
                        [LastName] = @LastName,
                        [Email] = @Email ");

            if (updatePassword)
            {
                sql.Append(", [PasswordIterations] = @PasswordIterations ");
                sql.Append(", [PasswordSalt] = @PasswordSalt ");
                sql.Append(", [PasswordHash] = @PasswordHash ");
            }

            if (isAdmin)
            {
                sql.Append(", [Admin] = @Admin");
                sql.Append(", [Active] = @Active ");
            }

            sql.Append("WHERE [Id] = @Id");

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql.ToString();
                        command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 64).Value = FirstName.Trim();
                        command.Parameters.Add("@LastName", SqlDbType.NVarChar, 64).Value = LastName.Trim();
                        command.Parameters.Add("@Email", SqlDbType.VarChar, 128).Value = Email.Trim();
                        command.Parameters.Add("@Id", SqlDbType.Int).Value = Id;

                        if (updatePassword)
                        {
                            string hash = PasswordHash.CreateHash(Password);
                            string[] split = hash.Split(':');

                            command.Parameters.Add("@PasswordIterations", SqlDbType.Int).Value =
                                split[PasswordHash.ITERATION_INDEX];
                            command.Parameters.Add("@PasswordSalt", SqlDbType.Char, 32).Value =
                                split[PasswordHash.SALT_INDEX];
                            command.Parameters.Add("@PasswordHash", SqlDbType.Char, 32).Value =
                                split[PasswordHash.PBKDF2_INDEX];
                        }

                        if (isAdmin)
                        {
                            command.Parameters.Add("@Admin", SqlDbType.Bit).Value = IsAdmin;
                            command.Parameters.Add("@Active", SqlDbType.Bit).Value = IsActive;
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
                INSERT INTO [dbo].[Users]
                (
                    [FirstName], [LastName], [Login],
                    [Created], [Email], [Admin], [Active],
                    [PasswordIterations], [PasswordSalt], [PasswordHash]
                )
                VALUES
                (
                    @FirstName, @LastName, @Login,
                    @Created, @Email, @Admin, @Active,
                    @PasswordIterations, @PasswordSalt, @PasswordHash
                )";

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;

                        command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 64).Value = FirstName.Trim();
                        command.Parameters.Add("@LastName", SqlDbType.NVarChar, 64).Value = LastName.Trim();
                        command.Parameters.Add("@Login", SqlDbType.VarChar, 24).Value = Login.Trim();
                        command.Parameters.Add("@Created", SqlDbType.DateTime).Value = DateTime.Now.ToUniversalTime();
                        command.Parameters.Add("@Email", SqlDbType.VarChar, 128).Value = Email.Trim();
                        command.Parameters.Add("@Admin", SqlDbType.Bit).Value = IsAdmin;
                        command.Parameters.Add("@Active", SqlDbType.Bit).Value = IsActive;

                        string hash = PasswordHash.CreateHash(Password);
                        string[] split = hash.Split(':');

                        command.Parameters.Add("@PasswordIterations", SqlDbType.Int).Value =
                            split[PasswordHash.ITERATION_INDEX];
                        command.Parameters.Add("@PasswordSalt", SqlDbType.Char, 32).Value =
                            split[PasswordHash.SALT_INDEX];
                        command.Parameters.Add("@PasswordHash", SqlDbType.Char, 32).Value =
                            split[PasswordHash.PBKDF2_INDEX];

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw;
            }
        }
    }
}
