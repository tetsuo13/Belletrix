using Belletrix.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Belletrix.Models
{
    public class ProgramModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }

        public static IEnumerable<ProgramModel> GetPrograms()
        {
            List<ProgramModel> programs = new List<ProgramModel>();

            const string sql = @"
                SELECT      [Id], [Name], [Abbreviation]
                FROM        [Programs]
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
                                ProgramModel program = new ProgramModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                };

                                int ord = reader.GetOrdinal("Abbreviation");

                                if (!reader.IsDBNull(ord))
                                {
                                    program.Abbreviation = reader.GetString(ord);
                                }

                                programs.Add(program);
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

            return programs;
        }
    }
}
