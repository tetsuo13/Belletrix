using Bennett.AbroadAdvisor.Models;
using System.Web.Mvc;

namespace Bennett.AbroadAdvisor.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        public ActionResult List()
        {
            ViewBag.ActivePage = "students";
            return View(StudentModel.GetStudents());
        }

        public ActionResult Add()
        {
            ViewBag.ActivePage = "students";
            ViewBag.Countries = new SelectList(CountryModel.GetCountries(), "Id", "Name");
            ViewBag.DormHalls = new SelectList(DormModel.GetDorms(), "Id", "HallName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(StudentModel model)
        {
            if (ModelState.IsValid)
            {
                StudentModel.Create(model, (Session["User"] as UserModel).Id);
                return RedirectToAction("List");
            }

            ModelState.AddModelError("", "There were errors adding the student.");
            return View(model);
        }
    }
}
