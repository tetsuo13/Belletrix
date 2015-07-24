using Belletrix.Domain;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Belletrix.Controllers
{
    [Authorize]
    public class ActivityLogPersonController : Controller
    {
        public static string ActivePageName = "activitylogperson";

        private readonly IActivityLogPersonService Service;

        public ActivityLogPersonController(IActivityLogPersonService service)
        {
            Service = service;
            ViewBag.ActivePage = ActivePageName;
        }

        public async Task<PartialViewResult> AddPerson(Guid guid)
        {
            IEnumerable<SelectListItem> types = from ActivityLogParticipantTypes t
                                                in Enum.GetValues(typeof(ActivityLogParticipantTypes))
                                                select new SelectListItem()
                                                {
                                                    Value = ((int)t).ToString(),
                                                    Text = t.ToString()
                                                };

            ViewBag.TypesSelect = new SelectList(types, "Value", "Text");

            IEnumerable<ActivityLogPersonModel> availablePeople = await Service.FindAllPeople();
            IEnumerable<ActivityLogParticipantModel> participantsInSession = ParticipantsInSession(guid);

            // Remove people from the available list who've already been added
            // in this session.
            if (participantsInSession != null && participantsInSession.Any())
            {
                availablePeople = availablePeople.Where(x => !participantsInSession.Any(y => y.Person.Id == x.Id));
            }

            ViewBag.PeopleSelect = new SelectList(availablePeople, "Id", "FullName");

            ActivityLogPersonCreateViewModel model = new ActivityLogPersonCreateViewModel()
            {
                SessionId = guid
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddPerson(ActivityLogPersonCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int id = await Service.CreatePerson(model);
                    await Service.SaveChanges();

                    ActivityLogParticipantModel participant = new ActivityLogParticipantModel()
                    {
                        Person = new ActivityLogPersonModel()
                        {
                            Id = id,
                            FullName = model.FullName,
                            PhoneNumber = model.PhoneNumber,
                            Description = model.Description,
                            Email = model.Email,
                            SessionId = model.SessionId
                        },
                        Type = (ActivityLogParticipantTypes)model.Type
                    };

                    AddParticipantToSession(model.SessionId, participant);

                    return Json(new
                        {
                            Success = true,
                            Message = String.Empty,
                            Id = id
                        });
                }
                catch (Exception e)
                {
                    MvcApplication.LogException(e);
                    return Json(new
                        {
                            Success = false,
                            Message = "There was an error saving. It has been logged for later review.",
                            Id = 0
                        });
                }
            }

            return Json(new
                {
                    Success = false,
                    Message = "Invalid form",
                    Id = 0
                });
        }

        [HttpPost]
        public async Task<JsonResult> AddPersonId(int id, int type, Guid sessionid)
        {
            ActivityLogPersonModel person = await Service.FindPersonById(id);

            if (person == null)
            {
                return Json(new
                    {
                        Success = false,
                        Message = "Person not found"
                    });
            }

            ActivityLogParticipantModel participant = new ActivityLogParticipantModel()
            {
                Person = person,
                Type = (ActivityLogParticipantTypes)type
            };

            AddParticipantToSession(sessionid, participant);

            return Json(new
                {
                    Success = true,
                    Message = String.Empty
                });
        }

        private void AddParticipantToSession(Guid sessionId, ActivityLogParticipantModel participant)
        {
            if (Session[ActivityLogService.SessionName] == null)
            {
                Session[ActivityLogService.SessionName] = new Dictionary<Guid, List<ActivityLogParticipantModel>>();
            }

            if (!(Session[ActivityLogService.SessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>).ContainsKey(sessionId))
            {
                (Session[ActivityLogService.SessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId] = new List<ActivityLogParticipantModel>();
            }

            (Session[ActivityLogService.SessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId].Add(participant);
        }

        private IEnumerable<ActivityLogParticipantModel> ParticipantsInSession(Guid sessionId)
        {
            if (Session[ActivityLogService.SessionName] != null &&
                (Session[ActivityLogService.SessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>).ContainsKey(sessionId))
            {
                return (Session[ActivityLogService.SessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId];
            }

            return null;
        }
    }
}