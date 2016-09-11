using Belletrix.Core;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
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

        public async Task<IEnumerable<PromoViewModel>> GetPromos()
        {
            List<PromoViewModel> promos = new List<PromoViewModel>();

            // Must convert the Guid column otherwise you get: "Invalid cast
            // from 'System.String' to 'System.Guid'" exception. See
            // https://github.com/StackExchange/dapper-dot-net/issues/447
            const string sql = @"
                SELECT      p.Id AS Id,
                            [Description],
                            p.Created,
                            [Code],
                            CONVERT(UNIQUEIDENTIFIER, PublicToken) AS PublicToken,
                            p.Active,
                            u.FirstName AS CreatedByFirstName,
                            u.LastName AS CreatedByLastName,
                            (SELECT COUNT(*) FROM [StudentPromoLog] WHERE [PromoId] = p.Id) AS Students
                FROM        [dbo].[UserPromo] p
                INNER JOIN  [dbo].[Users] u ON
                            [CreatedBy] = u.id
                ORDER BY    [Code]";

            try
            {
                promos = (await UnitOfWork.Context().QueryAsync<PromoViewModel>(sql)).ToList();
                promos.ForEach(x => x.Created = DateTimeFilter.UtcToLocal(x.Created));
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return promos;
        }

        public async Task<PromoViewModel> GetPromo(int id)
        {
            return (await GetPromos()).FirstOrDefault(p => p.Id == id);
        }

        public async Task<PromoViewModel> GetPromo(string code)
        {
            return (await GetPromos()).FirstOrDefault(p => p.Code == code.ToLower());
        }

        public async Task<int> Save(UserPromoModel model, int userId)
        {
            const string sql = @"
                INSERT INTO [dbo].[UserPromo]
                ([Description], [CreatedBy], [Created], [Code], [Active], [PublicToken])
                OUTPUT INSERTED.Id
                VALUES
                (@Description, @CreatedBy, @Created, @Code, @Active, @PublicToken)";

            try
            {
                return (await UnitOfWork.Context().QueryAsync<int>(sql, model)).Single();
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

        public async Task<IEnumerable<PromoSourceViewModel>> AsSources()
        {
            const string sql = @"
                SELECT      [Id], [Description]
                FROM        [UserPromo]
                ORDER BY    [Description]";

            try
            {
                return await UnitOfWork.Context().QueryAsync<PromoSourceViewModel>(sql);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return Enumerable.Empty<PromoSourceViewModel>();
        }
    }
}
