using Bennett.AbroadAdvisor.Models;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;

namespace Bennett.AbroadAdvisor.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            //string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;
            //Dictionary<string, string> majors = new Dictionary<string, string>();

            //using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            //{
            //    string sql = "SELECT * FROM programs";

            //    connection.Open();

            //    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            //    {
            //        NpgsqlDataReader reader = command.ExecuteReader();

            //        while (reader.Read())
            //        {
            //            majors.Add(reader.GetString(reader.GetOrdinal("Name")),
            //                reader["Abbreviation"].ToString());
            //        }
            //    }
            //}

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid && model.IsValid(model.UserName, model.Password))
            {
                UserModel.UpdateLastLogin(model.UserName);
                FormsAuthentication.SetAuthCookie(model.UserName, true);
                Session["User"] = UserModel.GetUser(model.UserName);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login credentials");
            return View(model);
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }
    }
}
