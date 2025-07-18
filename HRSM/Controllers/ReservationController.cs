using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRSM.Data;
using HRSM.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HRSM.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ReservationController : Controller
    {
        private readonly HRSMDbContext _context;
        private readonly ILogger<ReservationController> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ReservationController(HRSMDbContext context, ILogger<ReservationController> logger)
        {
            _context = context;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(string? reservationNumber = null)
        {
            try
            {
                _logger.LogInformation("Starting reservation view request");
                
                // Get user ID from claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation("User ID from claims: {UserId}", userId);

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
                {
                    _logger.LogWarning("Invalid user ID in claims: {UserId}", userId);
                    return RedirectToAction("Login", "Account");
                }

                // If reservation number is provided, find that specific booking
                if (!string.IsNullOrEmpty(reservationNumber))
                {
                    var specificBooking = await _context.Bookings
                        .Include(b => b.User)
                        .FirstOrDefaultAsync(b => b.ReservationNumber == reservationNumber && b.UserId == userIdInt);

                    if (specificBooking != null)
                    {
                        ViewBag.Nights = (specificBooking.CheckOutDate - specificBooking.CheckInDate).Days;
                        NormalizeRoomType(specificBooking);
                        return View("~/Views/Sidebar/Reservation.cshtml", specificBooking);
                    }
                }

                // Get all bookings for the user
                var bookings = await _context.Bookings
                    .Include(b => b.User)
                    .Where(b => b.UserId == userIdInt)
                    .OrderByDescending(b => b.BookingDate)
                    .ToListAsync();

                if (!bookings.Any())
                {
                    _logger.LogInformation("No bookings found for user {UserId}", userIdInt);
                    ViewBag.NoBooking = true;
                    return View("~/Views/Sidebar/Reservation.cshtml");
                }

                // Separate active and past reservations
                var currentDate = DateTime.Now;
                var activeReservation = bookings.FirstOrDefault(b => 
                    !b.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) && 
                    !b.Status.Replace(" ", "").Equals("CheckOut", StringComparison.OrdinalIgnoreCase) &&
                    !b.Status.Replace(" ", "").Equals("Checkout", StringComparison.OrdinalIgnoreCase) &&
                    !b.Status.Replace(" ", "").Equals("CheckedOut", StringComparison.OrdinalIgnoreCase) &&
                    b.CheckOutDate >= currentDate);

                var pastReservations = bookings.Where(b => 
                    b.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) || 
                    b.Status.Replace(" ", "").Equals("CheckOut", StringComparison.OrdinalIgnoreCase) ||
                    b.Status.Replace(" ", "").Equals("Checkout", StringComparison.OrdinalIgnoreCase) ||
                    b.Status.Replace(" ", "").Equals("CheckedOut", StringComparison.OrdinalIgnoreCase) ||
                    b.CheckOutDate < currentDate)
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();

                // Calculate nights for active reservation
                if (activeReservation != null)
                {
                    ViewBag.Nights = (activeReservation.CheckOutDate - activeReservation.CheckInDate).Days;
                    NormalizeRoomType(activeReservation);
                }

                // Add logging to help debug the issue
                _logger.LogInformation("Current Date: {CurrentDate}", currentDate);
                _logger.LogInformation("Active Reservation: {ActiveReservation}", 
                    activeReservation != null ? 
                    $"ID: {activeReservation.BookingId}, Status: {activeReservation.Status}, CheckOut: {activeReservation.CheckOutDate}" : 
                    "None");
                _logger.LogInformation("Past Reservations Count: {Count}", pastReservations.Count);
                foreach (var past in pastReservations)
                {
                    _logger.LogInformation("Past Reservation - ID: {Id}, Status: {Status}, CheckOut: {CheckOut}", 
                        past.BookingId, past.Status, past.CheckOutDate);
                }

                // Calculate nights for past reservations and prepare them for serialization
                var serializedPastReservations = pastReservations.Select(booking => new
                {
                    booking.ReservationNumber,
                    booking.RoomType,
                    booking.RoomRate,
                    booking.CheckInDate,
                    booking.CheckOutDate,
                    booking.BookingDate,
                    booking.Status,
                    booking.PaymentMethod,
                    booking.TotalAmount,
                    booking.NumAdults,
                    booking.NumChildren,
                    booking.ExtraBeds,
                    booking.ExtraRooms,
                    booking.SpaService,
                    booking.RoomService,
                    booking.AirportTransfer,
                    User = booking.User == null ? new
                    {
                        FirstName = "N/A",
                        LastName = "",
                        Email = "N/A",
                        Contact = "N/A"
                    } : new
                    {
                        FirstName = booking.User.FirstName,
                        LastName = booking.User.LastName,
                        Email = booking.User.Email,
                        Contact = booking.User.Contact
                    }
                }).ToList();

                // Store the serialized past reservations in ViewBag
                ViewBag.SerializedPastReservations = JsonSerializer.Serialize(serializedPastReservations, _jsonOptions);
                ViewBag.PastReservations = pastReservations;

                return View("~/Views/Sidebar/Reservation.cshtml", activeReservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reservation: {Message}", ex.Message);
                return View("Error");
            }
        }

        private void NormalizeRoomType(Booking booking)
        {
            booking.RoomType = (booking.RoomType ?? "Standard").Trim();
            if (booking.RoomType.Equals("SUITE", StringComparison.OrdinalIgnoreCase))
            {
                booking.RoomType = "Suite";
            }
            else if (booking.RoomType.Equals("DELUXE", StringComparison.OrdinalIgnoreCase))
            {
                booking.RoomType = "Deluxe";
            }
            else
            {
                booking.RoomType = "Standard";
            }
        }

        [HttpPost]
        [Route("CancelReservation/{bookingId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int bookingId)
        {
            try
            {
                _logger.LogInformation("Attempting to cancel reservation with ID: {BookingId}", bookingId);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
                {
                    _logger.LogWarning("User not authenticated when attempting to cancel booking {BookingId}", bookingId);
                    return BadRequest(new { success = false, message = "User not authenticated" });
                }

                var booking = await _context.Bookings
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userIdInt);
                
                if (booking == null)
                {
                    _logger.LogWarning("Booking not found or does not belong to user. BookingId: {BookingId}, UserId: {UserId}", 
                        bookingId, userIdInt);
                    return NotFound(new { success = false, message = "Booking not found" });
                }

                if (booking.Status == "Cancelled")
                {
                    _logger.LogWarning("Attempted to cancel already cancelled booking. BookingId: {BookingId}", bookingId);
                    return BadRequest(new { success = false, message = "Booking is already cancelled" });
                }

                if (booking.Status != "Pending")
                {
                    _logger.LogWarning("Attempted to cancel non-pending booking. BookingId: {BookingId}, Status: {Status}", bookingId, booking.Status);
                    return BadRequest(new { success = false, message = "Only pending reservations can be cancelled" });
                }

                if (booking.CheckInDate <= DateTime.Now)
                {
                    _logger.LogWarning("Attempted to cancel booking after check-in date. BookingId: {BookingId}", bookingId);
                    return BadRequest(new { success = false, message = "Cannot cancel a booking after check-in date" });
                }

                booking.Status = "Cancelled";
                booking.PaymentStatus = "Cancelled";
                
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully cancelled booking {BookingId}", bookingId);
                return Ok(new { success = true, message = "Booking cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cancelling reservation {BookingId}", bookingId);
                return StatusCode(500, new { success = false, message = "An error occurred while cancelling the reservation" });
            }
        }

        [HttpPost]
        [Route("UpdateStatus/{bookingId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int bookingId, string newStatus)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    return NotFound();
                }

                // Normalize the status by removing spaces and converting to lowercase
                var normalizedStatus = newStatus.Replace(" ", "").ToLower();
                
                // Set the status based on normalized input
                switch (normalizedStatus)
                {
                    case "confirmed":
                        booking.Status = "Confirmed";
                        booking.PaymentStatus = "Paid";
                        break;
                    case "checkin":
                        booking.Status = "Check In";
                        booking.PaymentStatus = "Paid";
                        break;
                    case "checkout":
                    case "checkedout":
                        booking.Status = "Check Out";
                        booking.PaymentStatus = "Paid";
                        break;
                    case "cancelled":
                        booking.Status = "Cancelled";
                        booking.PaymentStatus = "Cancelled";
                        break;
                    case "pending":
                        booking.Status = "Pending";
                        booking.PaymentStatus = "Pending";
                        break;
                    default:
                        return BadRequest($"Invalid status: {newStatus}");
                }

                _logger.LogInformation($"Updating booking {bookingId} status to {booking.Status} and payment status to {booking.PaymentStatus}");
                await _context.SaveChangesAsync();
                return Ok(new { status = booking.Status, paymentStatus = booking.PaymentStatus });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating booking status: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 