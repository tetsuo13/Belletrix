using Belletrix.DAL;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository StudentRepository;
        private readonly IStudentPromoRepository StudentPromoRepository;
        private readonly IStudyAbroadRepository StudyAbroadRepository;
        private readonly IEventLogRepository EventLogRepository;

        public StudentService(IStudentRepository studentRepository, IStudentPromoRepository studentPromoRepository,
            IStudyAbroadRepository studyAbroadRepository, IEventLogRepository eventLogRepository)
        {
            StudentRepository = studentRepository;
            StudentPromoRepository = studentPromoRepository;
            StudyAbroadRepository = studyAbroadRepository;
            EventLogRepository = eventLogRepository;
        }

        private IEnumerable<StudentModel> PopulatePromoLogs(IEnumerable<StudentModel> students)
        {
            if (students == null || !students.Any())
            {
                return students;
            }

            List<StudentModel> updatedStudents = new List<StudentModel>(students);

            updatedStudents.ForEach(async x => x.PromoIds = await StudentPromoRepository.GetPromoIdsForStudent(x.Id));

            return updatedStudents;
        }

        public async Task<IEnumerable<StudentModel>> GetStudents(int? id = null)
        {
            IEnumerable<StudentModel> students = await StudentRepository.GetStudents(id);
            return PopulatePromoLogs(students);
        }

        public async Task<StudentModel> GetStudent(int id)
        {
            StudentModel student = await StudentRepository.GetStudent(id);

            if (student != null)
            {
                ICollection<StudentModel> x = new List<StudentModel>();
                x.Add(student);

                return PopulatePromoLogs(x).First();
            }

            return null;
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
                students = students
                    .Where(x => x.GraduatingYear.HasValue)
                    .Where(x => search.SelectedGraduatingYears.Any(y => y == x.GraduatingYear.Value));
            }

            if (filterByMajors)
            {
                students = students
                    .Where(x => x.SelectedMajors.Count<int>() > 0)
                    .Where(x => x.SelectedMajors.Intersect(search.SelectedMajors).Count<int>() > 0);
            }

            if (filterByCountries)
            {
                IEnumerable<StudyAbroadModel> studyAbroad = await StudyAbroadRepository.GetAll();

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
                return String.Equals(x.FirstName, firstName, StringComparison.InvariantCultureIgnoreCase) &&
                    String.Equals(x.LastName, lastName, StringComparison.InvariantCultureIgnoreCase);
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

        public async Task InsertStudent(StudentModel model, UserModel user)
        {
            int studentId = await StudentRepository.InsertStudent(model);

            model.Id = studentId;

            EventLogModel eventLog = new EventLogModel()
            {
                Student = model,
                ModifiedById = user.Id,
                ModifiedByFirstName = user.FirstName,
                ModifiedByLastName = user.LastName
            };

            await EventLogRepository.AddStudentEvent(eventLog, user.Id, studentId, EventLogTypes.AddStudent);

            StudentRepository.SaveChanges();
            EventLogRepository.SaveChanges();
        }

        public async Task UpdateStudent(StudentModel model, UserModel user)
        {
            await StudentRepository.UpdateStudent(model);

            EventLogModel eventLog = new EventLogModel()
            {
                Student = model,
                ModifiedById = user.Id,
                ModifiedByFirstName = user.FirstName,
                ModifiedByLastName = user.LastName
            };

            await EventLogRepository.AddStudentEvent(eventLog, user.Id, model.Id, EventLogTypes.EditStudent);

            StudentRepository.SaveChanges();
            EventLogRepository.SaveChanges();
        }
    }
}
