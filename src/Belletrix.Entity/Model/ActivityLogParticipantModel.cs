using Belletrix.Entity.Enum;

namespace Belletrix.Entity.Model
{
    public class ActivityLogParticipantModel
    {
        public ActivityLogModel Event { get; set; }

        public ActivityLogPersonModel Person { get; set; }

        public ActivityLogParticipantTypes Type { get; set; }
    }
}
