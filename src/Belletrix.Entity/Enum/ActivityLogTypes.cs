using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.Enum
{
    public enum ActivityLogTypes
    {
        Conference = 1,
        Institute,
        Summit,
        Grant,
        Community,
        Student,

        [Display(Name="Site Visit")]
        SiteVisit,

        Meeting
    }
}
