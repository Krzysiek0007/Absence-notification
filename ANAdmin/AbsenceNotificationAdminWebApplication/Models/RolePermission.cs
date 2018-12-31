using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AbsenceNotificationAdminWebApplication.Models
{
    public class RolePermission
    {
        public int ID { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Role")]
        public string RoleId { get; set; }

        [Required]
        [ForeignKey("Permission")]
        public int PermissionId { get; set; }

        public virtual Permission Permission { get; set; }

        public virtual IdentityRole Role { get; set; }
    }
}