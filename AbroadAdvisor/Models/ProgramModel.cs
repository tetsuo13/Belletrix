using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System.Collections.Generic;

namespace Bennett.AbroadAdvisor.Models
{
    public class ProgramModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }

        public static IEnumerable<ProgramModel> GetPrograms()
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            string cacheId = "Programs";
            List<ProgramModel> programs = cacheProvider.Get(cacheId, () => new List<ProgramModel>());

            if (programs.Count == 0)
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            SELECT      id, name, abbreviation
                            FROM        programs
                            ORDER BY    name";

                        connection.Open();

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProgramModel program = new ProgramModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Name = reader.GetString(reader.GetOrdinal("name"))
                                };

                                int ord = reader.GetOrdinal("abbreviation");

                                if (!reader.IsDBNull(ord))
                                {
                                    program.Abbreviation = reader.GetString(ord);
                                }

                                programs.Add(program);
                            }

                            cacheProvider.Set(cacheId, programs);
                        }
                    }
                }
            }

            return programs;
        }
    }
}
