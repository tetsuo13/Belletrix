using Belletrix.DAL;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class ActivityLogPersonService : IActivityLogPersonService
    {
        private readonly IActivityLogPersonRepository Repository;

        public ActivityLogPersonService(IActivityLogPersonRepository repository)
        {
            Repository = repository;
        }

        public async Task<int> CreatePerson(ActivityLogPersonCreateViewModel createModel)
        {
            ActivityLogPersonModel model = new ActivityLogPersonModel()
            {
                FullName = createModel.FullName,
                Email = createModel.Email,
                Description = createModel.Description,
                PhoneNumber = createModel.PhoneNumber,
                SessionId = createModel.SessionId
            };

            return await Repository.CreatePerson(model);
        }

        // TODO: Should this be only people who don't have a temporary session ID value?
        // A session value means this person is in the process of being added
        // to a new activity log.
        public async Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople()
        {
            return await Repository.FindAllPeople();
        }

        public async Task SaveChanges()
        {
            await Repository.SaveChanges();
        }

        /// <summary>
        /// Find an existing person by their unique ID.
        /// </summary>
        /// <param name="id">Unique ID.</param>
        /// <returns>
        /// Person detail or <see typename="null"/> if no person is found by
        /// that ID.
        /// </returns>
        public async Task<ActivityLogPersonModel> FindPersonById(int id)
        {
            return await Repository.FindPersonById(id);
        }
    }
}
