using Belletrix.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Belletrix.Models
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

            const string sql = @"
                SELECT      [Id], [Name]
                FROM        [Minors]
                ORDER BY    [Name]";

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                minors.Add(new MinorsModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                });
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

            return minors;
        }
    }
}
