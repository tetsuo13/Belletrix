using System;
using System.Collections.Generic;
using System.Linq;

namespace Belletrix.Models
{
    public class StudentStudyAbroadWishlistModel
    {
        /// <summary>
        /// Value used to represent the "Please Select" option for the year
        /// select list.
        /// </summary>
        public const int CatchAllYearValue = 1;

        /// <summary>
        /// Value used to represent the "Please Select" option for the period
        /// select list.
        /// </summary>
        public const int CatchAllPeriodValue = 99;

        public enum PeriodValue
        {
            Fall = 1,
            Spring,
            Summer,
            Maymester,
            AcademicYear
        }

        public int StudentId { get; set; }
        public int CountryId { get; set; }
        public int Year { get; set; }
        public PeriodValue Period { get; set; }

        public static IEnumerable<object> GetPeriods()
        {
            ICollection<object> periods = new List<object>();

            foreach (PeriodValue value in Enum.GetValues(typeof(PeriodValue)))
            {
                periods.Add(new
                {
                    Id = (int)value,
                    Name = value
                });
            }

            return periods;
        }

        public static IEnumerable<object> GetPeriodsWithCatchAll()
        {
            IEnumerable<object> catchAll = new List<object>()
            {
                new { Id = CatchAllPeriodValue, Name = "Any Semester" }
            };

            IEnumerable<object> periods = catchAll;

            return periods.Concat(GetPeriods());
        }
    }
}
