using System.Collections.Generic;

namespace Belletrix.Entity.ViewModel
{
    public class StudentSearchViewModel
    {
        public IEnumerable<int> SelectedMajors { get; set; }
        public IEnumerable<int> SelectedGraduatingYears { get; set; }
        public IEnumerable<int> SelectedCountries { get; set; }
    }
}
