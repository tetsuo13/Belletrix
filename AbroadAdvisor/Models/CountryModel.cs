using Npgsql;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Bennett.AbroadAdvisor.Models
{
    public class CountryModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Abbreviation { get; set; }

        public static List<CountryModel> GetCountries()
        {
            List<CountryModel> countries = new List<CountryModel>();

            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT      id, name, abbreviation
                        FROM        countries
                        ORDER BY    name";

                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            countries.Add(new CountryModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                Abbreviation = reader.GetString(reader.GetOrdinal("abbreviation"))
                            });
                        }
                    }
                }
            }

            return countries;
        }
    }
}
