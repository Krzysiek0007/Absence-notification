using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace AbsenceNotificationWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;

                ViewBag.adminMenu = "No";

                if (Authorize.AuthorizeUser.HasPermissions("VIEW_ADMIN_MENU",user))
                {
                    ViewBag.adminMenu = "Yes";
                }

                ViewBag.displayAbsenceForm = "No";

                if (Authorize.AuthorizeUser.HasPermissions("VIEW_USER_MENU",user))
                {
                    ViewBag.displayAbsenceForm = "Yes";
                }

                return View();
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.adminMenu = "No";
            var user = User.Identity;

            if (Authorize.AuthorizeUser.HasPermissions("VIEW_ADMIN_MENU", user))
            {
                ViewBag.adminMenu = "Yes";
            }

            ViewBag.Message = "Absence notification application.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Administrator.";

            return View();
        }
    }
}