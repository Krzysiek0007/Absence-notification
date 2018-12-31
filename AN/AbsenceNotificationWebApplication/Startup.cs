using AbsenceNotificationWebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Data.Entity.Migrations;

[assembly: OwinStartupAttribute(typeof(AbsenceNotificationWebApplication.Startup))]
namespace AbsenceNotificationWebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesAndUsers();
            createAbsenceCategories();
        }

        // In this method we will create absence categories   
        private void createAbsenceCategories()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            context.AbsenceCategories.AddOrUpdate(
                new AbsenceCategory() { Id = 1, CategoryName = "work at home" },
                new AbsenceCategory() { Id = 2, CategoryName = "personal matter" },
                new AbsenceCategory() { Id = 3, CategoryName = "doctor's visit" }
                );
            context.SaveChanges();
        }

        // In this method we will create default User roles and Admin user cfor login   
        private void createRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                var user = new ApplicationUser();
                user.UserName = "admin@company.com";
                user.Email = "admin@company.com";

                string userPWD = "admin@company";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("Manager"))
            {
                var role = new IdentityRole();
                role.Name = "Manager";
                roleManager.Create(role);

            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("Employee"))
            {
                var role = new IdentityRole();
                role.Name = "Employee";
                roleManager.Create(role);

            }
        }
    }
}
