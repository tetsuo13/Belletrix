using Belletrix.Core;
using Belletrix.Entity.Enum;
using System.Linq;
using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Belletrix.DAL
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly SqlConnection DbContext;
        private readonly IUnitOfWork UnitOfWork;

        public ActivityLogRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            DbContext = unitOfWork.DbContext;
        }

        public async Task<ActivityLogModel> GetActivityById(int id)
        {
            const string sql = @"
                SELECT  [Id], [Title], [Title2], [Title3],
                        [Organizers], [Location], [Types], [StartDate],
                        [EndDate], [OnCampus], [WebSite], [Notes],
                        [Created], [CreatedBy]
                FROM    [dbo].[AtivityLog]
                WHERE   [Id] = @Id";

            ActivityLogModel activity = null;

            try
            {
                using (SqlCommand command = DbContext.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            activity = await ProcessRow(reader);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }

            return activity;
        }

        private async Task<ActivityLogModel> ProcessRow(SqlDataReader reader)
        {
            return new ActivityLogModel()
            {
                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                Created = await reader.GetFieldValueAsync<DateTime>(reader.GetOrdinal("Created")),
                CreatedBy = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("CreatedBy")),
                Title = await reader.GetText("Title"),
                Title2 = await reader.GetText("Title2"),
                Title3 = await reader.GetText("Title3"),
                Location = await reader.GetText("Location"),
                StartDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("StartDate"))),
                EndDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("EndDate"))),
                Types = await reader.GetFieldValueAsync<ActivityLogTypes[]>(reader.GetOrdinal("Types")),
                Organizers = await reader.GetText("Organizers"),
                OnCampus = await reader.GetFieldValueAsync<bool>(reader.GetOrdinal("OnCampus")),
                WebSite = await reader.GetText("WebSite"),
                Notes = await reader.GetText("Notes")
            };
        }

        public async Task<IEnumerable<ActivityLogModel>> GetAllActivities()
        {
            const string sql = @"
                SELECT      [Id], [Title], [Title2], [Title3],
                            [Organizers], [Location], [Types], [StartDate],
                            [EndDate], [OnCampus], [WebSite], [Notes],
                            [Created], [CreatedBy]
                FROM        [dbo].[ActivityLog]
                ORDER BY    [CreatedBy] DESC";

            ICollection<ActivityLogModel> activities = new List<ActivityLogModel>();

            try
            {
                using (SqlCommand command = DbContext.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            activities.Add(await ProcessRow(reader));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }

            return activities;
        }

        public async Task<int> InsertActivity(ActivityLogModel model, int userId)
        {
            const string sql = @"
                INSERT INTO [dbo].[ActivityLog]
                (
                    [Title], [Title2], [Title3], [Organizers],
                    [Location], [Types], [StartDate], [EndDate],
                    [OnCampus], [WebSite], [Notes], [Created],
                    [CreatedBy]
                )
                OUTPUT INSERTING.Id
                VALUES
                (
                    @Title, @Title2, @Title3, @Organizers,
                    @Location, @Types, @StartDate, @EndDate,
                    @OnCampus, @WebSite, @Notes, @Created,
                    @CreatedBy
                )";

            int id;

            try
            {
                using (SqlCommand command = DbContext.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.Add("@Title", SqlDbType.VarChar, 256).Value = model.Title;
                    command.Parameters.Add("@Title2", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Title2) ? (object)model.Title2 : DBNull.Value;
                    command.Parameters.Add("@Title3", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Title3) ? (object)model.Title3 : DBNull.Value;
                    command.Parameters.Add("@Organizers", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Organizers) ? (object)model.Organizers : DBNull.Value;
                    command.Parameters.Add("@Location", SqlDbType.VarChar, 512).Value = !String.IsNullOrEmpty(model.Location) ? (object)model.Location : DBNull.Value;
                    command.Parameters.Add("@Types", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer).Value = model.Types;
                    command.Parameters.Add("@StartDate", SqlDbType.Date).Value = model.StartDate.ToUniversalTime();
                    command.Parameters.Add("@EndDate", SqlDbType.Date).Value = model.EndDate.ToUniversalTime();
                    command.Parameters.Add("@OnCampus", SqlDbType.Bit).Value = model.OnCampus;
                    command.Parameters.Add("@WebSite", SqlDbType.VarChar, 2048).Value = !String.IsNullOrEmpty(model.WebSite) ? (object)model.WebSite : DBNull.Value;
                    command.Parameters.Add("@Notes", SqlDbType.VarChar, 4096).Value = !String.IsNullOrEmpty(model.Notes) ? (object)model.Notes : DBNull.Value;
                    command.Parameters.Add("@Created", SqlDbType.DateTime).Value = DateTime.Now.ToUniversalTime();
                    command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = userId;

                    id = (int)await command.ExecuteScalarAsync();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }

            return id;
        }

        public async Task UpdateActivity(ActivityLogModel model)
        {
            const string sql = @"
                UPDATE  [dbo].[ActivityLog]
                SET     [Title] = @Title,
                        [Title2] = @Title2,
                        [Title3] = @Title3,
                        [Organizers] = @Organizers,
                        [Location] = @Location,
                        [Types] = @Types,
                        [StartDate] = @StartDate,
                        [EndDate] = @EndDate,
                        [OnCampus] = @OnCampus,
                        [WebSite] = @WebSite,
                        [Notes] = @Notes
                WHERE   [Id] = @Id";

            try
            {
                using (SqlCommand command = DbContext.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.Add("@Title", SqlDbType.VarChar, 256).Value = model.Title;
                    command.Parameters.Add("@Title2", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Title2) ? (object)model.Title2 : DBNull.Value;
                    command.Parameters.Add("@Title3", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Title3) ? (object)model.Title3 : DBNull.Value;
                    command.Parameters.Add("@Organizers", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Organizers) ? (object)model.Organizers : DBNull.Value;
                    command.Parameters.Add("@Location", SqlDbType.VarChar, 512).Value = !String.IsNullOrEmpty(model.Location) ? (object)model.Location : DBNull.Value;
                    command.Parameters.Add("@Types", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer).Value = model.Types;
                    command.Parameters.Add("@StartDate", SqlDbType.Date).Value = model.StartDate.ToUniversalTime();
                    command.Parameters.Add("@EndDate", SqlDbType.Date).Value = model.EndDate.ToUniversalTime();
                    command.Parameters.Add("@OnCampus", SqlDbType.Bit).Value = model.OnCampus;
                    command.Parameters.Add("@WebSite", SqlDbType.VarChar, 2048).Value = !String.IsNullOrEmpty(model.WebSite) ? (object)model.WebSite : DBNull.Value;
                    command.Parameters.Add("@Notes", SqlDbType.VarChar, 4096).Value = !String.IsNullOrEmpty(model.Notes) ? (object)model.Notes : DBNull.Value;
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = model.Id;

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }
        }

        public async Task SaveChanges()
        {
            UnitOfWork.SaveChanges();
        }
    }
}
