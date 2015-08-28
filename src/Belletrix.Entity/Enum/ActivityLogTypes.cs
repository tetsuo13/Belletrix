using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.Enum
{
    public enum ActivityLogTypes
    {
        [Display(Name = "Conference/Institute/Summit")]
        ConferenceInstituteSummit = 1,
        Grant,
        Community,
        Student,

        [Display(Name = "Site Visit")]
        SiteVisit,

        Meeting,
        Other
    }
}
