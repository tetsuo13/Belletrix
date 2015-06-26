using Belletrix.Core;
using Belletrix.Entity.Model;
using Npgsql;
using System;
using System.Collections.Generic;

//int[] types = reader["types"] as int[];
//activity.Types = (ActivityType[])(object)types;

namespace Belletrix.DAL
{
    public class ActivityLogRepository
    {
        public ActivityLogModel GetActivityLogById(int id)
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
                        connection.Open();

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                activity = ProcessRow(reader);
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

        private ActivityLogModel ProcessRow(NpgsqlDataReader reader)
        {
            return new ActivityLogModel()
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Created = reader.GetDateTime(reader.GetOrdinal("created")),
                CreatedBy = reader.GetInt32(reader.GetOrdinal("created_by")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                Title2 = reader.GetText("title2"),
                Title3 = reader.GetText("title3"),
                Location = reader.GetText("location"),
                StartDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("start_date"))),
                EndDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("end_date"))),
                Types = reader["types"] as int[],
                Organizers = reader.GetText("organizers"),
                OnCampus = reader.GetBoolean(reader.GetOrdinal("on_campus")),
                WebSite = reader.GetText("web_site"),
                Notes = reader.GetText("notes")
            };
        }

        public IEnumerable<ActivityLogModel> GetAll()
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
                        connection.Open();

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                activities.Add(ProcessRow(reader));
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

        public void Create(ActivityLogModel model, int userId)
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
                    @Location, '{}', @StartDate, @EndDate,
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
                        command.Parameters.Add("@StartDate", NpgsqlTypes.NpgsqlDbType.Date).Value = model.StartDate.ToUniversalTime();
                        command.Parameters.Add("@EndDate", NpgsqlTypes.NpgsqlDbType.Date).Value = model.EndDate.ToUniversalTime();
                        command.Parameters.Add("@OnCampus", NpgsqlTypes.NpgsqlDbType.Boolean).Value = model.OnCampus;
                        command.Parameters.Add("@WebSite", NpgsqlTypes.NpgsqlDbType.Varchar, 2048).Value = !String.IsNullOrEmpty(model.WebSite) ? (object)model.WebSite : DBNull.Value;
                        command.Parameters.Add("@Notes", NpgsqlTypes.NpgsqlDbType.Varchar, 4096).Value = !String.IsNullOrEmpty(model.Notes) ? (object)model.Notes : DBNull.Value;
                        command.Parameters.Add("@Created", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();
                        command.Parameters.Add("@CreatedBy", NpgsqlTypes.NpgsqlDbType.Integer).Value = userId;

                        connection.Open();
                        command.ExecuteNonQuery();
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
