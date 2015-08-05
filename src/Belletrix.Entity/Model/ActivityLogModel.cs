using Belletrix.Entity.Enum;
using Belletrix.Entity.ViewModel;
using System;
using System.Linq;

namespace Belletrix.Entity.Model
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

        public ActivityLogTypes[] Types { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool OnCampus { get; set; }

        public string WebSite { get; set; }

        public string Notes { get; set; }

        public static explicit operator ActivityLogModel(ActivityLogCreateViewModel a)
        {
            ActivityLogModel model = new ActivityLogModel()
            {
                Title = a.Title,
                Title2 = a.Title2,
                Title3 = a.Title3,
                Organizers = a.Organizers,
                Location = a.Location,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                OnCampus = a.OnCampus,
                WebSite = a.WebSite,
                Notes = a.Notes,
                Types = a.Types.Cast<ActivityLogTypes>().ToArray()
            };

            if (a.Types != null)
            {
                model.Types = a.Types.Cast<ActivityLogTypes>().ToArray();
            }

            return model;
        }

        public static explicit operator ActivityLogModel(ActivityLogEditViewModel a)
        {
            ActivityLogModel model = new ActivityLogModel()
            {
                Id = a.Id,
                Title = a.Title,
                Title2 = a.Title2,
                Title3 = a.Title3,
                Organizers = a.Organizers,
                Location = a.Location,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                OnCampus = a.OnCampus,
                WebSite = a.WebSite,
                Notes = a.Notes
            };

            if (a.Types != null)
            {
                model.Types = a.Types.Cast<ActivityLogTypes>().ToArray();
            }

            return model;
        }
    }
}
