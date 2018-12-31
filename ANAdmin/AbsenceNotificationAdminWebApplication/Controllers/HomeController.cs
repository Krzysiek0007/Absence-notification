using AbsenceNotificationAdminWebApplication.Authorize;
using AbsenceNotificationAdminWebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AbsenceNotificationAdminWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (!AuthorizeUser.HasPermissions("LIST_ALL_NOTIFICATIONS", User.Identity))
            {
                ViewBag.Message = "In order to see list of notifications please log in as manager.";
            }
            return View();
        }

        public ActionResult About()
        {
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