using Bennett.AbroadAdvisor.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace Bennett.AbroadAdvisor.Controllers
{
    public class CacheController : ApiController
    {
        // GET api/cache
        public IEnumerable<string> Get()
        {
            CountryModel.GetCountries();
            LanguageModel.GetLanguages();
            StudentClassificationModel.GetClassifications();
            MajorsModel.GetMajors();
            MinorsModel.GetMinors();
            CountryModel.GetRegions();
            StudentStudyAbroadWishlistModel.GetPeriods();
            ProgramModel.GetPrograms();
            ProgramTypeModel.GetProgramTypes();

            StudentModel.GetStudents(null);
            StudyAbroadModel.GetAll();

            return new string[] { "foo" };
        }
    }
}
