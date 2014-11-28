using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public bool IsRegion { get; set; }

        public static List<CountryModel> GetCountries()
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            string cacheId = "Countries";
            List<CountryModel> countries = cacheProvider.Get(cacheId, () => new List<CountryModel>());

            if (countries.Count == 0)
            {
                const string sql = @"
                    SELECT      id, name, abbreviation
                    FROM        countries
                    WHERE       is_region = FALSE
                    ORDER BY    CASE abbreviation
                                    WHEN 'US' THEN 1
                                    WHEN '' THEN 2
                                    ELSE 3
                                END, name";

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
                }
                catch (Exception e)
                {
                    e.Data["SQL"] = sql;
                    throw e;
                }

                cacheProvider.Set(cacheId, countries);
            }

            return countries;
        }

        public static List<CountryModel> GetRegions()
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            string cacheId = "Regions";
            List<CountryModel> regions = cacheProvider.Get(cacheId, () => new List<CountryModel>());

            if (regions.Count == 0)
            {
                const string sql = @"
                    SELECT      id, name, abbreviation
                    FROM        countries
                    WHERE       is_region = TRUE
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
                                    regions.Add(new CountryModel()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                                        Name = reader.GetString(reader.GetOrdinal("name")),
                                        Abbreviation = reader.GetString(reader.GetOrdinal("abbreviation"))
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

                cacheProvider.Set(cacheId, regions);
            }

            return regions;
        }
    }
}
