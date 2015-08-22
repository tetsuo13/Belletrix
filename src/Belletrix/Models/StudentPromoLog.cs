using Belletrix.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Belletrix.Models
{
    public class StudentPromoLog
    {
        public int PromoId { get; set; }
        public int StudentId { get; set; }
        public DateTime Created { get; set; }

        public static IEnumerable<StudentPromoLog> Get()
        {
            ICollection<StudentPromoLog> logs = new List<StudentPromoLog>();

            const string sql = @"
                SELECT  [PromoId], [StudentId], [Created]
                FROM    [StudentPromoLog]";

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
                                logs.Add(new StudentPromoLog()
                                    {
                                        PromoId = reader.GetInt32(reader.GetOrdinal("PromoId")),
                                        StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                        Created = reader.GetDateTime(reader.GetOrdinal("Created"))
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

            return logs;
        }

        public static void Save(SqlConnection connection, SqlTransaction transaction, int studentId,
            IEnumerable<int> promoIds)
        {
            Delete(connection, transaction, studentId);

            if (promoIds != null && promoIds.Any())
            {
                ICollection<StudentPromoLog> logs = new List<StudentPromoLog>();

                const string sql = @"
                    INSERT INTO [dbo].[StudentPromoLog]
                    ([PromoId], [StudentId], [Created])
                    VALUES
                    (@PromoId, @StudentId, @Created)";

                try
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        DateTime created = DateTime.Now.ToUniversalTime();

                        command.Transaction = transaction;
                        command.CommandText = sql;

                        command.Parameters.Add("@PromoId", SqlDbType.Int);
                        command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                        command.Parameters.Add("@Created", SqlDbType.DateTime).Value = created;

                        command.Prepare();

                        foreach (int promoId in promoIds)
                        {
                            StudentPromoLog log = new StudentPromoLog()
                            {
                                PromoId = promoId,
                                StudentId = studentId,
                                Created = created
                            };

                            command.Parameters[0].Value = promoId;
                            command.ExecuteNonQuery();

                            logs.Add(log);
                        }
                    }
                }
                catch (Exception e)
                {
                    e.Data["SQL"] = sql;
                    throw e;
                }
            }
        }

        public static IEnumerable<int> GetPromoIdsForStudent(int studentId)
        {
            IEnumerable<StudentPromoLog> logs = Get();
            return logs
                .Where(x => x.StudentId == studentId)
                .Select(x => x.PromoId);
        }

        /// <summary>
        /// Save a single student ID to promo code association.
        /// </summary>
        /// <param name="connection">Opened connection.</param>
        /// <param name="transaction"></param>
        /// <param name="studentId">Student ID.</param>
        /// <param name="promoCode">Promo code.</param>
        public static void Save(SqlConnection connection, SqlTransaction transaction, int studentId, string promoCode)
        {
            ICollection<StudentPromoLog> logs = new List<StudentPromoLog>();

            const string sql = @"
                INSERT INTO [dbo].[StudentPromoLog]
                ([PromoId], [StudentId], [Created])
                OUTPUT INSERTED.PromoId
                VALUES
                (
                    (SELECT [Id] FROM [dbo].[UserPromo] WHERE [Code] = @PromoCode), @StudentId, @Created
                )";

            try
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    DateTime created = DateTime.Now.ToUniversalTime();

                    command.Transaction = transaction;
                    command.CommandText = sql;

                    command.Parameters.Add("@PromoCode", SqlDbType.VarChar, 32).Value = promoCode.ToLower();
                    command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                    command.Parameters.Add("@Created", SqlDbType.DateTime).Value = created;

                    int promoId = Convert.ToInt32(command.ExecuteScalar());

                    logs.Add(new StudentPromoLog()
                        {
                            PromoId = promoId,
                            StudentId = studentId,
                            Created = created
                        });
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        public static IEnumerable<StudentPromoLog> GetLogsForPromo(int id)
        {
            IEnumerable<StudentPromoLog> logs = Get();
            return logs.Where(x => x.PromoId == id);
        }

        private static void Delete(SqlConnection connection, SqlTransaction transaction, int studentId)
        {
            const string sql = @"
                DELETE FROM [dbo].[StudentPromoLog]
                WHERE       [StudentId] = @StudentId";

            try
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }
    }
}
