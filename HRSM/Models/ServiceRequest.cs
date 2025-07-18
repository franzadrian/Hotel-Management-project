using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRSM.Models
{
    [Table("SERVICE_REQUEST")]
    public class ServiceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("REQUEST_ID")]
        public int RequestId { get; set; }

        [Required]
        [Column("USER_ID")]
        public int UserId { get; set; }

        [Required]
        [Column("ROOM_NUMBER")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [Column("REQUEST_TYPE")]
        public string RequestType { get; set; } = string.Empty; // Maintenance or Housekeeping

        [Column("SERVICE_CATEGORY")]
        public string ServiceCategory { get; set; } = string.Empty; // Plumbing, Electrical, etc.

        [Column("HOUSEKEEPING_NEEDS")]
        public string? HousekeepingNeeds { get; set; } // Comma-separated list of needs

        [Required]
        [Column("DESCRIPTION")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column("REQUEST_DATE")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Required]
        [Column("STATUS")]
        public string Status { get; set; } = "Pending";

        // Navigation property for User
        public User? User { get; set; }
    }
} 