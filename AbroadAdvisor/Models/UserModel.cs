using System;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Npgsql;
using System.Configuration;

namespace Bennett.AbroadAdvisor.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastLogin { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
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

        public static UserModel GetUser(string username)
        {
            UserModel user = new UserModel();
            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                const string sql = @"
                    SELECT  id, first_name, last_name,
                            created, last_login, email,
                            admin, active
                    FROM    users
                    WHERE   login = @Username";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Username", NpgsqlTypes.NpgsqlDbType.Varchar, 24).Value = username;
                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.Id = reader.GetInt32(reader.GetOrdinal("id"));
                            user.FirstName = reader.GetString(reader.GetOrdinal("first_name"));
                            user.LastName = reader.GetString(reader.GetOrdinal("last_name"));
                            user.Created = reader.GetDateTime(reader.GetOrdinal("created"));
                            user.Email = reader.GetString(reader.GetOrdinal("email"));
                            user.IsAdmin = reader.GetBoolean(reader.GetOrdinal("admin"));
                            user.IsAdmin = reader.GetBoolean(reader.GetOrdinal("active"));

                            int ord = reader.GetOrdinal("last_login");

                            if (!reader.IsDBNull(ord))
                            {
                                user.LastLogin = reader.GetDateTime(ord);
                            }
                        }
                    }
                }
            }

            return user;
        }
    }
}
