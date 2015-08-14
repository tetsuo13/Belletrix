using Belletrix.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Belletrix.Models
{
    public class ProgramTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool ShortTerm { get; set; }

        public static IEnumerable<ProgramTypeModel> GetProgramTypes()
        {
            ICollection<ProgramTypeModel> programTypes = new List<ProgramTypeModel>();
            const string sql = @"
                SELECT      [Id], [Name], [ShortTerm]
                FROM        [ProgramTypes]
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
                                programTypes.Add(new ProgramTypeModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    ShortTerm = reader.GetBoolean(reader.GetOrdinal("ShortTerm"))
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
