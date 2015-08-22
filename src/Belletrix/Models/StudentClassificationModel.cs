using System.Collections.Generic;

namespace Belletrix.Models
{
    public class StudentClassificationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static IEnumerable<StudentClassificationModel> GetClassifications()
        {
            return new List<StudentClassificationModel>()
            {
                new StudentClassificationModel() { Id = 0, Name = "Freshman" },
                new StudentClassificationModel() { Id = 1, Name = "Sophomore" },
                new StudentClassificationModel() { Id = 2, Name = "Junior" },
                new StudentClassificationModel() { Id = 3, Name = "Senior" }
            };
        }
    }
}
