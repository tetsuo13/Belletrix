using Belletrix.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
            ApplicationCache cacheProvider = new ApplicationCache();
            const string cacheId = "Minors";
            List<MinorsModel> minors = cacheProvider.Get(cacheId, () => new List<MinorsModel>());

            if (minors.Count == 0)
            {
                const string sql = @"
                    SELECT      id, name
                    FROM        minors
                    ORDER BY    name";

                try
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                    {
                        connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                        using (NpgsqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = sql;
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
                }
                catch (Exception e)
                {
                    e.Data["SQL"] = sql;
                    throw e;
                }

                cacheProvider.Set(cacheId, minors);
            }

            return minors;
        }
    }
}
