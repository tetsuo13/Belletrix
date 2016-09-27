using System.Collections.Generic;

namespace Belletrix.Entity.ViewModel
{
    public class DashboardViewModel
    {
        public IEnumerable<EventLogViewModel> RecentActivity { get; set; }
    }
}
