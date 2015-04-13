using Belletrix.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Belletrix.Models
{
    public class LanguageModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public static List<LanguageModel> GetLanguages()
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            const string cacheId = "Languages";
            List<LanguageModel> languages = cacheProvider.Get(cacheId, () => new List<LanguageModel>());

            if (languages.Count == 0)
            {
                const string sql = @"
                    SELECT      id, name
                    FROM        languages
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
                                    languages.Add(new LanguageModel()
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

                cacheProvider.Set(cacheId, languages);
            }

            return languages;
        }
    }
}
