using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace Bennett.AbroadAdvisor.Models
{
    public class StudentPromoLog
    {
        private const string CacheId = "StudentPromoLog";

        public int PromoId { get; set; }
        public int StudentId { get; set; }
        public StudentBaseModel Student { get; set; }

        public void Save(NpgsqlConnection connection, int studentId, string promoCode)
        {
            connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO student_promo_log
                    (
                        promo_id, student_id
                    )
                    VALUES
                    (
                        (SELECT id FROM user_promo WHERE code = @PromoCode), @StudentId
                    )";

                command.Parameters.Add("@PromoCode", NpgsqlTypes.NpgsqlDbType.Varchar, 32).Value = promoCode.ToLower();
                command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;

                command.ExecuteNonQuery();

                ApplicationCache cacheProvider = new ApplicationCache();
                List<StudentPromoLog> events = cacheProvider.Get(CacheId, () => new List<StudentPromoLog>());
                events.Add(this);
                cacheProvider.Set(CacheId, events);
            }
        }

        public static IEnumerable<StudentPromoLog> GetLogsForPromo(int id)
        {
            List<StudentPromoLog> logs = new List<StudentPromoLog>();

            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT  student_id
                        FROM    student_promo_log
                        WHERE   promo_id = @PromoId";

                    command.Parameters.Add("@PromoId", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;
                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudentPromoLog log = new StudentPromoLog()
                            {
                                PromoId = id,
                                StudentId = reader.GetInt32(reader.GetOrdinal("student_id"))
                            };

                            IList<StudentBaseModel> student = StudentModel.GetStudents(log.StudentId).ToList();
                            log.Student = student[0];

                            logs.Add(log);
                        }
                    }
                }
            }

            return logs;
        }
    }
}
