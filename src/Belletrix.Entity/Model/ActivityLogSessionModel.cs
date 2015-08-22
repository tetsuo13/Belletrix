using System.Collections.Generic;

namespace Belletrix.Entity.Model
{
    /// <summary>
    /// Tabs on participants at the start of adding/editing an activity log
    /// and the participants at the end of it.
    /// </summary>
    public class ActivityLogSessionModel
    {
        /// <summary>
        /// Collection of participants at the start of editing an activity
        /// log. This will be an empty list when adding a new activity log.
        /// </summary>
        public List<ActivityLogParticipantModel> StartingList { get; set; }

        /// <summary>
        /// Collection of participants at the end of editing an activity log.
        /// </summary>
        public List<ActivityLogParticipantModel> WorkingList { get; set; }
    }
}
