using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRSM.Models
{
    [Table("FEEDBACK")]
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("FEEDBACK_ID")]
        public int FeedbackId { get; set; }

        [Required]
        [Column("USER_ID")]
        public int UserId { get; set; }

        [Required]
        [Column("ROOM_TYPE")]
        public string RoomType { get; set; } = string.Empty;

        [Required]
        [Column("RATING")]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [Column("COMMENT")]
        public string Comment { get; set; } = string.Empty;

        [Required]
        [Column("SUBMIT_DATE")]
        public DateTime SubmitDate { get; set; } = DateTime.Now;

        [Required]
        [Column("STAY_TYPE")]
        public string StayType { get; set; } = string.Empty; // "CheckedIn" or "CheckedOut"
        
        [Column("ADMIN_RESPONSE")]
        public string? AdminResponse { get; set; }
        
        [Column("RESPONSE_DATE")]
        public DateTime? ResponseDate { get; set; }

        // Navigation property for User
        public User? User { get; set; }
    }
} 