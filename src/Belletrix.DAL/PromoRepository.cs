using Belletrix.Core;
using Belletrix.Entity.Model;
using Dapper;
using StackExchange.Exceptional;
using System;
using System.Collections.Generic;
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
            List<PromoModel> promos = new List<PromoModel>();
            const string sql = @"
                SELECT      p.Id AS Id, [Description], [CreatedBy] AS CreatedById, p.Created, [Code], p.Active,
                            u.FirstName AS CreatedByFirstName, u.LastName AS CreatedByLastName,
                            (SELECT COUNT(*) FROM [StudentPromoLog] WHERE [PromoId] = p.Id) AS Students
                FROM        [dbo].[UserPromo] p
                INNER JOIN  [dbo].[Users] u ON
                            [CreatedBy] = u.id
                ORDER BY    [Code]";

            try
            {
                promos = (await UnitOfWork.Context().QueryAsync<PromoModel>(sql)).ToList();
                promos.ForEach(x => x.Created = DateTimeFilter.UtcToLocal(x.Created));
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

        public async Task<int> Save(PromoModel model, int userId)
        {
            const string sql = @"
                INSERT INTO [dbo].[UserPromo]
                ([Description], [CreatedBy], [Created], [Code], [Active])
                OUTPUT INSERTED.Id
                VALUES
                (@Description, @CreatedBy, @Created, @Code, @Active)";

            try
            {
                return (await UnitOfWork.Context().QueryAsync<int>(sql,
                    new
                    {
                        Description = model.Description,
                        CreatedBy = userId,
                        Created = DateTime.Now.ToUniversalTime(),
                        Code = model.Code.ToLower(),
                        Active = true
                    })).Single();
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
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            const string sql = @"
                SELECT  COUNT([Id]) AS Count
                FROM    [UserPromo]
                WHERE   LOWER([Code]) = @Code";

            try
            {
                return (await UnitOfWork.Context().ExecuteScalarAsync<int>(sql, new { Code = name.ToLower() })) == 0;
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return false;
        }

        public async Task<IEnumerable<PromoModel>> AsSources()
        {
            const string sql = @"
                SELECT      [Id], [Code], [Description]
                FROM        [UserPromo]
                ORDER BY    [Description]";

            try
            {
                return await UnitOfWork.Context().QueryAsync<PromoModel>(sql);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return new List<PromoModel>();
        }
    }
}
