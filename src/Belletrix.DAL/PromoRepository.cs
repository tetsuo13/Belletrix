using Belletrix.Core;
using Belletrix.Entity.Model;
using StackExchange.Exceptional;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Belletrix.DAL
{
    public class PromoRepository : IPromoRepository
    {
        private readonly IUnitOfWork UnitOfWork;

        public PromoRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PromoModel>> GetPromos()
        {
            ICollection<PromoModel> promos = new List<PromoModel>();
            const string sql = @"
                SELECT      p.Id AS PromoId, [Description], [CreatedBy], p.Created, [Code], p.Active,
                            u.FirstName, u.LastName,
                            (SELECT COUNT(*) FROM [StudentPromoLog] WHERE [PromoId] = p.Id) AS NumStudents
                FROM        [dbo].[UserPromo] p
                INNER JOIN  [dbo].[Users] u ON
                            [CreatedBy] = u.id
                ORDER BY    [Code]";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PromoModel promo = new PromoModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("PromoId")),
                                Description = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Description")),
                                Created = DateTimeFilter.UtcToLocal(await reader.GetFieldValueAsync<DateTime>(reader.GetOrdinal("Created"))),
                                Code = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Code")),
                                IsActive = await reader.GetFieldValueAsync<bool>(reader.GetOrdinal("Active")),
                                CreatedById = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("CreatedBy")),
                                CreatedByFirstName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("FirstName")),
                                CreatedByLastName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("LastName")),
                                Stuents = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("NumStudents"))
                            };

                            promos.Add(promo);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return promos;
        }

        public async Task<PromoModel> GetPromo(int id)
        {
            return (await GetPromos()).FirstOrDefault(p => p.Id == id);
        }

        public async Task<PromoModel> GetPromo(string code)
        {
            return (await GetPromos()).FirstOrDefault(p => p.Code == code.ToLower());
        }

        public async Task Save(PromoModel model, int userId)
        {
            const string sql = @"
                INSERT INTO [dbo].[UserPromo]
                ([Description], [CreatedBy], [Created], [Code], [Active])
                OUTPUT INSERTED.Id
                VALUES
                (@Description, @CreatedBy, @Created, @Code, @Active)";

            DateTime created = DateTime.Now.ToUniversalTime();

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.Add("@Description", SqlDbType.VarChar, 256).Value = model.Description;
                    command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = userId;
                    command.Parameters.Add("@Created", SqlDbType.DateTime).Value = created;
                    command.Parameters.Add("@Code", SqlDbType.VarChar, 32).Value = model.Code.ToLower();
                    command.Parameters.Add("@Active", SqlDbType.Bit).Value = true;

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        public async Task<bool> CheckNameForUniqueness(string name)
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
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@Code", SqlDbType.VarChar, 32).Value = name.ToLower();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        result = !reader.HasRows;
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return result;
        }

        public async Task<IEnumerable<PromoModel>> AsSources()
        {
            ICollection<PromoModel> promos = new List<PromoModel>();
            const string sql = @"
                SELECT      [Id], [Code], [Description]
                FROM        [UserPromo]
                ORDER BY    [Description]";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            promos.Add(new PromoModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                                Code = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Code")),
                                Description = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Description"))
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return promos;
        }
    }
}
