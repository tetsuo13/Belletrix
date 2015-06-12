using Belletrix.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Belletrix.Models
{
    public class MajorsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public static IEnumerable<MajorsModel> GetMajors()
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            const string cacheId = "Majors";
            List<MajorsModel> majors = cacheProvider.Get(cacheId, () => new List<MajorsModel>());

            if (majors.Count == 0)
            {
                const string sql = @"
                    SELECT      id, name
                    FROM        majors
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
                                    majors.Add(new MajorsModel()
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

                cacheProvider.Set(cacheId, majors);
            }

            return majors;
        }
    }
}
