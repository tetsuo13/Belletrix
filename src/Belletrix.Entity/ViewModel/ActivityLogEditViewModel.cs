using Belletrix.Entity.Model;
using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.ViewModel
{
    public class ActivityLogEditViewModel : ActivityLogCreateViewModel
    {
        [Required]
        public int Id { get; set; }

        public static explicit operator ActivityLogEditViewModel(ActivityLogModel a)
        {
            return new ActivityLogEditViewModel()
            {
                Id = a.Id,
                Title = a.Title,
                Title2 = a.Title2,
                Title3 = a.Title3,
                Organizers = a.Organizers,
                Location = a.Location,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                OnCampus = a.OnCampus,
                WebSite = a.WebSite,
                Notes = a.Notes
            };
        }
    }
}
