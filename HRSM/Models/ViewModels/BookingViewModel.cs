using System;

namespace HRSM.Models.ViewModels
{
    public class BookingViewModel
    {
        public int BookingId { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsCheckedIn => Status == "CheckedIn";
    }
} 