using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Bennett.AbroadAdvisor.Models
{
    public class PromoModel
    {
        private const string CacheId = "Promos";

        public int Id { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(256)]
        public string Description { get; set; }

        public UserModel CreatedBy { get; set; }

        public DateTime Created { get; set; }

        [Required]
        [Display(Name = "Unique Code")]
        [MaxLength(32)]
        public string Code { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<StudentPromoLog> Logs { get; set; }

        public static IEnumerable<PromoModel> GetPromos(bool withLogs = false)
        {
            const string sql = @"
                SELECT      p.id AS promo_id, description, created_by, p.created, code, p.active,
                            u.first_name, u.last_name
                FROM        user_promo p
                INNER JOIN  users u ON
                            created_by = u.id
                ORDER BY    code";
            ICollection<PromoModel> promos = new List<PromoModel>();

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
                                PromoModel promo = new PromoModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("promo_id")),
                                    Description = reader.GetString(reader.GetOrdinal("description")),
                                    Created = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("created"))),
                                    Code = reader.GetString(reader.GetOrdinal("code")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("active"))
                                };

                                UserModel user = new UserModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("created_by")),
                                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                                    LastName = reader.GetString(reader.GetOrdinal("last_name"))
                                };

                                promo.CreatedBy = user;

                                if (withLogs)
                                {
                                    promo.Logs = StudentPromoLog.GetLogsForPromo(promo.Id);
                                }

                                promos.Add(promo);
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

            return promos;
        }

        public static PromoModel GetPromo(int id)
        {
            IEnumerable<PromoModel> promos = GetPromos();
            PromoModel needle = promos.FirstOrDefault(p => p.Id == id);

            if (needle != null)
            {
                return needle;
            }

            return null;
        }

        public static PromoModel GetPromo(string code)
        {
            IEnumerable<PromoModel> promos = GetPromos();
            code = code.ToLower();
            PromoModel needle = promos.FirstOrDefault(p => p.Code == code);

            if (needle != null)
            {
                return needle;
            }

            return null;
        }

        public void Save(int userId)
        {
            const string sql = @"
                INSERT INTO user_promo
                (
                    description, created_by, created,
                    code, active
                )
                VALUES
                (
                    @Description, @CreatedBy, @Created,
                    @Code, @Active
                )
                RETURNING id";

            Created = DateTime.Now.ToUniversalTime();
            IsActive = true;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;
                    connection.Open();

                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        int promoId;

                        using (NpgsqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = sql;

                            command.Parameters.Add("@Description", NpgsqlTypes.NpgsqlDbType.Varchar, 256).Value = Description;
                            command.Parameters.Add("@CreatedBy", NpgsqlTypes.NpgsqlDbType.Integer).Value = userId;
                            command.Parameters.Add("@Created", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = Created;
                            command.Parameters.Add("@Code", NpgsqlTypes.NpgsqlDbType.Varchar, 32).Value = Code.ToLower();
                            command.Parameters.Add("@Active", NpgsqlTypes.NpgsqlDbType.Boolean).Value = IsActive;

                            promoId = (int)command.ExecuteScalar();
                        }

                        transaction.Commit();

                        ApplicationCache cacheProvider = new ApplicationCache();
                        List<PromoModel> promos = cacheProvider.Get(CacheId, () => new List<PromoModel>());
                        Id = promoId;
                        CreatedBy = UserModel.GetUser(userId);
                        promos.Add(this);
                        cacheProvider.Set(CacheId, promos);
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        public static bool CheckNameForUniqueness(string name)
        {
            bool result = false;

            if (String.IsNullOrEmpty(name))
            {
                return result;
            }

            const string sql = @"
                SELECT  id
                FROM    user_promo
                WHERE   LOWER(code) = @Code";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.Add("@Code", NpgsqlTypes.NpgsqlDbType.Varchar, 32).Value = name.ToLower();
                        connection.Open();

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            result = !reader.HasRows;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }

            return result;
        }

        public static IEnumerable<PromoModel> AsSources()
        {
            IList<PromoModel> promos = new List<PromoModel>();
            const string sql = @"
                SELECT      id, code, description
                FROM        user_promo
                ORDER BY    description";

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
                                promos.Add(new PromoModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Code = reader.GetString(reader.GetOrdinal("code")),
                                    Description = reader.GetString(reader.GetOrdinal("description"))
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

            return promos;
        }
    }
}
