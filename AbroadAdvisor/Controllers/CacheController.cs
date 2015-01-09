using Bennett.AbroadAdvisor.Core;
using Bennett.AbroadAdvisor.Models;
using System;
using System.Web;
using System.Web.Http;

namespace Bennett.AbroadAdvisor.Controllers
{
    [AllowAnonymous]
    public class CacheController : ApiController
    {
        [HttpGet]
        public string Get()
        {
            DateTime startDate = DateTime.Now;

            CountryModel.GetCountries();
            LanguageModel.GetLanguages();
            StudentClassificationModel.GetClassifications();
            MajorsModel.GetMajors();
            MinorsModel.GetMinors();
            CountryModel.GetRegions();
            StudentStudyAbroadWishlistModel.GetPeriods();
            ProgramModel.GetPrograms();
            ProgramTypeModel.GetProgramTypes();
            StudentModel.GetStudents();
            StudyAbroadModel.GetAll();
            PromoModel.GetPromos(true);
            StudentPromoLog.Get();

            TimeSpan span = DateTime.Now - startDate;

            return String.Format("Cached {0} objects in {1} seconds",
                HttpRuntime.Cache.Count, span.TotalSeconds);
        }

        [HttpGet]
        public string Refresh()
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            cacheProvider.Clear();
            return "Cleared. " + Get();
        }
    }
}
