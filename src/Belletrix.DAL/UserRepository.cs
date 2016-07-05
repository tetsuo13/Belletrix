using Belletrix.Core;
using Belletrix.Entity.Model;
using StackExchange.Exceptional;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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

        public void UpdateLastLogin(string username)
        {
            const string sql = @"
                UPDATE  [dbo].[Users]
                SET     [LastLogin] = @LastLogin
                WHERE   [Login] = @Username";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@LastLogin", SqlDbType.DateTime).Value = DateTime.Now.ToUniversalTime();
                    command.Parameters.Add("@Username", SqlDbType.VarChar, 24).Value = username;

                    command.ExecuteNonQuery();
                }
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
            ICollection<UserModel> users = new List<UserModel>();

            string sql = @"
                SELECT  [Id], [FirstName], [LastName],
                        [Created], [LastLogin], [Email],
                        [Admin], [Active], [Login],
                        [PasswordIterations], [PasswordSalt], [PasswordHash],
                        [Password]
                FROM    [dbo].[Users] ";

            if (username != null)
            {
                sql += "WHERE [Login] = @Username";
            }

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    if (username != null)
                    {
                        command.Parameters.Add("@Username", SqlDbType.VarChar, 24).Value = username;
                    }

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            UserModel user = new UserModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                                FirstName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("FirstName")),
                                LastName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("LastName")),
                                Login = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Login")),
                                Created = DateTimeFilter.UtcToLocal(await reader.GetFieldValueAsync<DateTime>(reader.GetOrdinal("Created"))),
                                Email = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Email")),
                                IsAdmin = await reader.GetFieldValueAsync<bool>(reader.GetOrdinal("Admin")),
                                IsActive = await reader.GetFieldValueAsync<bool>(reader.GetOrdinal("Active")),
                                PasswordIterations = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("PasswordIterations")),
                                PasswordSalt = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("PasswordSalt")),
                                Password = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("PasswordHash")),
                                PasswordHash = await reader.GetValueOrDefault<string>("Password")
                            };

                            int ord = reader.GetOrdinal("LastLogin");

                            if (!reader.IsDBNull(ord))
                            {
                                user.LastLogin = DateTimeFilter.UtcToLocal(await reader.GetFieldValueAsync<DateTime>(ord));
                            }

                            users.Add(user);
                        }
                    }
                }
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
            IEnumerable<UserModel> users = await GetUsers(username);

            if (users.Any())
            {
                return users.First();
            }

            throw new Exception("User not found");
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

        public void Update(UserModel model)
        {
            const string sql = @"
                UPDATE  [dbo].[Users]
                SET     [FirstName] = @FirstName,
                        [LastName] = @LastName,
                        [Email] = @Email,
                        [PasswordIterations] = 0,
                        [PasswordSalt] = '',
                        [PasswordHash] = '',
                        [Password] = @Password,
                        [Admin] = @Admin,
                        [Active] = @Active
                WHERE   [Id] = @Id";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql.ToString();
                    command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 64).Value = model.FirstName.Trim();
                    command.Parameters.Add("@LastName", SqlDbType.NVarChar, 64).Value = model.LastName.Trim();
                    command.Parameters.Add("@Email", SqlDbType.VarChar, 128).Value = model.Email.Trim();
                    command.Parameters.Add("@Password", SqlDbType.VarChar).Value = model.PasswordHash;
                    command.Parameters.Add("@Admin", SqlDbType.Bit).Value = model.IsAdmin;
                    command.Parameters.Add("@Active", SqlDbType.Bit).Value = model.IsActive;
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = model.Id;

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql.ToString();
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        public void InsertUser(UserModel model)
        {
            const string sql = @"
                INSERT INTO [dbo].[Users]
                (
                    [FirstName], [LastName], [Login],
                    [Created], [Email], [Admin], [Active],
                    [PasswordIterations], [PasswordSalt], [PasswordHash], [Password]
                )
                VALUES
                (
                    @FirstName, @LastName, @Login,
                    @Created, @Email, @Admin, @Active,
                    0, '', '', @Password
                )";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 64).Value = model.FirstName.Trim();
                    command.Parameters.Add("@LastName", SqlDbType.NVarChar, 64).Value = model.LastName.Trim();
                    command.Parameters.Add("@Login", SqlDbType.VarChar, 24).Value = model.Login.Trim();
                    command.Parameters.Add("@Created", SqlDbType.DateTime).Value = DateTime.Now.ToUniversalTime();
                    command.Parameters.Add("@Email", SqlDbType.VarChar, 128).Value = model.Email.Trim();
                    command.Parameters.Add("@Admin", SqlDbType.Bit).Value = model.IsAdmin;
                    command.Parameters.Add("@Active", SqlDbType.Bit).Value = model.IsActive;
                    command.Parameters.Add("@Password", SqlDbType.VarChar).Value = model.PasswordHash;

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw;
            }
        }

        public void SaveChanges()
        {
            UnitOfWork.SaveChanges();
        }
    }
}
