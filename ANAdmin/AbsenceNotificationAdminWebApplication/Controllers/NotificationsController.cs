using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AbsenceNotificationAdminWebApplication.Calculate;
using AbsenceNotificationAdminWebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AbsenceNotificationAdminWebApplication.Controllers
{
    public class NotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Notification
        [AuthorizeUser(Permissions = "LIST_ALL_NOTIFICATIONS")]
        public ActionResult Index(DateTime? dateFrom, DateTime? dateTo)
        {
            var userId = User.Identity.GetUserId();
            var users = db.Users.ToList();
            dateFrom = dateFrom != null ? dateFrom : DateTime.MinValue;
            dateTo = dateTo != null ? dateTo : DateTime.MaxValue;

            ViewBag.start = dateFrom;
            ViewBag.end = dateTo;

            List<NotificationViewModel> model = new List<NotificationViewModel>();

            foreach (var item in db.Notifications.Where(n =>
                (n.DateFrom <= dateFrom && dateFrom <= n.DateTo && dateFrom <= dateTo)
                || (n.DateFrom <= dateTo && dateTo <= n.DateTo && dateFrom <= dateTo)
                || (dateFrom <= n.DateFrom && n.DateTo <= dateTo)))
            {
                var calculator = new Calculation(new List<DateTime>(), new OpenHours("08:00;16:00"));

                var minutes = calculator.getElapsedMinutes(item.DateFrom, item.DateTo);
                var hours = Math.Round(minutes / 60);

                model.Add(new NotificationViewModel
                {
                    Login = users.Where(m => m.Id == item.SenderId).SingleOrDefault().UserName,
                    Date = item.Date,
                    AbsencePeriod = item.DateFrom.ToString("g") + " --> " + item.DateTo.ToString("g"),
                    AbsenceDuration = hours.ToString(),
                    AbsenceCategory = db.AbsenceCategories.Where(m => m.Id == item.AbsenceCategoryId).SingleOrDefault().CategoryName,
                    AbsenceDescription = item.Description,
                });
            }
            return View(model.ToList());
        }

        [AuthorizeUser(Permissions = "EXPORT_LIST_NOTIFICATIONS")]
        public ActionResult ExportView(DateTime? dateFrom, DateTime? dateTo)
        {
            var userId = User.Identity.GetUserId();
            var users = db.Users.ToList();

            dateFrom = dateFrom != null ? dateFrom : DateTime.MinValue;
            dateTo = dateTo != null ? dateTo : DateTime.MaxValue;

            List<NotificationViewModel> model = new List<NotificationViewModel>();

            foreach (var item in db.Notifications.Where(n =>
                (n.DateFrom <= dateFrom && dateFrom <= n.DateTo && dateFrom <= dateTo)
                || (n.DateFrom <= dateTo && dateTo <= n.DateTo && dateFrom <= dateTo)
                || (dateFrom <= n.DateFrom && n.DateTo <= dateTo)))
            {

                var calculator = new Calculation(new List<DateTime>(), new OpenHours("08:00;16:00"));

                var minutes = calculator.getElapsedMinutes(item.DateFrom, item.DateTo);
                var hours = Math.Round(minutes / 60);

                model.Add(new NotificationViewModel
                {
                    Login = users.Where(m => m.Id == item.SenderId).SingleOrDefault().UserName,
                    Date = item.Date,
                    AbsencePeriod = item.DateFrom.ToString("g") + " --> " + item.DateTo.ToString("g"),
                    AbsenceDuration = hours.ToString(),
                    AbsenceCategory = db.AbsenceCategories.Where(m => m.Id == item.AbsenceCategoryId).SingleOrDefault().CategoryName,
                    AbsenceDescription = item.Description,
                });
            }

            Response.AddHeader("content-disposition", "attachment;filename=Notifications.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
