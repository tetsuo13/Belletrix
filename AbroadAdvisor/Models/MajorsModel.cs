﻿using Npgsql;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Bennett.AbroadAdvisor.Models
{
    public class MajorsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public static List<MajorsModel> GetMajors()
        {
            List<MajorsModel> majors = new List<MajorsModel>();

            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT      id, name
                        FROM        majors
                        ORDER BY    name";

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

            return majors;
        }
    }
}
