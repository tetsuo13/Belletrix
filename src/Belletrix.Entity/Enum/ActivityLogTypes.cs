using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.Enum
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Be sure to always add new items to the end of the list otherwise
    /// you'll overwrite an existing ID!
    /// </remarks>
    public enum ActivityLogTypes
    {
        [Display(Name = "Conference/Institute/Summit")]
        ConferenceInstituteSummit = 1,

        Grant,

        [Display(Name = "International Travel")]
        InternationalTravel,

        Community,
        Student,

        [Display(Name = "Site Visit")]
        SiteVisit,

        Meeting,
        Other,
        Workshop,

        [Display(Name = "Faculty Led Program")]
        FacultyLedProgram
    }
}
