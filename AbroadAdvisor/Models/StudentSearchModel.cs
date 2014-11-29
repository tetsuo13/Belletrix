using System.Collections.Generic;

namespace Bennett.AbroadAdvisor.Models
{
    public class StudentSearchModel
    {
        public IEnumerable<int> SelectedMajors { get; set; }
        public IEnumerable<int> SelectedGraduatingYears { get; set; }
        public IEnumerable<int> SelectedCountries { get; set; }
    }
}
