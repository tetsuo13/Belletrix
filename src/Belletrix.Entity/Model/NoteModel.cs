using System;
using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.Model
{
    public class NoteModel
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int CreatedById { get; set; }
        public string CreatedByFirstName { get; set; }
        public string CreatedByLastName { get; set; }

        public DateTime EntryDate { get; set; }

        [Required]
        [StringLength(16384)]
        public string Note { get; set; }
    }
}
