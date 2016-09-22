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
                            [PublicToken],
                            p.Active,
                            u.FirstName AS CreatedByFirstName,
                            u.LastName AS CreatedByLastName,
                            (SELECT COUNT(*) FROM [StudentPromoLog] WHERE [PromoId] = p.Id) AS TotalStudents
                FROM        [dbo].[UserPromo] p
                INNER JOIN  [dbo].[Users] u ON
                            [CreatedBy] = u.id
                ORDER BY    [Description]";

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

        public async Task<PromoViewModel> GetPromo(Guid token)
        {
            return (await GetPromos()).FirstOrDefault(p => p.PublicToken == token);
        }

        public async Task<int> Save(UserPromoModel model, int userId)
        {
            const string sql = @"
                INSERT INTO [dbo].[UserPromo]
                ([Description], [CreatedBy], [Created], [Active], [PublicToken])
                OUTPUT INSERTED.Id
                VALUES
                (@Description, @CreatedBy, @Created, @Active, @PublicToken)";

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

        public async Task<bool> Delete(int id)
        {
            const string sql = @"
                DELETE FROM [UserPromo]
                WHERE       [Id] = @Id";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql, new { Id = id });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                return false;
            }

            return true;
        }
    }
}
