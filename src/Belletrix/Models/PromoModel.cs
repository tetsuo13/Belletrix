using Belletrix.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Belletrix.Models
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
            ICollection<PromoModel> promos = new List<PromoModel>();
            const string sql = @"
                SELECT      p.Id AS PromoId, [Description], [CreatedBy], p.Created, [Code], p.Active,
                            u.FirstName, u.LastName
                FROM        [dbo].[UserPromo] p
                INNER JOIN  [dbo].[Users] u ON
                            [CreatedBy] = u.id
                ORDER BY    [Code]";

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
                                PromoModel promo = new PromoModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("PromoId")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Created = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("Created"))),
                                    Code = reader.GetString(reader.GetOrdinal("Code")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("Active"))
                                };

                                UserModel user = new UserModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
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
                INSERT INTO [dbo].[UserPromo]
                ([Description], [CreatedBy], [Created], [Code], [Active])
                OUTPUT INSERTED.Id
                VALUES
                (@Description, @CreatedBy, @Created, @Code, @Active)";

            Created = DateTime.Now.ToUniversalTime();

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;

                        command.Parameters.Add("@Description", SqlDbType.VarChar, 256).Value = Description;
                        command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = userId;
                        command.Parameters.Add("@Created", SqlDbType.DateTime).Value = Created;
                        command.Parameters.Add("@Code", SqlDbType.VarChar, 32).Value = Code.ToLower();
                        command.Parameters.Add("@Active", SqlDbType.Bit).Value = true;

                        connection.Open();
                        Id = (int)command.ExecuteScalar();
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
                SELECT  [Id]
                FROM    [UserPromo]
                WHERE   LOWER([Code]) = @Code";

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.Add("@Code", SqlDbType.VarChar, 32).Value = name.ToLower();
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
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
                SELECT      [Id], [Code], [Description]
                FROM        [UserPromo]
                ORDER BY    [Description]";

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
                                promos.Add(new PromoModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Code = reader.GetString(reader.GetOrdinal("Code")),
                                    Description = reader.GetString(reader.GetOrdinal("Description"))
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
