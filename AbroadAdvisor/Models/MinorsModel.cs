using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Bennett.AbroadAdvisor.Models
{
    public class MinorsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public static IEnumerable<MinorsModel> GetMinors()
        {
            List<MinorsModel> minors = new List<MinorsModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT      id, name
                        FROM        minors
                        ORDER BY    name";

                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            minors.Add(new MinorsModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name"))
                            });
                        }
                    }
                }
            }

            return minors.AsEnumerable();
        }
    }
}
