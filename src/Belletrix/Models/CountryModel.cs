using Belletrix.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Belletrix.Models
{
    public class CountryModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Abbreviation { get; set; }

        public bool IsRegion { get; set; }

        public static List<CountryModel> GetCountries()
        {
            List<CountryModel> countries = new List<CountryModel>();

            const string sql = @"
                SELECT      [Id], [Name], [Abbreviation]
                FROM        [dbo].[Countries]
                WHERE       [IsRegion] = 0
                ORDER BY    CASE [Abbreviation]
                                WHEN 'US' THEN 1
                                WHEN '' THEN 2
                                ELSE 3
                            END, [Name]";

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
                                countries.Add(new CountryModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Abbreviation = reader.GetString(reader.GetOrdinal("Abbreviation"))
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

            return countries;
        }

        public static List<CountryModel> GetRegions()
        {
            List<CountryModel> regions = new List<CountryModel>();

            const string sql = @"
                SELECT      [Id], [Name], [Abbreviation]
                FROM        [dbo].[Countries]
                WHERE       [IsRegion] = 1
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
                                regions.Add(new CountryModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Abbreviation = reader.GetString(reader.GetOrdinal("Abbreviation"))
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

            return regions;
        }
    }
}
