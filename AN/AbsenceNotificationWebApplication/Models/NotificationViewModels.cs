using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AbsenceNotificationWebApplication.Models
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }

        [Display(Name = "Sender")]
        public string SenderName { get; set; }

        [Display(Name = "Recipient")]
        public string RecipientName { get; set; }
        public int AbsenceCategoryId { get; set; }

        [Display(Name = "Absence category")]
        public string AbsenceCategoryName { get; set; }
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime Date { get; set; }

        [Display(Name = "Date from")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime DateFrom { get; set; }

        [Display(Name = "Date to")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime DateTo { get; set; }
    }

    public class MailViewModel
    {
        public string FromEmail { get; set; }
        public string Message { get; set; }
    }
}