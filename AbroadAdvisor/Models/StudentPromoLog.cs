using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bennett.AbroadAdvisor.Models
{
    public class StudentPromoLog
    {
        private const string CacheId = "StudentPromoLog";

        public int PromoId { get; set; }
        public int StudentId { get; set; }
        public DateTime Created { get; set; }

        public static IEnumerable<StudentPromoLog> Get()
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            ICollection<StudentPromoLog> logs = cacheProvider.Get(CacheId, () => new List<StudentPromoLog>());

            if (logs.Count == 0)
            {
                const string sql = @"
                    SELECT  promo_id, student_id, created
                    FROM    student_promo_log";

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
                                    logs.Add(new StudentPromoLog()
                                        {
                                            PromoId = reader.GetInt32(reader.GetOrdinal("promo_id")),
                                            StudentId = reader.GetInt32(reader.GetOrdinal("student_id")),
                                            Created = reader.GetDateTime(reader.GetOrdinal("created"))
                                        });
                                }

                                cacheProvider.Set(CacheId, logs);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    e.Data["SQL"] = sql;
                    throw e;
                }
            }

            return logs;
        }

        public static void Save(NpgsqlConnection connection, int studentId, IEnumerable<int> promoIds)
        {
            Delete(connection, studentId);

            if (promoIds != null && promoIds.Any())
            {
                ApplicationCache cacheProvider = new ApplicationCache();
                ICollection<StudentPromoLog> logs = cacheProvider.Get(CacheId, () => new List<StudentPromoLog>());

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
                        DateTime created = DateTime.Now.ToUniversalTime();

                        command.CommandText = sql;

                        command.Parameters.Add("@PromoId", NpgsqlTypes.NpgsqlDbType.Integer);
                        command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;
                        command.Parameters.Add("@Created", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = created;

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

                    cacheProvider.Set(CacheId, logs);
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
        /// <param name="studentId">Student ID.</param>
        /// <param name="promoCode">Promo code.</param>
        public static void Save(NpgsqlConnection connection, int studentId, string promoCode)
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            ICollection<StudentPromoLog> logs = cacheProvider.Get(CacheId, () => new List<StudentPromoLog>());

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
                    DateTime created = DateTime.Now.ToUniversalTime();

                    command.CommandText = sql;

                    command.Parameters.Add("@PromoCode", NpgsqlTypes.NpgsqlDbType.Varchar, 32).Value = promoCode.ToLower();
                    command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;
                    command.Parameters.Add("@Created", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = created;

                    int promoId = Convert.ToInt32(command.ExecuteScalar());

                    logs.Add(new StudentPromoLog()
                        {
                            PromoId = promoId,
                            StudentId = studentId,
                            Created = created
                        });

                    cacheProvider.Set(CacheId, logs);
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

        private static void Delete(NpgsqlConnection connection, int studentId)
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

            ApplicationCache cacheProvider = new ApplicationCache();
            IEnumerable<StudentPromoLog> logs = cacheProvider.Get(CacheId, () => new List<StudentPromoLog>());
            logs = logs.Where(x => x.StudentId != studentId);
            cacheProvider.Set(CacheId, logs);
        }
    }
}
