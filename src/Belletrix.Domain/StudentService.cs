﻿using Belletrix.DAL;
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

        public StudentService(IStudentRepository studentRepository)
        {
            StudentRepository = studentRepository;
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
                IEnumerable<StudyAbroadModel> studyAbroad = StudyAbroadModel.GetAll();

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
    }
}
