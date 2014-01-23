using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

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
                connection.ValidateRemoteCertificateCallback += connection_ValidateRemoteCertificateCallback;
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

        bool connection_ValidateRemoteCertificateCallback(X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
            throw new System.NotImplementedException();
        }
    }
}
