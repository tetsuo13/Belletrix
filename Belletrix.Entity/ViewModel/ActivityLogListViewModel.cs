using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Belletrix.Entity.ViewModel
{
    public class ActivityLogListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        public string Title2 { get; set; }

        public string Title3 { get; set; }

        [Display(Name = "Starting")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Types")]
        public IEnumerable<int> Types { get; set; }

        [Display(Name = "Organizers")]
        public string Organizers { get; set; }
    }
}
