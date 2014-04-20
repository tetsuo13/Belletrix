using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
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

        public static List<PromoModel> GetPromos()
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            List<PromoModel> promos = cacheProvider.Get(CacheId, () => new List<PromoModel>());

            if (promos.Count == 0)
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            SELECT      p.id AS promo_id, description, created_by, p.created, code, p.active,
                                        u.first_name, u.last_name
                            FROM        user_promo p
                            INNER JOIN  users u ON
                                        created_by = u.id
                            ORDER BY    code";

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

                                promos.Add(promo);
                            }

                            cacheProvider.Set(CacheId, promos);
                        }
                    }
                }
            }

            return promos;
        }

        public static PromoModel GetPromo(int id)
        {
            List<PromoModel> promos = GetPromos();
            PromoModel needle = promos.Where(p => p.Id == id).FirstOrDefault();

            if (needle != null)
            {
                return needle;
            }

            return null;
        }

        public static PromoModel GetPromo(string code)
        {
            List<PromoModel> promos = GetPromos();
            code = code.ToLower();
            PromoModel needle = promos.Where(p => p.Code == code).FirstOrDefault();

            if (needle != null)
            {
                return needle;
            }

            return null;
        }

        public void Save(int userId)
        {
            Created = DateTime.Now.ToUniversalTime();
            IsActive = true;

            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;
                connection.Open();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    int promoId;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
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

        public static bool CheckNameForUniqueness(string name)
        {
            bool result = false;

            if (String.IsNullOrEmpty(name))
            {
                return result;
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT  id
                        FROM    user_promo
                        WHERE   LOWER(code) = @Code";

                    command.Parameters.Add("@Code", NpgsqlTypes.NpgsqlDbType.Varchar, 32).Value = name.ToLower();
                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        result = !reader.HasRows;
                    }
                }
            }

            return result;
        }
    }
}
