using Belletrix.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Belletrix.Models
{
    public class MatriculationModel
    {
        public int StudentId { get; set; }
        public int MajorId { get; set; }
        public bool IsMajor { get; set; }

        public static IEnumerable<MatriculationModel> GetDeclarations(int studentId)
        {
            const string sql = @"
                SELECT  [MajorId], [IsMajor]
                FROM    [Matriculation]
                WHERE   [StudentId] = @StudentId";

            ICollection<MatriculationModel> declarations = new List<MatriculationModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                declarations.Add(new MatriculationModel()
                                {
                                    StudentId = studentId,
                                    MajorId = reader.GetInt32(reader.GetOrdinal("major_id")),
                                    IsMajor = reader.GetBoolean(reader.GetOrdinal("is_major"))
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

            return declarations;
        }
    }
}
