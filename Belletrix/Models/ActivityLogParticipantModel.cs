using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Belletrix.Models
{
    public class ActivityLogParticipantModel
    {
        public ActivityLogModel Event { get; set; }

        public ActivityLogPersonModel Person { get; set; }

        public enum Types
        {
            Attendee,
            Contact
        }

        public Types Type { get; set; }
    }
}
