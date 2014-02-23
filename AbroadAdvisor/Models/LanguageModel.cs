﻿using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bennett.AbroadAdvisor.Models
{
    public class LanguageModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public static List<LanguageModel> GetLanguages()
        {
            List<LanguageModel> languages = new List<LanguageModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT      id, name
                        FROM        languages
                        ORDER BY    name";

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

            return languages;
        }
    }
}