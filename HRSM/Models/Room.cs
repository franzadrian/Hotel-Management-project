using System.ComponentModel.DataAnnotations;

namespace HRSM.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        [StringLength(10)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RoomType { get; set; } = string.Empty; // standard, deluxe, suite

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "available"; // available, occupied, cleaning, maintenance

        public DateTime StatusUpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string UpdatedBy { get; set; } = string.Empty;
    }
} 