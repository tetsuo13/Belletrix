using System;

namespace Belletrix.Entity.Model
{
    public class EventLogModel
    {
        public DateTime Date { get; set; }
        public int? ModifiedBy { get; set; }
        public int StudentId { get; set; }
        public int Type { get; set; }
    }
}
