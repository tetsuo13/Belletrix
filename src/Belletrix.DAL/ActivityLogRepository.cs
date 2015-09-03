using Belletrix.Core;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly IUnitOfWork UnitOfWork;

        public ActivityLogRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<ActivityLogModel> GetActivityById(int id)
        {
            const string sql = @"
                SELECT  [Id], [Title], [Title2], [Title3],
                        [Organizers], [Location], [StartDate],
                        [EndDate], [OnCampus], [WebSite], [Notes],
                        [Created], [CreatedBy]
                FROM    [dbo].[ActivityLog]
                WHERE   [Id] = @Id";

            ActivityLogModel activity = null;

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
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

            if (activity != null)
            {
                activity.Types = await GetActivityTypes(activity.Id);
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
                Title = await reader.GetValueOrDefault<string>("Title"),
                Title2 = await reader.GetValueOrDefault<string>("Title2"),
                Title3 = await reader.GetValueOrDefault<string>("Title3"),
                Location = await reader.GetValueOrDefault<string>("Location"),
                StartDate = DateTimeFilter.UtcToLocal(await reader.GetFieldValueAsync<DateTime>(reader.GetOrdinal("StartDate"))),
                EndDate = DateTimeFilter.UtcToLocal(await reader.GetFieldValueAsync<DateTime>(reader.GetOrdinal("EndDate"))),
                Organizers = await reader.GetValueOrDefault<string>("Organizers"),
                OnCampus = await reader.GetValueOrDefault<bool?>("OnCampus"),
                WebSite = await reader.GetValueOrDefault<string>("WebSite"),
                Notes = await reader.GetValueOrDefault<string>("Notes")
            };
        }

        public async Task<IEnumerable<ActivityLogModel>> GetAllActivities()
        {
            const string sql = @"
                SELECT      [Id], [Title], [Title2], [Title3],
                            [Organizers], [Location], [StartDate],
                            [EndDate], [OnCampus], [WebSite], [Notes],
                            [Created], [CreatedBy]
                FROM        [dbo].[ActivityLog]
                ORDER BY    [CreatedBy] DESC";

            ICollection<ActivityLogModel> activities = new List<ActivityLogModel>();

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
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

            foreach (ActivityLogModel activity in activities)
            {
                activity.Types = await GetActivityTypes(activity.Id);
            }

            return activities;
        }

        /// <summary>
        /// Get all types for a given activity log.
        /// </summary>
        /// <param name="activityId">Activity log ID.</param>
        /// <returns>Types for the activity.</returns>
        public async Task<ActivityLogTypes[]> GetActivityTypes(int activityId)
        {
            const string sql = @"
                SELECT  [TypeId]
                FROM    [dbo].[ActivityLogTypes]
                WHERE   [EventId] = @EventId";

            ICollection<int> types = new List<int>();

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@EventId", SqlDbType.Int).Value = activityId;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            types.Add(await reader.GetFieldValueAsync<int>(reader.GetOrdinal("TypeId")));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }

            return types.Cast<ActivityLogTypes>().ToArray();
        }

        /// <summary>
        /// Associates a collection of activity log types with an activity
        /// log. This performs an "upsert," meaning it can be used both when
        /// creating a new activity log or updating an existing one.
        /// </summary>
        /// <param name="activityId">Activity log ID.</param>
        /// <param name="types">One or more types to associate.</param>
        /// <returns>Nothing</returns>
        public async Task MergeActivityTypes(int activityId, IEnumerable<int> types)
        {
            const string deleteSql = @"
                DELETE FROM [dbo].[ActivityLogTypes]
                WHERE       [EventId] = @EventId";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = deleteSql;
                    command.Parameters.Add("@EventId", SqlDbType.Int).Value = activityId;
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = deleteSql;
                throw e;
            }

            const string insertSql = @"
                INSERT INTO [dbo].[ActivityLogTypes]
                ([EventId], [TypeId])
                VALUES
                (@EventId, @TypeId)";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = insertSql;
                    command.Parameters.Add("@EventId", SqlDbType.Int).Value = activityId;
                    command.Parameters.Add("@TypeId", SqlDbType.Int);

                    foreach (int type in types)
                    {
                        command.Parameters["@TypeId"].Value = type;
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = insertSql;
                throw e;
            }
        }

        public async Task<int> InsertActivity(ActivityLogModel model, int userId)
        {
            const string sql = @"
                INSERT INTO [dbo].[ActivityLog]
                (
                    [Title], [Title2], [Title3], [Organizers],
                    [Location], [StartDate], [EndDate], [OnCampus],
                    [WebSite], [Notes], [Created], [CreatedBy]
                )
                OUTPUT INSERTED.Id
                VALUES
                (
                    @Title, @Title2, @Title3, @Organizers,
                    @Location, @StartDate, @EndDate, @OnCampus,
                    @WebSite, @Notes, @Created, @CreatedBy
                )";

            int id;

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.Add("@Title", SqlDbType.VarChar, 256).Value = model.Title;
                    command.Parameters.Add("@Title2", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Title2) ? (object)model.Title2 : DBNull.Value;
                    command.Parameters.Add("@Title3", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Title3) ? (object)model.Title3 : DBNull.Value;
                    command.Parameters.Add("@Organizers", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Organizers) ? (object)model.Organizers : DBNull.Value;
                    command.Parameters.Add("@Location", SqlDbType.VarChar, 512).Value = !String.IsNullOrEmpty(model.Location) ? (object)model.Location : DBNull.Value;
                    command.Parameters.Add("@StartDate", SqlDbType.Date).Value = model.StartDate.ToUniversalTime();
                    command.Parameters.Add("@EndDate", SqlDbType.Date).Value = model.EndDate.ToUniversalTime();
                    command.Parameters.Add("@OnCampus", SqlDbType.Bit).Value = model.OnCampus.HasValue ? (object)model.OnCampus.Value : DBNull.Value;
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
                        [StartDate] = @StartDate,
                        [EndDate] = @EndDate,
                        [OnCampus] = @OnCampus,
                        [WebSite] = @WebSite,
                        [Notes] = @Notes
                WHERE   [Id] = @Id";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.Add("@Title", SqlDbType.VarChar, 256).Value = model.Title;
                    command.Parameters.Add("@Title2", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Title2) ? (object)model.Title2 : DBNull.Value;
                    command.Parameters.Add("@Title3", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Title3) ? (object)model.Title3 : DBNull.Value;
                    command.Parameters.Add("@Organizers", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Organizers) ? (object)model.Organizers : DBNull.Value;
                    command.Parameters.Add("@Location", SqlDbType.VarChar, 512).Value = !String.IsNullOrEmpty(model.Location) ? (object)model.Location : DBNull.Value;
                    command.Parameters.Add("@StartDate", SqlDbType.Date).Value = model.StartDate.ToUniversalTime();
                    command.Parameters.Add("@EndDate", SqlDbType.Date).Value = model.EndDate.ToUniversalTime();
                    command.Parameters.Add("@OnCampus", SqlDbType.Bit).Value = model.OnCampus.HasValue ? (object)model.OnCampus.Value : DBNull.Value;
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
