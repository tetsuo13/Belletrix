using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IActivityLogPersonService
    {
        Task<int> CreatePerson(ActivityLogPersonCreateViewModel createModel);
        Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople();
        Task SaveChanges();

        /// <summary>
        /// Find an existing person by their unique ID.
        /// </summary>
        /// <param name="id">Unique ID.</param>
        /// <returns>
        /// Person detail or <see typename="null"/> if no person is found by
        /// that ID.
        /// </returns>
        Task<ActivityLogPersonModel> FindPersonById(int id);
    }
}
