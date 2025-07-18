using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRSM.Models
{
    [Table("BOOKING")]
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("BOOKING_ID")]
        public int BookingId { get; set; }

        [Required]
        [Column("RESERVATION_NUMBER")]
        public string ReservationNumber { get; set; } = string.Empty;

        [Required]
        [Column("USER_ID")]
        public int UserId { get; set; }

        [Required]
        [Column("ROOM_NUMBER")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [Column("ROOM_TYPE")]
        public string RoomType { get; set; } = string.Empty;

        [Required]
        [Column("ROOM_RATE")]
        public decimal RoomRate { get; set; }

        [Required]
        [Column("CHECK_IN_DATE")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [Column("CHECK_OUT_DATE")]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Column("BOOKING_DATE")]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        [Column("BOOKING_TYPE")]
        public string BookingType { get; set; } = string.Empty;

        [Required]
        [Column("NUM_ADULTS")]
        public int NumAdults { get; set; }

        [Required]
        [Column("NUM_CHILDREN")]
        public int NumChildren { get; set; }

        [Column("EXTRA_BEDS")]
        public int ExtraBeds { get; set; }

        [Column("EXTRA_ROOMS")]
        public int ExtraRooms { get; set; }

        [Column("SPA_SERVICE")]
        public bool SpaService { get; set; }

        [Column("ROOM_SERVICE")]
        public bool RoomService { get; set; }

        [Column("AIRPORT_TRANSFER")]
        public bool AirportTransfer { get; set; }

        [Column("SPECIAL_REQUESTS")]
        public string? SpecialRequests { get; set; }

        [Required]
        [Column("TOTAL_AMOUNT")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column("STATUS")]
        public string Status { get; set; } = "Pending";

        [Required]
        [Column("PAYMENT_STATUS")]
        public string PaymentStatus { get; set; } = "Pending";

        [Required]
        [Column("PAYMENT_METHOD")]
        public string PaymentMethod { get; set; } = string.Empty;

        // Navigation property for User
        public User? User { get; set; }
    }
}