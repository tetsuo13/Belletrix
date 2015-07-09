using Belletrix.Domain;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Belletrix.Controllers.Api
{
    [Authorize]
    public class ActivityLogController : ApiController
    {
        [HttpPost]
        public async Task<object> AddPerson(ActivityLogPersonCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var service = new ActivityLogService();
                    //int id = await service.CreatePerson(model);
                    int id = new Random().Next();
                    return new { Success = true, Message = String.Empty, Id = id };
                }
                catch (Exception e)
                {
                    MvcApplication.LogException(e);
                    return new
                    {
                        Success = false,
                        Message = "There was an error saving. It has been logged for later review.",
                        Id = 0
                    };
                }
            }

            return new
            {
                Success = false,
                Message = "Invalid form",
                Id = 0
            };
        }
    }
}
