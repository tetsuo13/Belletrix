using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IActivityLogPersonService
    {
        Task<int> CreatePerson(ActivityLogPersonCreateViewModel createModel);
        Task SaveChanges();
    }
}
