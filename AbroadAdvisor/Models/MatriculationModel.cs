using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;
using System.Collections.Generic;

namespace Bennett.AbroadAdvisor.Models
{
    public class MatriculationModel
    {
        public int StudentId { get; set; }
        public int MajorId { get; set; }
        public bool IsMajor { get; set; }

        public static IEnumerable<MatriculationModel> GetDeclarations(int studentId)
        {
            const string sql = @"
                SELECT  major_id, is_major
                FROM    matriculation
                WHERE   student_id = @StudentId";
            ICollection<MatriculationModel> declarations = new List<MatriculationModel>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;
                        connection.Open();

                        using (NpgsqlDataReader reader = command.ExecuteReader())
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
