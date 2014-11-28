using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;
using System.Collections.Generic;

namespace Bennett.AbroadAdvisor.Models
{
    public class StudentPromoLog
    {
        private const string CacheId = "StudentPromoLog";

        public int PromoId { get; set; }
        public int StudentId { get; set; }
        public DateTime Created { get; set; }

        public static void Save(NpgsqlConnection connection, int studentId, string promoCode)
        {
            const string sql = @"
                INSERT INTO student_promo_log
                (
                    promo_id, student_id, created
                )
                VALUES
                (
                    (SELECT id FROM user_promo WHERE code = @PromoCode), @StudentId, @Created
                )
                RETURNING promo_id";

            try
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.Add("@PromoCode", NpgsqlTypes.NpgsqlDbType.Varchar, 32).Value = promoCode.ToLower();
                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;
                    command.Parameters.Add("@Created", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();

                    int promoId = Convert.ToInt32(command.ExecuteScalar());
                    CacheOurselves(promoId, studentId);
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        public static void Save(NpgsqlConnection connection, int studentId, int promoId)
        {
            const string sql = @"
                INSERT INTO student_promo_log
                (
                    promo_id, student_id, created
                )
                VALUES
                (
                    @PromoId, @StudentId, @Created
                )";

            try
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@PromoId", NpgsqlTypes.NpgsqlDbType.Integer).Value = promoId;
                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;
                    command.Parameters.Add("@Created", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();
                    command.ExecuteNonQuery();

                    CacheOurselves(promoId, studentId);
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        private static void CacheOurselves(int promoId, int studentId)
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            List<StudentPromoLog> events = cacheProvider.Get(CacheId, () => new List<StudentPromoLog>());
            events.Add(new StudentPromoLog()
                {
                    PromoId = promoId,
                    StudentId = studentId
                });
            cacheProvider.Set(CacheId, events);
        }

        /// <summary>
        /// Change the associated promo ID for an existing student.
        /// </summary>
        /// <param name="studentId">Student ID.</param>
        /// <param name="promoId">Promo ID.</param>
        private static void UpdateCache(int studentId, int promoId)
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            List<StudentPromoLog> events = cacheProvider.Get(CacheId, () => new List<StudentPromoLog>());

            foreach (StudentPromoLog e in events)
            {
                if (e.StudentId == studentId)
                {
                    e.PromoId = promoId;
                    break;
                }
            }

            cacheProvider.Set(CacheId, events);
        }

        public static IEnumerable<StudentPromoLog> GetLogsForPromo(int id)
        {
            ICollection<StudentPromoLog> logs = new List<StudentPromoLog>();
            const string sql = @"
                SELECT  student_id, created
                FROM    student_promo_log
                WHERE   promo_id = @PromoId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.Add("@PromoId", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;
                        connection.Open();

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                StudentPromoLog log = new StudentPromoLog()
                                {
                                    PromoId = id,
                                    StudentId = reader.GetInt32(reader.GetOrdinal("student_id")),
                                    Created = reader.GetDateTime(reader.GetOrdinal("created"))
                                };

                                logs.Add(log);
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

        public static void Delete(NpgsqlConnection connection, int studentId)
        {
            const string sql = @"
                DELETE FROM student_promo_log
                WHERE   student_id = @StudentId";

            try
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Numeric).Value = studentId;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        /// <summary>
        /// Check to see if the student is already associated with a promo. If
        /// not then create a new association; otherwise update the promo ID
        /// that exists for the student.
        /// </summary>
        /// <param name="connection">Opened connection.</param>
        /// <param name="studentId">Student ID.</param>
        /// <param name="promoId">Promo ID.</param>
        public static void Upsert(NpgsqlConnection connection, int studentId, int promoId)
        {
            string sql = null;

            try
            {
                bool recordExists = false;

                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    sql = @"
                        SELECT  promo_id
                        FROM    student_promo_log
                        WHERE   student_id = @StudentId";

                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Numeric).Value = studentId;

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        recordExists = reader.HasRows;
                    }
                }

                if (recordExists)
                {
                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        sql = @"
                            UPDATE  student_promo_log
                            SET     promo_id = @PromoId,
                                    created = @Created
                            WHERE   student_id = @StudentId";

                        command.CommandText = sql;
                        command.Parameters.Add("@PromoId", NpgsqlTypes.NpgsqlDbType.Numeric).Value = promoId;
                        command.Parameters.Add("@Created", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();
                        command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Numeric).Value = studentId;
                        command.ExecuteNonQuery();

                        UpdateCache(studentId, promoId);
                    }
                }
                else
                {
                    Save(connection, studentId, promoId);
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
