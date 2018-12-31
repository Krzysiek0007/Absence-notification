using AbsenceNotificationWebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AbsenceNotificationWebApplication.Controllers
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        // Custom property
        public string Permissions { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }
                        
            var privilegeLevels = GetUserRights(httpContext.User.Identity.GetUserId());

            foreach (var acceessLevel in this.Permissions.Split(',').ToList())
            {
                if (privilegeLevels.Contains(acceessLevel))
                {
                    return true;
                }
            }
            return false;
        }

        private List<string> GetUserRights(string userId)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var userRole = UserManager.GetRoles(userId).Single();
            string roleId = roleManager.FindByName(userRole).Id;
            return context.RolePermissions.Where(n => n.RoleId == roleId).Select(n => n.Permission.Name).ToList();
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Error",
                                action = "Unauthorised"
                            })
                        );
        }
    }
}