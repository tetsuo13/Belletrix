using Belletrix.Core;
using Npgsql;
using System;
using System.Collections.Generic;

namespace Belletrix.Models
{
    public class ProgramTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool ShortTerm { get; set; }

        public static IEnumerable<ProgramTypeModel> GetProgramTypes()
        {
            const string sql = @"
                SELECT      id, name, short_term
                FROM        program_types
                ORDER BY    name";
            ICollection<ProgramTypeModel> programTypes = new List<ProgramTypeModel>();

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
                                programTypes.Add(new ProgramTypeModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Name = reader.GetString(reader.GetOrdinal("name")),
                                    ShortTerm = reader.GetBoolean(reader.GetOrdinal("short_term"))
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

            return programTypes;
        }
    }
}
