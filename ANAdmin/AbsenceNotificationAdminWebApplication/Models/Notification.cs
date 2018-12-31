namespace AbsenceNotificationAdminWebApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Notification : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Sender")]
        public string SenderId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Recipient")]
        public string RecipientId { get; set; }

        [Required]
        [ForeignKey("AbsenceCategory")]
        public int AbsenceCategoryId { get; set; }

        [StringLength(500, ErrorMessage = "Must not exceed 500 char")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        public virtual AbsenceCategory AbsenceCategory { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Recipient { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateTo <= DateFrom)
            {
                yield return
                  new ValidationResult(errorMessage: "\"Date to\" must be greater than \"Date from\"",
                                       memberNames: new[] { "EndDate" });
            }
        }
    }
}
