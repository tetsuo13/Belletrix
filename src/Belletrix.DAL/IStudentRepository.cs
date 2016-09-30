using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IStudentRepository
    {
        Task<IEnumerable<StudentModel>> GetStudents(int? id = null);
        Task<StudentModel> GetStudent(int id);

        Task<IEnumerable<CountryModel>> GetCountries();
        Task<IEnumerable<CountryModel>> GetRegions();

        Task<IEnumerable<LanguageModel>> GetLanguages();

        Task<IEnumerable<MajorsModel>> GetMajors();
        Task<IEnumerable<MinorsModel>> GetMinors();

        Task<IEnumerable<ProgramModel>> GetPrograms();
        Task<IEnumerable<ProgramTypeModel>> GetProgramTypes();

        Task<int> InsertStudent(object model);
        Task UpdateStudent(StudentModel model);
        Task<bool> Delete(int id);

        Task<bool> DeleteStudyAbroadDestinations(int id);
        Task<bool> DeleteMatriculations(int id);
        Task<bool> DeleteLanguages(int id);
    }
}
