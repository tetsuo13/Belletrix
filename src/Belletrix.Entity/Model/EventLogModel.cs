using System;

namespace Belletrix.Entity.Model
{
    public class EventLogModel
    {
        public int Id { get; set; }
        public DateTime EventDate { get; set; }

        /// <summary>
        /// References an <see cref="EventLogTypes"/> element.
        /// </summary>
        public int Type { get; set; }

        public string Action { get; set; }

        public int ModifiedById { get; set; }
        public string ModifiedByFirstName { get; set; }
        public string ModifiedByLastName { get; set; }

        public StudentBaseModel Student { get; set; }

        public int UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }

        public string RelativeDate { get; set; }
    }
}
