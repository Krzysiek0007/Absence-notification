using AbsenceNotificationWebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AbsenceNotificationWebApplication.Controllers
{
    public class RoleController : Controller
    {
        private ApplicationDbContext context;

        public RoleController()
        {
            context = new ApplicationDbContext();
        }

        // GET: Role
        [AuthorizeUser(Permissions = "MANAGE_ROLES")]
        public ActionResult Index()
        {
            ViewBag.adminMenu = "No";
            var user = User.Identity;

            if (user.IsAuthenticated)
            {
                if (Authorize.AuthorizeUser.HasPermissions("VIEW_ADMIN_MENU", user))
                {
                    ViewBag.adminMenu = "Yes";
                }

                if (!Authorize.AuthorizeUser.HasPermissions("VIEW_ADMIN_MENU", user))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var Roles = context.Roles.ToList();
            return View(Roles);
        }

        /// <summary>
        /// Create  a New role
        /// </summary>
        /// <returns></returns>
        [AuthorizeUser(Permissions = "MANAGE_ROLES")]
        public ActionResult Create()
        {
            ViewBag.adminMenu = "No";
            var user = User.Identity;

            if (user.IsAuthenticated)
            {
                if (Authorize.AuthorizeUser.HasPermissions("VIEW_ADMIN_MENU", user))
                {
                    ViewBag.adminMenu = "Yes";
                }

                if (!Authorize.AuthorizeUser.HasPermissions("VIEW_ADMIN_MENU", user))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var Role = new IdentityRole();
            return View(Role);
        }

        /// <summary>
        /// Create a New Role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeUser(Permissions = "MANAGE_ROLES")]
        public ActionResult Create(IdentityRole Role)
        {
            var user = User.Identity;

            if (User.Identity.IsAuthenticated)
            {
                if (!Authorize.AuthorizeUser.HasPermissions("VIEW_ADMIN_MENU", user))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            context.Roles.Add(Role);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
