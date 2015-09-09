using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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
            ICollection<StudentPromoLog> logs = new List<StudentPromoLog>();

            const string sql = @"
                SELECT  [PromoId], [StudentId], [Created]
                FROM    [StudentPromoLog]";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            logs.Add(new StudentPromoLog()
                            {
                                PromoId = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("PromoId")),
                                StudentId = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("StudentId")),
                                Created = await reader.GetFieldValueAsync<DateTime>(reader.GetOrdinal("Created"))
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
            }

            return logs;
        }

        public async Task Save(int studentId, IEnumerable<int> promoIds)
        {
            await Delete(studentId);

            if (promoIds != null && promoIds.Any())
            {
                ICollection<StudentPromoLog> logs = new List<StudentPromoLog>();

                const string sql = @"
                    INSERT INTO [dbo].[StudentPromoLog]
                    ([PromoId], [StudentId], [Created])
                    VALUES
                    (@PromoId, @StudentId, @Created)";

                try
                {
                    using (SqlCommand command = UnitOfWork.CreateCommand())
                    {
                        DateTime created = DateTime.Now.ToUniversalTime();

                        command.CommandText = sql;

                        command.Parameters.Add("@PromoId", SqlDbType.Int);
                        command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                        command.Parameters.Add("@Created", SqlDbType.DateTime).Value = created;

                        command.Prepare();

                        foreach (int promoId in promoIds)
                        {
                            StudentPromoLog log = new StudentPromoLog()
                            {
                                PromoId = promoId,
                                StudentId = studentId,
                                Created = created
                            };

                            command.Parameters[0].Value = promoId;
                            await command.ExecuteNonQueryAsync();

                            logs.Add(log);
                        }
                    }
                }
                catch (Exception e)
                {
                    e.Data["SQL"] = sql;
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
            ICollection<StudentPromoLog> logs = new List<StudentPromoLog>();

            const string sql = @"
                INSERT INTO [dbo].[StudentPromoLog]
                ([PromoId], [StudentId], [Created])
                OUTPUT INSERTED.PromoId
                VALUES
                (
                    (SELECT [Id] FROM [dbo].[UserPromo] WHERE [Code] = @PromoCode), @StudentId, @Created
                )";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    DateTime created = DateTime.Now.ToUniversalTime();

                    command.CommandText = sql;

                    command.Parameters.Add("@PromoCode", SqlDbType.VarChar, 32).Value = promoCode.ToLower();
                    command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                    command.Parameters.Add("@Created", SqlDbType.DateTime).Value = created;

                    int promoId = Convert.ToInt32(await command.ExecuteScalarAsync());

                    logs.Add(new StudentPromoLog()
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
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        public void SaveChanges()
        {
            UnitOfWork.SaveChanges();
        }
    }
}
