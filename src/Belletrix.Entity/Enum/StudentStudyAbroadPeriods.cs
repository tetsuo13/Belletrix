using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.Enum
{
    public enum StudentStudyAbroadPeriods
    {
        Fall = 1,
        Spring,
        Summer,
        Maymester,

        [Display(Name = "Academic Year")]
        AcademicYear
    }
}
