using AbsenceNotificationWebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace AbsenceNotificationWebApplication.Authorize
{
    public static class AuthorizeUser
    {
        public static bool HasPermissions(string permissions, IIdentity user)
        {
            if (!user.IsAuthenticated)
            {
                return false;
            }

            var privilegeLevels = GetUserRights(user.GetUserId());

            foreach (var acceessLevel in permissions.Split(',').ToList())
            {
                if (privilegeLevels.Contains(acceessLevel))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasPermissions(string permissions, string userId)
        {
            var privilegeLevels = GetUserRights(userId);

            foreach (var acceessLevel in permissions.Split(',').ToList())
            {
                if (privilegeLevels.Contains(acceessLevel))
                {
                    return true;
                }
            }
            return false;
        }

        private static List<string> GetUserRights(string userId)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var userRole = UserManager.GetRoles(userId).Single();
            string roleId = roleManager.FindByName(userRole).Id;
            return context.RolePermissions.Where(n => n.RoleId == roleId).Select(n => n.Permission.Name).ToList();
        }
    }
}