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

                    foreach (int promoId in promoIds)
                    {
                        await UnitOfWork.Context().ExecuteAsync(sql,
                            new
                            {
                                PromoId = promoId,
                                StudentId = studentId,
                                Created = created
                            });
                    }
                }
                catch (Exception e)
                {
                    e.Data["SQL"] = sql;
                    ErrorStore.LogException(e, HttpContext.Current);
                }
            }
        }

        public async Task Save(int studentId, Guid promoToken)
        {
            const string sql = @"
                INSERT INTO [dbo].[StudentPromoLog]
                ([PromoId], [StudentId], [Created])
                VALUES
                (
                    (SELECT [Id] FROM [dbo].[UserPromo] WHERE [PublicToken] = @PromoToken), @StudentId, @Created
                )";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql,
                    new
                    {
                        PromoToken = promoToken,
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
