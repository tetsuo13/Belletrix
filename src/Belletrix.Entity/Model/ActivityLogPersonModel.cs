using System;

namespace Belletrix.Entity.Model
{
    public class ActivityLogPersonModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Description { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public Guid? SessionId { get; set; }
    }
}
