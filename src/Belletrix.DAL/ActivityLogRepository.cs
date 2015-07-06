using Belletrix.Core;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public class ActivityLogRepository
    {
        public async Task<ActivityLogModel> GetActivityLogById(int id)
        {
            const string sql = @"
                SELECT      id, title, title2, title3,
                            organizers, location, types, start_date,
                            end_date, on_campus, web_site, notes,
                            created, created_by
                FROM        activity_log
                WHERE       id = @Id";

            ActivityLogModel activity = null;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.Add("@Id", NpgsqlTypes.NpgsqlDbType.Numeric).Value = id;
                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                activity = await ProcessRow(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = e;
                throw e;
            }

            return activity;
        }

        private async Task<ActivityLogModel> ProcessRow(DbDataReader reader)
        {
            return new ActivityLogModel()
            {
                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("id")),
                Created = await reader.GetFieldValueAsync<DateTime>(reader.GetOrdinal("created")),
                CreatedBy = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("created_by")),
                Title = await reader.GetText("title"),
                Title2 = await reader.GetText("title2"),
                Title3 = await reader.GetText("title3"),
                Location = await reader.GetText("location"),
                StartDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("start_date"))),
                EndDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("end_date"))),
                Types = await reader.GetFieldValueAsync<ActivityLogTypes[]>(reader.GetOrdinal("types")),
                Organizers = await reader.GetText("organizers"),
                OnCampus = await reader.GetFieldValueAsync<bool>(reader.GetOrdinal("on_campus")),
                WebSite = await reader.GetText("web_site"),
                Notes = await reader.GetText("notes")
            };
        }

        public async Task<IEnumerable<ActivityLogModel>> GetAll()
        {
            const string sql = @"
                SELECT      id, title, title2, title3,
                            organizers, location, types, start_date,
                            end_date, on_campus, web_site, notes,
                            created, created_by
                FROM        activity_log
                ORDER BY    created_by DESC";

            ICollection<ActivityLogModel> activities = new List<ActivityLogModel>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        await connection.OpenAsync();

                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                activities.Add(await ProcessRow(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = e;
                throw e;
            }

            return activities;
        }

        public async Task Create(ActivityLogModel model, int userId)
        {
            const string sql = @"
                INSERT INTO activity_log
                (
                    title, title2, title3, organizers,
                    location, types, start_date, end_date,
                    on_campus, web_site, notes, created,
                    created_by
                )
                VALUES
                (
                    @Title, @Title2, @Title3, @Organizers,
                    @Location, @Types, @StartDate, @EndDate,
                    @OnCampus, @WebSite, @Notes, @Created,
                    @CreatedBy
                )";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;

                        command.Parameters.Add("@Title", NpgsqlTypes.NpgsqlDbType.Varchar, 256).Value = model.Title;
                        command.Parameters.Add("@Title2", NpgsqlTypes.NpgsqlDbType.Varchar, 256).Value = !String.IsNullOrEmpty(model.Title2) ? (object)model.Title2 : DBNull.Value;
                        command.Parameters.Add("@Title3", NpgsqlTypes.NpgsqlDbType.Varchar, 256).Value = !String.IsNullOrEmpty(model.Title3) ? (object)model.Title3 : DBNull.Value;
                        command.Parameters.Add("@Organizers", NpgsqlTypes.NpgsqlDbType.Varchar, 256).Value = !String.IsNullOrEmpty(model.Organizers) ? (object)model.Organizers : DBNull.Value;
                        command.Parameters.Add("@Location", NpgsqlTypes.NpgsqlDbType.Varchar, 512).Value = !String.IsNullOrEmpty(model.Location) ? (object)model.Location : DBNull.Value;
                        command.Parameters.Add("@Types", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer).Value = model.Types;
                        command.Parameters.Add("@StartDate", NpgsqlTypes.NpgsqlDbType.Date).Value = model.StartDate.ToUniversalTime();
                        command.Parameters.Add("@EndDate", NpgsqlTypes.NpgsqlDbType.Date).Value = model.EndDate.ToUniversalTime();
                        command.Parameters.Add("@OnCampus", NpgsqlTypes.NpgsqlDbType.Boolean).Value = model.OnCampus;
                        command.Parameters.Add("@WebSite", NpgsqlTypes.NpgsqlDbType.Varchar, 2048).Value = !String.IsNullOrEmpty(model.WebSite) ? (object)model.WebSite : DBNull.Value;
                        command.Parameters.Add("@Notes", NpgsqlTypes.NpgsqlDbType.Varchar, 4096).Value = !String.IsNullOrEmpty(model.Notes) ? (object)model.Notes : DBNull.Value;
                        command.Parameters.Add("@Created", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();
                        command.Parameters.Add("@CreatedBy", NpgsqlTypes.NpgsqlDbType.Integer).Value = userId;

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = e;
                throw e;
            }
        }

        public async Task Save(ActivityLogModel model)
        {
            const string sql = @"
                UPDATE  activity_log
                SET     title = @Title,
                        title2 = @Title2,
                        title3 = @Title3,
                        organizers = @Organizers,
                        location = @Location,
                        types = @Types,
                        start_date = @StartDate,
                        end_date = @EndDate,
                        on_campus = @OnCampus,
                        web_site = @WebSite,
                        notes = @Notes
                WHERE   id = @Id";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;

                        command.Parameters.Add("@Title", NpgsqlTypes.NpgsqlDbType.Varchar, 256).Value = model.Title;
                        command.Parameters.Add("@Title2", NpgsqlTypes.NpgsqlDbType.Varchar, 256).Value = !String.IsNullOrEmpty(model.Title2) ? (object)model.Title2 : DBNull.Value;
                        command.Parameters.Add("@Title3", NpgsqlTypes.NpgsqlDbType.Varchar, 256).Value = !String.IsNullOrEmpty(model.Title3) ? (object)model.Title3 : DBNull.Value;
                        command.Parameters.Add("@Organizers", NpgsqlTypes.NpgsqlDbType.Varchar, 256).Value = !String.IsNullOrEmpty(model.Organizers) ? (object)model.Organizers : DBNull.Value;
                        command.Parameters.Add("@Location", NpgsqlTypes.NpgsqlDbType.Varchar, 512).Value = !String.IsNullOrEmpty(model.Location) ? (object)model.Location : DBNull.Value;
                        command.Parameters.Add("@Types", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer).Value = model.Types;
                        command.Parameters.Add("@StartDate", NpgsqlTypes.NpgsqlDbType.Date).Value = model.StartDate.ToUniversalTime();
                        command.Parameters.Add("@EndDate", NpgsqlTypes.NpgsqlDbType.Date).Value = model.EndDate.ToUniversalTime();
                        command.Parameters.Add("@OnCampus", NpgsqlTypes.NpgsqlDbType.Boolean).Value = model.OnCampus;
                        command.Parameters.Add("@WebSite", NpgsqlTypes.NpgsqlDbType.Varchar, 2048).Value = !String.IsNullOrEmpty(model.WebSite) ? (object)model.WebSite : DBNull.Value;
                        command.Parameters.Add("@Notes", NpgsqlTypes.NpgsqlDbType.Varchar, 4096).Value = !String.IsNullOrEmpty(model.Notes) ? (object)model.Notes : DBNull.Value;
                        command.Parameters.Add("@Id", NpgsqlTypes.NpgsqlDbType.Integer).Value = model.Id;

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = e;
                throw e;
            }
        }
    }
}
