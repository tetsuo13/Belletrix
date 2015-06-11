using Belletrix.Core;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Belletrix.Models
{
    public class ActivityLogModel
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public int CreatedBy { get; set; }

        public string Title { get; set; }

        public string Title2 { get; set; }

        public string Title3 { get; set; }

        public string Organizers { get; set; }

        public string Location { get; set; }

        public enum ActivityType
        {
            Conference,
            Institute,
            Summit,
            Grant,
            Community,
            Student,
            SiteVisit
        }

        public ActivityType[] Types { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool OnCampus { get; set; }

        public string WebSite { get; set; }

        public string Notes { get; set; }

        public static IEnumerable<ActivityLogListViewModel> GetActivityLogs()
        {
            const string sql = @"
                SELECT      id, title, title2, title3,
                            organizers, location, types, start_date,
                            end_date, on_campus, web_site, notes,
                            created, created_by
                FROM        activity_log
                ORDER BY    created_by DESC";

            ICollection<ActivityLogListViewModel> activities = null;

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
                            if (reader.HasRows)
                            {
                                activities = new List<ActivityLogListViewModel>();

                                while (reader.Read())
                                {
                                    ActivityLogListViewModel activity = new ActivityLogListViewModel()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                                        Title = reader.GetString(reader.GetOrdinal("title")),
                                        Title2 = reader.GetText("title2"),
                                        Title3 = reader.GetText("title3"),
                                        StartDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("start_date"))),
                                        Types = reader["types"] as int[],
                                        Organizers = reader.GetText("organizers")
                                    };

                                    //ActivityLogModel activity = new ActivityLogModel()
                                    //{
                                    //    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    //    Created = reader.GetDateTime(reader.GetOrdinal("created")),
                                    //    CreatedBy = reader.GetInt32(reader.GetOrdinal("created_by")),
                                    //    Title = reader.GetString(reader.GetOrdinal("title")),
                                    //    Title2 = reader.GetText("title2"),
                                    //    Title3 = reader.GetText("title3"),
                                    //    Organizers = reader.GetText("organizers"),
                                    //    Location = reader.GetText("location"),
                                    //    StartDate = reader.GetDateTime(reader.GetOrdinal("start_date")),
                                    //    EndDate = reader.GetDateTime(reader.GetOrdinal("end_date")),
                                    //    OnCampus = reader.GetBoolean(reader.GetOrdinal("on_campus")),
                                    //    WebSite = reader.GetText("web_site"),
                                    //    Notes = reader.GetText("notes")
                                    //};

                                    //int[] types = reader["types"] as int[];
                                    //activity.Types = (ActivityType[])(object)types;

                                    activities.Add(activity);
                                }
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
    }
}
