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
    public class UserRepository : IUserRepository
    {
        private readonly IUnitOfWork UnitOfWork;

        public UserRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task UpdateLastLogin(string username)
        {
            const string sql = @"
                UPDATE  [dbo].[Users]
                SET     [LastLogin] = @LastLogin
                WHERE   [Login] = @Username";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql,
                    new
                    {
                        LastLogin = DateTime.Now.ToUniversalTime(),
                        Username = username
                    });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        public async Task<IEnumerable<UserModel>> GetUsers(string username = null)
        {
            List<UserModel> users = new List<UserModel>();

            string sql = @"
                SELECT  [Id], [FirstName], [LastName],
                        [Created], [LastLogin], [Email],
                        [Admin] AS IsAdmin, [Active] AS IsActive, [Login],
                        [Password]
                FROM    [dbo].[Users] ";

            if (username != null)
            {
                sql += "WHERE [Login] = @Username";
            }

            try
            {
                if (username != null)
                {
                    users = (await UnitOfWork.Context().QueryAsync<UserModel>(sql, new { Username = username })).ToList();
                }
                else
                {
                    users = (await UnitOfWork.Context().QueryAsync<UserModel>(sql)).ToList();
                }

                users.ForEach(x =>
                {
                    x.Created = DateTimeFilter.UtcToLocal(x.Created);
                    x.LastLogin = DateTimeFilter.UtcToLocal(x.LastLogin);
                });

                // TODO: LastLogin may need to be changed to a nullable type.
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            return users;
        }

        public async Task<UserModel> GetUser(string username)
        {
            return (await GetUsers(username)).FirstOrDefault();
        }

        public async Task<UserModel> GetUser(int id)
        {
            UserModel user = (await GetUsers()).FirstOrDefault(x => x.Id == id);

            if (user != null)
            {
                return user;
            }

            throw new Exception("User not found");
        }

        public async Task Update(UserModel model)
        {
            const string sql = @"
                UPDATE  [dbo].[Users]
                SET     [FirstName] = @FirstName,
                        [LastName] = @LastName,
                        [Email] = @Email,
                        [Password] = @Password,
                        [Admin] = @Admin,
                        [Active] = @Active
                WHERE   [Id] = @Id";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql,
                    new
                    {
                        FirstName = model.FirstName.Trim(),
                        LastName = model.LastName.Trim(),
                        Email = model.Email.Trim(),
                        Password = model.Password,
                        Admin = model.IsAdmin,
                        Active = model.IsActive,
                        Id = model.Id
                    });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql.ToString();
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        public async Task InsertUser(UserModel model)
        {
            const string sql = @"
                INSERT INTO [dbo].[Users]
                (
                    [FirstName], [LastName], [Login],
                    [Created], [Email], [Admin], [Active],
                    [Password]
                )
                VALUES
                (
                    @FirstName, @LastName, @Login,
                    @Created, @Email, @Admin, @Active,
                    @Password
                )";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql,
                    new
                    {
                        FirstName = model.FirstName.Trim(),
                        LastName = model.LastName.Trim(),
                        Login = model.Login.Trim(),
                        Created = DateTime.Now.ToUniversalTime(),
                        Email = model.Email.Trim().ToLower(),
                        Admin = model.IsAdmin,
                        Active = model.IsActive,
                        Password = model.Password
                    });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            const string sql = @"
                DELETE FROM [dbo].[Users]
                WHERE   [Id] = @Id";

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
