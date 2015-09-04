using Belletrix.Entity.Enum;

namespace Belletrix.Entity.Model
{
    public class StudentStudyAbroadWishlistModel
    {
        public int StudentId { get; set; }
        public int CountryId { get; set; }
        public int Year { get; set; }
        public StudentStudyAbroadPeriods Period { get; set; }
    }
}
