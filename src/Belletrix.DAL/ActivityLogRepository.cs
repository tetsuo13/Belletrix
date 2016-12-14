using Belletrix.Core;
using Belletrix.Entity.Enum;
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
                IEnumerable<dynamic> rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql,
                    new { Id = id });

                activity = ProcessRows(rows).SingleOrDefault();
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            if (activity != null)
            {
                activity.Types = await GetActivityTypes(activity.Id);
            }

            return activity;
        }

        private IEnumerable<ActivityLogModel> ProcessRows(IEnumerable<dynamic> rows)
        {
            ICollection<ActivityLogModel> logs = new List<ActivityLogModel>();

            foreach (IDictionary<string, object> row in rows)
            {
                logs.Add(new ActivityLogModel()
                {
                    Id = (int)row["Id"],
                    Created = DateTimeFilter.UtcToLocal((DateTime)row["Created"]),
                    CreatedBy = (int)row["CreatedBy"],
                    Title = row["Title"] as string,
                    Title2 = row["Title2"] as string,
                    Title3 = row["Title3"] as string,
                    Location = row["Location"] as string,
                    StartDate = DateTimeFilter.UtcToLocal((DateTime)row["StartDate"]),
                    EndDate = DateTimeFilter.UtcToLocal((DateTime)row["EndDate"]),
                    Organizers = row["Organizers"] as string,
                    OnCampus = row["OnCampus"] as bool?,
                    WebSite = row["WebSite"] as string,
                    Notes = row["Notes"] as string
                });
            }

            return logs;
        }

        public async Task<ActivityLogModel> GetActivityByTitle(string title)
        {
            const string sql = @"
                SELECT  [Id], [Title], [Title2], [Title3],
                        [Organizers], [Location], [StartDate],
                        [EndDate], [OnCampus], [WebSite], [Notes],
                        [Created], [CreatedBy]
                FROM    [dbo].[ActivityLog]
                WHERE   [Title] = @Title";

            ActivityLogModel activity = null;

            try
            {
                IEnumerable<dynamic> rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql,
                    new { Title = title });

                activity = ProcessRows(rows).SingleOrDefault();
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                activity = null;
            }

            return activity;
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

            List<ActivityLogModel> activities = new List<ActivityLogModel>();

            try
            {
                IEnumerable<dynamic> rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql);
                activities = ProcessRows(rows).ToList();
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
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

            ICollection<ActivityLogTypes> types = new List<ActivityLogTypes>();

            try
            {
                IEnumerable<dynamic> rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql,
                    new { EventId = activityId });

                foreach (IDictionary<string, object> row in rows)
                {
                    types.Add((ActivityLogTypes)(int)row["TypeId"]);
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            // TODO: Can we refactor this to return an ICollection or something rather than an array?
            return types.ToArray();
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
                await UnitOfWork.Context().ExecuteAsync(deleteSql, new { EventId = activityId });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = deleteSql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            const string insertSql = @"
                INSERT INTO [dbo].[ActivityLogTypes]
                ([EventId], [TypeId])
                VALUES
                (@EventId, @TypeId)";

            try
            {
                foreach (int typeId in types)
                {
                    await UnitOfWork.Context().ExecuteAsync(insertSql,
                        new
                        {
                            EventId = activityId,
                            TypeId = typeId
                        });
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = insertSql;
                ErrorStore.LogException(e, HttpContext.Current);
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

            try
            {
                model.StartDate = model.StartDate.ToUniversalTime();
                model.EndDate = model.EndDate.ToUniversalTime();
                model.Created = DateTime.Now.ToUniversalTime();
                model.CreatedBy = userId;

                return (await UnitOfWork.Context().QueryAsync<int>(sql, model)).Single();
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
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
                model.StartDate = model.StartDate.ToUniversalTime();
                model.EndDate = model.EndDate.ToUniversalTime();

                await UnitOfWork.Context().ExecuteAsync(sql, model);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        public async Task<bool> TransferOwnership(int oldId, int newId)
        {
            const string sql = @"
                UPDATE  [dbo].[ActivityLog]
                SET     [CreatedBy] = @NewId
                WHERE   [CreatedBy] = @OldId";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql, new { OldId = oldId, NewId = newId });
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
