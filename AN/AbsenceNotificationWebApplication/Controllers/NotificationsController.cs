using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using AbsenceNotificationWebApplication.Authorize;
using AbsenceNotificationWebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AbsenceNotificationWebApplication.Controllers
{
    public class NotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Notifications
        [AuthorizeUser(Permissions="READ_NOTIFICATION")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var users = db.Users.ToList();
            List<NotificationViewModel> model = new List<NotificationViewModel>();

            foreach (var item in db.Notifications)
            {
                model.Add(new NotificationViewModel
                {
                    Id = item.Id,
                    SenderId = item.SenderId,
                    SenderName = users.Where(m => m.Id == item.SenderId).SingleOrDefault().UserName,
                    RecipientId = item.RecipientId,
                    RecipientName = users.Where(m => m.Id == item.RecipientId).SingleOrDefault().UserName,
                    AbsenceCategoryId = item.AbsenceCategoryId,
                    AbsenceCategoryName = db.AbsenceCategories.Where(m => m.Id == item.AbsenceCategoryId).SingleOrDefault().CategoryName,
                    Description = item.Description,
                    Date = item.Date,
                    DateFrom = item.DateFrom,
                    DateTo = item.DateTo
                });
            }

            return View(model.Where(m => m.SenderId == userId).ToList());
        }

        // GET: Notifications/Details/5
        [AuthorizeUser(Permissions="READ_NOTIFICATION")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var users = db.Users.ToList();
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }

            NotificationViewModel vnotification = new NotificationViewModel
            {
                Id = notification.Id,
                SenderId = notification.SenderId,
                SenderName = users.Where(m => m.Id == notification.SenderId).SingleOrDefault().UserName,
                RecipientId = notification.RecipientId,
                RecipientName = users.Where(m => m.Id == notification.RecipientId).SingleOrDefault().UserName,
                AbsenceCategoryId = notification.AbsenceCategoryId,
                AbsenceCategoryName = db.AbsenceCategories.Where(m => m.Id == notification.AbsenceCategoryId).SingleOrDefault().CategoryName,
                Description = notification.Description,
                Date = notification.Date,
                DateFrom = notification.DateFrom,
                DateTo = notification.DateTo
            };

            return View(vnotification);
        }

        // GET: Notifications/Create
        [AuthorizeUser(Permissions="CREATE_NOTIFICATION")]
        public ActionResult Create()
        {
            var users = GetUsersHavingPermission("RECEIVE_NOTIFICATION");

            ViewBag.recipientId = new SelectList(users, "Id", "UserName");
            ViewBag.absenceCategoryId = new SelectList(db.AbsenceCategories.ToList(), "Id", "CategoryName");
            return View();
        }

        private List<ApplicationUser> GetUsersHavingPermission(string permission)
        {
            var roleIds = db.Permissions.Where(p => p.Name == permission).Single()
                .RolePermissions.Select(n => n.RoleId).ToList();

            List<ApplicationUser> users = new List<ApplicationUser>();

            foreach (var roleId in roleIds)
            {
                users.AddRange(db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(roleId)).ToList()); 
            }
            return users;
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Permissions="CREATE_NOTIFICATION")]
        public ActionResult Create([Bind(Include = "Id,SenderId,RecipientId,AbsenceCategoryId,Description,Date,DateFrom,DateTo")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                notification.SenderId = User.Identity.GetUserId();
                notification.Date = DateTime.Now;
                db.Notifications.Add(notification);
                db.SaveChanges();

                sendMail(notification);

                return RedirectToAction("Index");
            }
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ViewBag.recipientId = new SelectList(UserManager.Users.ToList(), "Id", "UserName");
            ViewBag.absenceCategoryId = new SelectList(db.AbsenceCategories.ToList(), "Id", "CategoryName");
            ViewBag.dateError = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors).Where(x => x.ErrorMessage == "\"Date to\" must be greater than \"Date from\"")
                                        .Select(x => x.ErrorMessage));
            return View(notification);
        }                

        // GET: Notifications/Edit/5
        [AuthorizeUser(Permissions="EDIT_NOTIFICATION")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ViewBag.AbsenceCategoryId = new SelectList(db.AbsenceCategories, "Id", "CategoryName", notification.AbsenceCategoryId);
            ViewBag.RecipientId = new SelectList(db.Users, "Id", "Email", notification.RecipientId);
            ViewBag.SenderId = new SelectList(db.Users, "Id", "Email", notification.SenderId);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Permissions="EDIT_NOTIFICATION")]
        public ActionResult Edit([Bind(Include = "Id,SenderId,RecipientId,AbsenceCategoryId,Description,Date,DateFrom,DateTo")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AbsenceCategoryId = new SelectList(db.AbsenceCategories, "Id", "CategoryName", notification.AbsenceCategoryId);
            ViewBag.RecipientId = new SelectList(db.Users, "Id", "Email", notification.RecipientId);
            ViewBag.SenderId = new SelectList(db.Users, "Id", "Email", notification.SenderId);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        [AuthorizeUser(Permissions="DELETE_NOTIFICATION")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Permissions="DELETE_NOTIFICATION")]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = db.Notifications.Find(id);
            db.Notifications.Remove(notification);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void sendMail(Notification notification)
        {
            var users = db.Users.ToList();
            NotificationViewModel vnotification = new NotificationViewModel
            {
                Id = notification.Id,
                SenderId = notification.SenderId,
                SenderName = users.Where(m => m.Id == notification.SenderId).SingleOrDefault().UserName,
                RecipientId = notification.RecipientId,
                RecipientName = users.Where(m => m.Id == notification.RecipientId).SingleOrDefault().UserName,
                AbsenceCategoryId = notification.AbsenceCategoryId,
                AbsenceCategoryName = db.AbsenceCategories.Where(m => m.Id == notification.AbsenceCategoryId).SingleOrDefault().CategoryName,
                Description = notification.Description,
                Date = notification.Date,
                DateFrom = notification.DateFrom,
                DateTo = notification.DateTo
            };

            MailViewModel mail = new MailViewModel();
            mail.FromEmail = User.Identity.Name;
            mail.Message =
                "<div>" +
                "<h3>Notification</h3>" +
                "<hr />" +
                "<dl class=\"dl - horizontal\">" +
                "<dt><strong>Employee</strong></dt>" +
                "<dd>" + mail.FromEmail + "</dd>" +
                "<dt><strong>Absence category</strong></dt>" +
                "<dd>" + vnotification.AbsenceCategoryName + "</dd>" +
                "<dt><strong>Description</strong></dt>" +
                "<dd>" + vnotification.Description + "</dd>" +
                "<dt><strong>Date</strong></dt>" +
                "<dd>" + vnotification.Date + "</dd>" +
                "<dt><strong>Date from</strong></dt>" +
                "<dd>" + vnotification.DateFrom + "</dd>" +
                "<dt><strong>Date to</strong></dt>" +
                "<dd>" + vnotification.DateTo + "</dd>" +
                "</dl>" +
                "</div>";
            var body = "<p>Email From: {0} </p><p>Message:</p><p>{1}</p>";
            var message = new MailMessage();
            message.To.Add(new MailAddress(db.Users.Where(m => m.Id == notification.RecipientId).SingleOrDefault().Email));
            message.Subject = "Notification";
            message.Body = string.Format(body, mail.FromEmail, mail.Message);
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                ICredentialsByHost credentials = smtp.Credentials;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = credentials;
                smtp.Send(message);
            }
        }

    }
}
