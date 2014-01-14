using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Bennett.AbroadAdvisor.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Username")]
        [StringLength(24)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(256)]
        public string Password { get; set; }

        public bool IsValid(string username, string password)
        {
            bool valid = false;
            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                const string sql = @"
                    SELECT  id
                    FROM    users
                    WHERE   login = @Username AND
                            password = @Password AND
                            active = TRUE";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Username", NpgsqlTypes.NpgsqlDbType.Varchar, 24).Value = username;

                    string hashedPassword = UserModel.CalculatePasswordHash(password);
                    command.Parameters.Add("@Password", NpgsqlTypes.NpgsqlDbType.Char, 256).Value = hashedPassword;

                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            valid = true;
                        }
                    }
                }
            }

            return valid;
        }
    }
}
