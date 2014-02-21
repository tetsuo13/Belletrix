﻿using Npgsql;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Bennett.AbroadAdvisor.Models
{
    public class DormModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string HallName { get; set; }

        public static List<DormModel> GetDorms()
        {
            List<DormModel> dorms = new List<DormModel>();

            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT      id, hall_name
                        FROM        dorms
                        ORDER BY    hall_name";

                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dorms.Add(new DormModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    HallName = reader.GetString(reader.GetOrdinal("hall_name"))
                                });
                        }
                    }
                }
            }

            return dorms;
        }
    }
}