using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRSM.Models
{
    [Table("NOTIFICATION")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("NOTIFICATION_ID")]
        public int Id { get; set; }

        [Required]
        [Column("USER_ID")]
        public int UserId { get; set; }

        [Required]
        [Column("MESSAGE")]
        public string Message { get; set; } = string.Empty;

        [Required]
        [Column("TYPE")]
        public string Type { get; set; } = string.Empty; // booking, service, feedback, etc.

        [Required]
        [Column("ICON")]
        public string Icon { get; set; } = "fas fa-bell"; // Font Awesome icon name

        [Column("RELATED_ID")]
        public int? RelatedId { get; set; } // ID of related entity (booking, user, room, etc.)

        [Required]
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("IS_READ")]
        public bool IsRead { get; set; } = false;

        [Column("RELATED_URL")]
        public string? RelatedUrl { get; set; } // URL to related entity page
        
        // Navigation property for User
        public User? User { get; set; }
    }
} 