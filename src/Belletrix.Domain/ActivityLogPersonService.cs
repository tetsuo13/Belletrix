using Belletrix.DAL;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class ActivityLogPersonService : IActivityLogPersonService
    {
        private IActivityLogPersonRepository repository;

        public ActivityLogPersonService(IActivityLogPersonRepository repository)
        {
            this.repository = repository;
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

            return await repository.CreatePerson(model);
        }

        public async Task SaveChanges()
        {
            await repository.SaveChanges();
        }
    }
}
