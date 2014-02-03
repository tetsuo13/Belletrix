﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Security.Cryptography;
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
        public bool IsAdmin { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public static string CalculatePasswordHash(string password)
        {
            StringBuilder result = new StringBuilder();

            using (SHA256 sha = new SHA256Managed())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < hash.Length; i++)
                {
                    result.Append(hash[i].ToString("x2"));
                }
            }

            return result.ToString();
        }

        public static void UpdateLastLogin(string username)
        {
            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                const string sql = @"
                    UPDATE  users
                    SET     last_login = @LastLogin
                    WHERE   login = @Username";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.Add("@LastLogin", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();
                    command.Parameters.Add("@Username", NpgsqlTypes.NpgsqlDbType.Varchar, 24).Value = username;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<UserModel> GetUsers(string username = null)
        {
            List<UserModel> users = new List<UserModel>();
            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                string sql = @"
                    SELECT  id, first_name, last_name,
                            created, last_login, email,
                            admin, active, login
                    FROM    users ";

                if (username != null)
                {
                    sql += "WHERE login = @Username";
                }

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
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
                                Created = reader.GetDateTime(reader.GetOrdinal("created")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                IsAdmin = reader.GetBoolean(reader.GetOrdinal("admin")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("active"))
                            };

                            int ord = reader.GetOrdinal("last_login");

                            if (!reader.IsDBNull(ord))
                            {
                                user.LastLogin = reader.GetDateTime(ord);
                            }

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public static UserModel GetUser(string username)
        {
            List<UserModel> users = UserModel.GetUsers(username);
            return users[0];
        }

        public static void Update(UserModel model, bool isAdmin)
        {
            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                bool updatePassword = !String.IsNullOrEmpty(model.Password);

                StringBuilder sql = new StringBuilder(@"
                    UPDATE  users
                    SET     first_name = @FirstName,
                            last_name = @LastName,
                            email = @Email ");

                if (updatePassword)
                {
                    sql.Append(", password = @Password ");
                }

                if (isAdmin)
                {
                    sql.Append(", admin = @Admin, active = @Active ");
                }

                sql.Append("WHERE id = @Id");

                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql.ToString();
                    command.Parameters.Add("@FirstName", NpgsqlTypes.NpgsqlDbType.Varchar, 64).Value = model.FirstName;
                    command.Parameters.Add("@LastName", NpgsqlTypes.NpgsqlDbType.Varchar, 64).Value = model.LastName;
                    command.Parameters.Add("@Email", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = model.Email;
                    command.Parameters.Add("@Id", NpgsqlTypes.NpgsqlDbType.Integer).Value = model.Id;

                    if (updatePassword)
                    {
                        command.Parameters.Add("@Password", NpgsqlTypes.NpgsqlDbType.Char, 256).Value =
                            UserModel.CalculatePasswordHash(model.Password);
                    }

                    if (isAdmin)
                    {
                        command.Parameters.Add("@Admin", NpgsqlTypes.NpgsqlDbType.Boolean).Value = model.IsAdmin;
                        command.Parameters.Add("@Active", NpgsqlTypes.NpgsqlDbType.Boolean).Value = model.IsActive;
                    }

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
