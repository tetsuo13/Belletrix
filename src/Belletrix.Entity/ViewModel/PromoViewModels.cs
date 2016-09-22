using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.ViewModel
{
    public class PromoViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string CreatedByFirstName { get; set; }
        public string CreatedByLastName { get; set; }
        public DateTime Created { get; set; }
        public bool Active { get; set; }
        public Guid PublicToken { get; set; }
        public int TotalStudents { get; set; }
        public IEnumerable<StudentModel> Students { get; set; }
        public bool CanDelete { get; set; }
    }

    public class PromoCreateViewModel
    {
        [Required]
        [Display(Name = "Description")]
        [StringLength(256)]
        public string Description { get; set; }
    }

    /// <summary>
    /// Used as a select list for student add/edit to associate them without
    /// having to use the promo portal.
    /// </summary>
    public class PromoSourceViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
