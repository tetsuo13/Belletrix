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
    public class StudentPromoRepository : IStudentPromoRepository
    {
        private IUnitOfWork UnitOfWork;

        public StudentPromoRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StudentPromoLog>> Get()
        {
            const string sql = @"
                SELECT  [PromoId], [StudentId], [Created]
                FROM    [StudentPromoLog]";

            try
            {
                return await UnitOfWork.Context().QueryAsync<StudentPromoLog>(sql);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return new List<StudentPromoLog>();
        }

        public async Task Save(int studentId, IEnumerable<int> promoIds)
        {
            await Delete(studentId);

            if (promoIds != null && promoIds.Any())
            {
                const string sql = @"
                    INSERT INTO [dbo].[StudentPromoLog]
                    ([PromoId], [StudentId], [Created])
                    VALUES
                    (@PromoId, @StudentId, @Created)";

                try
                {
                    DateTime created = DateTime.Now.ToUniversalTime();

                    await UnitOfWork.Context().ExecuteAsync(sql,
                        new
                        {
                            PromoId = promoIds,
                            StudentId = studentId,
                            Created = created
                        });
                }
                catch (Exception e)
                {
                    e.Data["SQL"] = sql;
                    ErrorStore.LogException(e, HttpContext.Current);
                }
            }
        }

        public async Task<IEnumerable<int>> GetPromoIdsForStudent(int studentId)
        {
            return (await Get())
                .Where(x => x.StudentId == studentId)
                .Select(x => x.PromoId);
        }

        public async Task Save(int studentId, string promoCode)
        {
            const string sql = @"
                INSERT INTO [dbo].[StudentPromoLog]
                ([PromoId], [StudentId], [Created])
                VALUES
                (
                    (SELECT [Id] FROM [dbo].[UserPromo] WHERE [Code] = @PromoCode), @StudentId, @Created
                )";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql,
                    new
                    {
                        PromoCode = promoCode.ToLower(),
                        StudentId = studentId,
                        Created = DateTime.Now.ToUniversalTime()
                    });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }
        }

        public async Task<IEnumerable<StudentPromoLog>> GetLogsForPromo(int id)
        {
            return (await Get()).Where(x => x.PromoId == id);
        }

        private async Task Delete(int studentId)
        {
            const string sql = @"
                DELETE FROM [dbo].[StudentPromoLog]
                WHERE       [StudentId] = @StudentId";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql, new { StudentId = studentId });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }
    }
}
