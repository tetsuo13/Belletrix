using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Bennett.AbroadAdvisor.Models
{
    public class StudentStudyAbroadWishlistModel
    {
        public const int CatchAllValue = 99;

        public enum PeriodValue
        {
            Fall,
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
            List<object> periods = new List<object>();

            foreach (PeriodValue value in Enum.GetValues(typeof(PeriodValue)))
            {
                periods.Add(new
                {
                    Id = (int)value,
                    Name = value
                });
            }

            return periods.AsEnumerable();
        }

        public static IEnumerable<object> GetPeriodsWithCatchAll()
        {
            List<object> catchAll = new List<object>()
            {
                new { Id = CatchAllValue, Name = "Any Semester" }
            };

            IEnumerable<object> periods = catchAll;

            return periods.Concat(GetPeriods());
        }
    }
}
