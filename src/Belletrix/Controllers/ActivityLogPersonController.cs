﻿using Belletrix.Domain;
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

        /// <summary>
        /// Start a new collection of participants for this add/edit activity
        /// session.
        /// </summary>
        /// <param name="guid">Current session ID.</param>
        public void StartSession(Guid guid)
        {
            Service.StartSession(Session, guid);
        }

        /// <summary>
        /// Add all participants for a given activity log into the session.
        /// This should only be called when editing an existing activity.
        /// </summary>
        /// <param name="guid">Current session ID.</param>
        /// <param name="activityId">Existing activity ID.</param>
        /// <returns>Nothing</returns>
        public async Task PopuplateSession(Guid guid, int activityId)
        {
            await Service.PopulateSession(Session, guid, activityId);
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
            IEnumerable<ActivityLogParticipantModel> participantsInSession = Service.ParticipantsInSession(Session,
                guid);

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

                    Service.AddParticipantToSession(Session, model.SessionId, participant);

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

        [HttpDelete]
        public async Task<JsonResult> RemovePersonId(int id, Guid sessionId)
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
                Person = person
            };

            Service.RemoveParticipantFromSession(Session, sessionId, participant);

            return Json(new
            {
                Success = true,
                Message = String.Empty
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

            Service.AddParticipantToSession(Session, sessionid, participant);

            return Json(new
                {
                    Success = true,
                    Message = String.Empty
                });
        }

        public async Task<JsonResult> participantsInSession(Guid guid)
        {
            return Json(Service.ParticipantsInSession(Session, guid), JsonRequestBehavior.AllowGet);
        }
    }
}