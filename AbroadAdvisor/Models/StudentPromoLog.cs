using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System.Collections.Generic;

namespace Bennett.AbroadAdvisor.Models
{
    public class StudentPromoLog
    {
        private const string CacheId = "StudentPromoLog";

        public int PromoId { get; set; }
        public int StudentId { get; set; }

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
    }
}
