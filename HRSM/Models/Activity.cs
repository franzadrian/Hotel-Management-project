using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRSM.Models
{
    [Table("ACTIVITY")]
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ACTIVITY_ID")]
        public int ActivityId { get; set; }

        [Required]
        [Column("MESSAGE")]
        public string Message { get; set; } = string.Empty;

        [Column("ACTIVITY_TYPE")]
        public string ActivityType { get; set; } = string.Empty; // booking, service, checkin, checkout, feedback, etc.

        [Column("ICON")]
        public string Icon { get; set; } = "fa-bell"; // Font Awesome icon name

        [Column("RELATED_ID")]
        public int? RelatedId { get; set; } // ID of related entity (booking, user, room, etc.)

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("RELATED_URL")]
        public string? RelatedUrl { get; set; } // URL to related entity page
    }
} 