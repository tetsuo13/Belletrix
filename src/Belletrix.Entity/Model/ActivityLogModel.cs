using Belletrix.Entity.Enum;
using System;

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

    }
}
