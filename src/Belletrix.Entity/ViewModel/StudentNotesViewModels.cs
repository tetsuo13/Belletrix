using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.ViewModel
{
    public class AddStudentNoteViewModel
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        [StringLength(16384)]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }
    }
}
