﻿using Belletrix.Core;
using Belletrix.DAL;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Belletrix.Domain
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository StudentRepository;
        private readonly IStudentPromoRepository StudentPromoRepository;
        private readonly IStudyAbroadRepository StudyAbroadRepository;
        private readonly IEventLogService EventLogService;
        private readonly IStudentNoteRepository StudentNoteRepository;
        private readonly IEventLogRepository EventLogRepository;

        public StudentService(IStudentRepository studentRepository, IStudentPromoRepository studentPromoRepository,
            IStudyAbroadRepository studyAbroadRepository, IEventLogService eventLogService,
            IStudentNoteRepository studentNoteRepository, IEventLogRepository eventLogRepository)
        {
            StudentRepository = studentRepository;
            StudentPromoRepository = studentPromoRepository;
            StudyAbroadRepository = studyAbroadRepository;
            EventLogService = eventLogService;
            StudentNoteRepository = studentNoteRepository;
            EventLogRepository = eventLogRepository;
        }

        public async Task<IEnumerable<StudentModel>> GetStudents(int? id = null)
        {
            return await StudentRepository.GetStudents(id);
        }

        public async Task<StudentModel> GetStudent(int id)
        {
            return await StudentRepository.GetStudent(id);
        }

        public async Task<IEnumerable<StudentModel>> Search(StudentSearchViewModel search)
        {
            bool filterByGraduatingYears = search.SelectedGraduatingYears != null && search.SelectedGraduatingYears.Any();
            bool filterByMajors = search.SelectedMajors != null && search.SelectedMajors.Any();
            bool filterByCountries = search.SelectedCountries != null && search.SelectedCountries.Any();

            if (!filterByGraduatingYears && !filterByMajors && !filterByCountries)
            {
                return Enumerable.Empty<StudentModel>();
            }

            IEnumerable<StudentModel> students = await StudentRepository.GetStudents();

            if (filterByGraduatingYears)
            {
                students = students.Where(x =>
                {
                    return x.GraduatingYear.HasValue &&
                        search.SelectedGraduatingYears.Any(y => y == x.GraduatingYear.Value);
                });
            }

            if (filterByMajors)
            {
                students = students.Where(x =>
                {
                    return x.SelectedMajors != null &&
                        x.SelectedMajors.Count() > 0 &&
                        x.SelectedMajors.Intersect(search.SelectedMajors).Count() > 0;
                });
            }

            if (filterByCountries)
            {
                IEnumerable<StudyAbroadViewModel> studyAbroad = await StudyAbroadRepository.GetAll();

                students = studyAbroad
                    .Where(x => search.SelectedCountries.Any(y => y == x.CountryId))
                    .Select(x => x.Student);

                //students = students
                //    .Where(x => x.StudyAbroadCountry.Count<int>() > 0)
                //    .Where(x => x.StudyAbroadCountry.Intersect(search.SelectedCountries).Count<int>() > 0);
            }

            return students;
        }

        public async Task<IEnumerable<StudentModel>> SearchByFullName(string firstName, string lastName)
        {
            return (await GetStudents()).Where(x =>
            {
                return string.Equals(x.FirstName, firstName, StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(x.LastName, lastName, StringComparison.InvariantCultureIgnoreCase);
            });
        }

        public async Task<IEnumerable<StudentModel>> FromPromo(int promoId)
        {
            return (await GetStudents()).Where(x => x.PromoIds != null && x.PromoIds.Any(y => y == promoId));
        }

        public async Task<IEnumerable<CountryModel>> GetCountries()
        {
            return await StudentRepository.GetCountries();
        }

        public async Task<IEnumerable<CountryModel>> GetRegions()
        {
            return await StudentRepository.GetRegions();
        }

        public async Task<IEnumerable<LanguageModel>> GetLanguages()
        {
            return await StudentRepository.GetLanguages();
        }

        public async Task<IEnumerable<MajorsModel>> GetMajors()
        {
            return await StudentRepository.GetMajors();
        }

        public async Task<IEnumerable<MinorsModel>> GetMinors()
        {
            return await StudentRepository.GetMinors();
        }

        public IEnumerable<StudentClassificationModel> GetClassifications()
        {
            return new List<StudentClassificationModel>()
            {
                new StudentClassificationModel() { Id = 0, Name = "Freshman" },
                new StudentClassificationModel() { Id = 1, Name = "Sophomore" },
                new StudentClassificationModel() { Id = 2, Name = "Junior" },
                new StudentClassificationModel() { Id = 3, Name = "Senior" }
            };
        }

        /// <summary>
        /// Value used to represent the "Please Select" option for the year
        /// select list.
        /// </summary>
        /// <returns>Option value.</returns>
        public int GetStudyAbroadWishlistCatchAllYearValue()
        {
            return 1;
        }

        public IEnumerable<object> GetStudyAbroadWishlistPeriods()
        {
            ICollection<object> periods = new List<object>();

            foreach (StudentStudyAbroadPeriods value in Enum.GetValues(typeof(StudentStudyAbroadPeriods)))
            {
                periods.Add(new
                {
                    Id = (int)value,
                    Name = value
                });
            }

            return periods;
        }

        public IEnumerable<object> GetStudyAbroadWishlistPeriodsWithCatchAll()
        {
            IEnumerable<object> catchAll = new List<object>()
            {
                new { Id = 99, Name = "Any Semester" }
            };

            IEnumerable<object> periods = catchAll;

            return periods.Concat(GetStudyAbroadWishlistPeriods());
        }

        public async Task<IEnumerable<ProgramModel>> GetPrograms()
        {
            return await StudentRepository.GetPrograms();
        }

        public async Task<IEnumerable<ProgramTypeModel>> GetProgramTypes()
        {
            return await StudentRepository.GetProgramTypes();
        }

        public async Task InsertStudent(StudentModel model, UserModel user, string remoteIp)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int studentId = await StudentRepository.InsertStudent(model);

                model.Id = studentId;

                await StudentPromoRepository.Save(model.Id, model.PromoIds);
                await EventLogService.AddStudentEvent(user.Id, studentId, EventLogTypes.AddStudent, remoteIp);

                scope.Complete();
            }
        }

        public async Task InsertStudent(StudentPromoModel model, int? userId, Guid promoToken, string remoteIp)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int studentId = await StudentRepository.InsertStudent(model);
                model.Id = studentId;

                await StudentPromoRepository.Save(model.Id, promoToken);

                if (userId.HasValue)
                {
                    await EventLogService.AddStudentEvent(userId.Value, model.Id, EventLogTypes.EditStudent,
                        remoteIp);
                }
                else
                {
                    await EventLogService.AddStudentEvent(model.Id, EventLogTypes.EditStudent, remoteIp);
                }

                scope.Complete();
            }
        }

        public async Task UpdateStudent(StudentModel model, UserModel user, string remoteIp)
        {
            if (model.InitialMeeting.HasValue)
            {
                model.InitialMeeting = model.InitialMeeting.Value.ToUniversalTime();
            }

            if (!string.IsNullOrEmpty(model.MiddleName))
            {
                model.MiddleName = model.MiddleName.CapitalizeFirstLetter();
            }

            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                model.PhoneNumber = model.PhoneNumber.Trim();
            }

            if (!string.IsNullOrEmpty(model.StudentId))
            {
                model.StudentId = model.StudentId.Trim();
            }

            if (model.DateOfBirth.HasValue)
            {
                model.DateOfBirth = model.DateOfBirth.Value.ToUniversalTime();
            }

            model.CampusEmail = Formatter.SanitizeEmail(model.CampusEmail);
            model.AlternateEmail = Formatter.SanitizeEmail(model.AlternateEmail);

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await StudentRepository.UpdateStudent(model);
                await StudentPromoRepository.Save(model.Id, model.PromoIds);
                await EventLogService.AddStudentEvent(user.Id, model.Id, EventLogTypes.EditStudent, remoteIp);

                scope.Complete();
            }
        }

        public async Task<GenericResult> Delete(int id)
        {
            GenericResult result = new GenericResult();

            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (!await StudentRepository.DeleteStudyAbroadDestinations(id))
                    {
                        result.Message = "Error deleting study abroad destinations";
                    }
                    else if (!await StudentNoteRepository.Delete(id))
                    {
                        result.Message = "Error deleting student notes";
                    }
                    else if (!await EventLogRepository.DeleteStudent(id))
                    {
                        result.Message = "Error deleting student event logs";
                    }
                    else if (!await StudentPromoRepository.Delete(id))
                    {
                        result.Message = "Error deleting details from promo";
                    }
                    else if (!await StudyAbroadRepository.DeleteStudent(id))
                    {
                        result.Message = "Error deleting experiences";
                    }
                    else if (!await StudentRepository.DeleteMatriculations(id))
                    {
                        result.Message = "Error deleting matriculations";
                    }
                    else if (!await StudentRepository.DeleteLanguages(id))
                    {
                        result.Message = "Error deleting fluent/desired/studied languages";
                    }
                    else if (!await StudentRepository.Delete(id))
                    {
                        result.Message = "Error deleting student record";
                    }
                    else
                    {
                        result.Result = true;
                        scope.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                result.Result = false;
                result.Message = "Error completing transaction scope: " + e.Message;
            }

            return result;
        }
    }
}
