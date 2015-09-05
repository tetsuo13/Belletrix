using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.Model
{
    public class PromoModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(256)]
        public string Description { get; set; }

        public int CreatedById { get; set; }
        public string CreatedByFirstName { get; set; }
        public string CreatedByLastName { get; set; }

        public DateTime Created { get; set; }

        [Required]
        [Display(Name = "Unique Code")]
        [MaxLength(32)]
        public string Code { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<StudentPromoLog> Logs { get; set; }
    }
}
