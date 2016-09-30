using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentModel>> GetStudents(int? id = null);
        Task<StudentModel> GetStudent(int id);
        Task<IEnumerable<StudentModel>> FromPromo(int promoId);

        Task<IEnumerable<StudentModel>> Search(StudentSearchViewModel search);
        Task<IEnumerable<StudentModel>> SearchByFullName(string firstName, string lastName);

        Task<IEnumerable<CountryModel>> GetCountries();
        Task<IEnumerable<CountryModel>> GetRegions();

        Task<IEnumerable<LanguageModel>> GetLanguages();

        Task<IEnumerable<MajorsModel>> GetMajors();
        Task<IEnumerable<MinorsModel>> GetMinors();

        IEnumerable<StudentClassificationModel> GetClassifications();

        /// <summary>
        /// Value used to represent the "Please Select" option for the year
        /// select list.
        /// </summary>
        /// <returns>Option value.</returns>
        int GetStudyAbroadWishlistCatchAllYearValue();
        IEnumerable<object> GetStudyAbroadWishlistPeriods();
        IEnumerable<object> GetStudyAbroadWishlistPeriodsWithCatchAll();

        Task<IEnumerable<ProgramModel>> GetPrograms();
        Task<IEnumerable<ProgramTypeModel>> GetProgramTypes();

        Task InsertStudent(StudentModel model, UserModel user, string remoteIp);
        Task InsertStudent(StudentPromoModel model, int? userId, Guid promoToken, string remoteIp);
        Task UpdateStudent(StudentModel model, UserModel user, string remoteIp);

        Task<GenericResult> Delete(int id);
    }
}
