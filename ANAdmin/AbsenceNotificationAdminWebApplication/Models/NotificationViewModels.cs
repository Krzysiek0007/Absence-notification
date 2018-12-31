using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AbsenceNotificationAdminWebApplication.Models
{
    public class NotificationViewModel
    {
        [Display(Name = "login")]
        public string Login { get; set; }

        [Display(Name = "date")]
        [DisplayFormat(DataFormatString = "{0:g}")] 
        public DateTime Date { get; set; }

        [Display(Name = "absence period")]
        public string AbsencePeriod { get; set; }

        [Display(Name = "absence duration (hours)")]
        public string AbsenceDuration { get; set; }
        
        [Display(Name = "absence category")]
        public string AbsenceCategory { get; set; }

        [Display(Name = "absence description")]
        public string AbsenceDescription { get; set; }
    }
}