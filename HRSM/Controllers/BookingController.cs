using Microsoft.AspNetCore.Mvc;
using HRSM.Models;
using HRSM.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Security.Claims;
using HRSM.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HRSM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly HRSMDbContext _context;
        private readonly ILogger<BookingController> _logger;
        private readonly AdminController _adminController;

        public BookingController(HRSMDbContext context, ILogger<BookingController> logger, AdminController adminController)
        {
            _context = context;
            _logger = logger;
            _adminController = adminController;
        }

        private string GenerateReservationNumber(string roomType)
        {
            // Get current year
            string year = DateTime.Now.Year.ToString();
            
            // Generate a random number between 1000-9999 for additional uniqueness
            Random random = new Random();
            int randomNum = random.Next(1000, 9999);
            
            // Combine into format LC2025-5614
            return $"LC{year}-{randomNum}";
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            try
            {
                _logger.LogInformation("Starting booking creation process...");
                _logger.LogInformation("Request details: {@Request}", request);

                if (request == null)
                {
                    _logger.LogWarning("Booking request is null");
                    return BadRequest(new { success = false, message = "No booking data provided" });
                }

                // Validate request data
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    _logger.LogWarning("Invalid model state: {@Errors}", errors);
                    return BadRequest(new { success = false, message = "Invalid booking data", errors });
                }

                // Clean and validate phone number
                var cleanPhoneNumber = request.GuestContact.Replace(" ", "");
                if (!Regex.IsMatch(cleanPhoneNumber, @"^(\+63|0)[0-9]{10}$"))
                {
                    return BadRequest(new { success = false, message = "Invalid phone number format" });
                }

                // Parse and validate dates
                if (!DateTime.TryParse(request.CheckInDate, out DateTime checkInDate))
                {
                    _logger.LogWarning("Invalid check-in date format: {CheckInDate}", request.CheckInDate);
                    return BadRequest(new { success = false, message = "Invalid check-in date format" });
                }

                if (!DateTime.TryParse(request.CheckOutDate, out DateTime checkOutDate))
                {
                    _logger.LogWarning("Invalid check-out date format: {CheckOutDate}", request.CheckOutDate);
                    return BadRequest(new { success = false, message = "Invalid check-out date format" });
                }

                if (checkInDate >= checkOutDate)
                {
                    _logger.LogWarning("Invalid date range: Check-in {CheckIn} is not before Check-out {CheckOut}", 
                        checkInDate, checkOutDate);
                    return BadRequest(new { success = false, message = "Check-in date must be before check-out date" });
                }

                // Begin transaction
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _logger.LogInformation("Checking for existing user...");
                    
                    // Check if user already exists
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.GuestEmail);
                    int userId;

                    if (existingUser != null)
                    {
                        _logger.LogInformation("Using existing user with ID: {UserId}", existingUser.User_Id);
                        userId = existingUser.User_Id;
                        
                        // Check if user already has an active or pending booking
                        var activeBooking = await _context.Bookings
                            .Where(b => b.UserId == userId && 
                                  (b.Status == "Pending" || b.Status == "Confirmed" || b.Status == "CheckedIn") &&
                                   b.CheckOutDate > DateTime.Now)
                            .FirstOrDefaultAsync();
                            
                        if (activeBooking != null)
                        {
                            _logger.LogWarning("User already has an active booking. ReservationNumber: {ReservationNumber}", 
                                activeBooking.ReservationNumber);
                            return BadRequest(new { 
                                success = false, 
                                message = "You already have an active or pending reservation. Please complete or cancel your existing reservation before creating a new one.",
                                reservationNumber = activeBooking.ReservationNumber
                            });
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Creating new user record...");
                        // Create User record
                        var user = new User
                        {
                            FirstName = request.GuestName.Split(' ')[0],
                            LastName = request.GuestName.Split(' ').Length > 1 ? string.Join(" ", request.GuestName.Split(' ').Skip(1)) : "",
                            Contact = cleanPhoneNumber,
                            Email = request.GuestEmail,
                            Password = "temp" + Guid.NewGuid().ToString("N").Substring(0, 8), // Temporary password
                            Role = "Guest",
                            MemberSince = DateTime.Now
                        };

                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();
                        userId = user.User_Id;
                        _logger.LogInformation("User record created successfully. UserId: {UserId}", userId);
                    }

                    // Generate reservation number based on room type
                    string reservationNumber = GenerateReservationNumber(request.RoomType);

                    // Create Booking record with the new model structure
                    var booking = new Booking
                    {
                        ReservationNumber = reservationNumber,
                        UserId = userId,
                        RoomNumber = request.RoomNumber,
                        RoomType = request.RoomType,
                        RoomRate = GetRoomRate(request.RoomType),
                        CheckInDate = checkInDate,
                        CheckOutDate = checkOutDate,
                        BookingDate = DateTime.Now,
                        BookingType = request.BookingType,
                        NumAdults = request.Adults,
                        NumChildren = request.Children,
                        ExtraBeds = request.ExtraBeds,
                        ExtraRooms = request.ExtraRooms,
                        SpaService = request.SpaService,
                        RoomService = request.RoomService,
                        AirportTransfer = request.AirportTransfer,
                        SpecialRequests = request.SpecialRequests,
                        TotalAmount = request.TotalAmount,
                        Status = "Pending", // Always set status to Pending regardless of payment method
                        PaymentMethod = request.PaymentMethod
                    };

                    try
                    {
                        _context.Bookings.Add(booking);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Booking record created successfully. ReservationNumber: {ReservationNumber}", reservationNumber);

                        // Get user information for the activity log
                        var user = await _context.Users.FindAsync(userId);
                        if (user == null)
                        {
                            _logger.LogWarning("User not found for activity logging. Using generic guest name.");
                        }

                        // Log the booking activity
                        string guestName = user != null 
                            ? $"{user.FirstName} {user.LastName}" 
                            : $"Guest (ID: {userId})";
                            
                        var activity = new Activity
                        {
                            Message = $"New booking created for {booking.RoomType} Room {booking.RoomNumber} by {guestName}",
                            ActivityType = "booking",
                            Icon = "fas fa-calendar-check",
                            RelatedId = booking.BookingId,
                            CreatedAt = DateTime.Now,
                            RelatedUrl = "/Admin/ReservationManagement"
                        };

                        _context.Activities.Add(activity);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Booking activity logged successfully");

                        // Create a notification for the user
                        if (user != null)
                        {
                            var notification = new Notification
                            {
                                UserId = userId,
                                Message = $"Your booking for {booking.RoomType} Room {booking.RoomNumber} has been created successfully",
                                Type = "booking",
                                Icon = "fas fa-calendar-check",
                                RelatedId = booking.BookingId,
                                RelatedUrl = "/Reservation/Index"
                            };

                            _context.Notifications.Add(notification);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("User notification created for new booking");
                        }

                        // Commit transaction
                        await transaction.CommitAsync();
                        
                        _logger.LogInformation("Booking process completed successfully");
                        return Ok(new { 
                            success = true,
                            reservationNumber = booking.ReservationNumber,
                            message = "Booking created successfully"
                        });
                    }
                    catch (DbUpdateException dbEx)
                    {
                        _logger.LogError(dbEx, "Database error while saving booking: {Message}", dbEx.Message);
                        await transaction.RollbackAsync();
                        return StatusCode(500, new { 
                            success = false,
                            message = "A database error occurred while saving the booking.",
                            error = dbEx.InnerException?.Message ?? dbEx.Message
                        });
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during transaction. Rolling back changes: {Message}", ex.Message);
                    return StatusCode(500, new { 
                        success = false,
                        message = "An error occurred during the booking process.",
                        error = ex.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred: {Message}", ex.Message);
                return StatusCode(500, new { 
                    success = false,
                    message = "An unexpected error occurred while processing your booking.",
                    error = ex.Message
                });
            }
        }

        private decimal GetRoomRate(string roomType)
        {
            return roomType.ToLower() switch
            {
                "standard" => 1806M,
                "deluxe" => 3588M,
                "suite" => 15000M,
                _ => throw new ArgumentException($"Invalid room type: {roomType}")
            };
        }

        // Get all reservations with optional filtering
        [HttpGet]
        [Route("GetReservations")]
        public async Task<IActionResult> GetReservations(string? status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Fetching reservations with filters - Status: {Status}, StartDate: {StartDate}, EndDate: {EndDate}", 
                    status, startDate, endDate);

                var query = _context.Bookings
                    .Include(b => b.User)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
                {
                    query = query.Where(b => b.Status.ToLower() == status.ToLower());
                }

                if (startDate.HasValue)
                {
                    query = query.Where(b => b.CheckInDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(b => b.CheckOutDate <= endDate.Value);
                }

                var reservations = await query
                    .OrderByDescending(b => b.BookingDate)
                    .Select(b => new {
                        b.ReservationNumber,
                        GuestName = b.User == null ? "N/A" : b.User.FirstName + " " + b.User.LastName,
                        b.RoomType,
                        CheckIn = b.CheckInDate.ToString("dd/MM/yyyy"),
                        CheckOut = b.CheckOutDate.ToString("dd/MM/yyyy"),
                        b.Status,
                        b.TotalAmount,
                        b.BookingId,
                        GuestContact = b.User == null ? "N/A" : b.User.Contact,
                        GuestEmail = b.User == null ? "N/A" : b.User.Email,
                        b.NumAdults,
                        b.NumChildren,
                        b.SpecialRequests,
                        b.PaymentMethod
                    })
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} reservations", reservations.Count);
                return Ok(new { success = true, data = reservations });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching reservations");
                return StatusCode(500, new { success = false, message = "Error retrieving reservations", error = ex.Message });
            }
        }

        // Update reservation status
        [HttpPut]
        [Route("UpdateStatus/{bookingId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateReservationStatus(int bookingId, [FromBody] string newStatus)
        {
            try
            {
                _logger.LogInformation("Updating reservation status - BookingId: {BookingId}, NewStatus: {NewStatus}", 
                    bookingId, newStatus);

                var booking = await _context.Bookings
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
                    
                if (booking == null)
                {
                    _logger.LogWarning("Booking not found with ID: {BookingId}", bookingId);
                    return NotFound(new { success = false, message = "Booking not found" });
                }

                var oldStatus = booking.Status;
                booking.Status = newStatus;
                
                // When checking in, update payment status to Paid
                if (newStatus == "CheckedIn")
                {
                    booking.PaymentStatus = "Paid";
                    _logger.LogInformation("Payment status updated to Paid for booking ID: {BookingId}", bookingId);
                }
                
                await _context.SaveChangesAsync();

                // Log the status change activity
                string guestName = booking.User != null ? $"{booking.User.FirstName} {booking.User.LastName}" : "Unknown Guest";
                string activityIcon = GetStatusIcon(newStatus);
                
                await _adminController.LogActivityAsync(
                    $"Booking #{booking.ReservationNumber} status changed from {oldStatus} to {newStatus} for guest {guestName}",
                    "booking",
                    activityIcon,
                    booking.BookingId,
                    "/Admin/ReservationManagement"
                );

                // Create a user notification if the user exists
                if (booking.User != null)
                {
                    string notificationMessage = GetStatusNotificationMessage(newStatus, booking.ReservationNumber, booking.RoomType);
                    
                    await _adminController.CreateUserNotificationAsync(
                        booking.User.User_Id,
                        notificationMessage,
                        "booking",
                        activityIcon,
                        booking.BookingId,
                        "/Reservation/Index"
                    );
                    
                    _logger.LogInformation("User notification created for booking status change");
                }

                _logger.LogInformation("Successfully updated booking status");
                return Ok(new { success = true, message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating reservation status: {Message}", ex.Message);
                return StatusCode(500, new { success = false, message = "Error updating status", error = ex.Message });
            }
        }

        // Helper method to get appropriate icon for booking status
        private string GetStatusIcon(string status)
        {
            return status.ToLower() switch
            {
                "confirmed" => "fas fa-check-circle",
                "checkedin" => "fas fa-door-open",
                "checkedout" => "fas fa-door-closed",
                "cancelled" => "fas fa-times-circle",
                "pending" => "fas fa-hourglass-half",
                "noshow" => "fas fa-user-slash",
                _ => "fas fa-calendar-day"
            };
        }
        
        // Helper method to get appropriate notification message for booking status
        private string GetStatusNotificationMessage(string status, string reservationNumber, string roomType)
        {
            return status.ToLower() switch
            {
                "confirmed" => $"Your booking #{reservationNumber} for {roomType} has been confirmed",
                "checkedin" => $"Welcome! You have been checked in to your {roomType} room",
                "checkedout" => $"Thank you for staying with us! You have been checked out from your {roomType} room",
                "cancelled" => $"Your booking #{reservationNumber} for {roomType} has been cancelled",
                "pending" => $"Your booking #{reservationNumber} for {roomType} is pending confirmation",
                "noshow" => $"Your booking #{reservationNumber} for {roomType} has been marked as no-show",
                _ => $"Your booking #{reservationNumber} for {roomType} status has been updated to {status}"
            };
        }

        // Get reservation details
        [HttpGet]
        [Route("GetReservation/{bookingId}")]
        public async Task<IActionResult> GetReservationDetails(int bookingId)
        {
            try
            {
                _logger.LogInformation("Fetching reservation details for BookingId: {BookingId}", bookingId);

                var booking = await _context.Bookings
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    _logger.LogWarning("Booking not found with ID: {BookingId}", bookingId);
                    return NotFound(new { success = false, message = "Booking not found" });
                }

                var details = new {
                    booking.ReservationNumber,
                    GuestName = booking.User != null ? $"{booking.User.FirstName} {booking.User.LastName}" : "N/A",
                    Contact = booking.User?.Contact ?? "N/A",
                    Email = booking.User?.Email ?? "N/A",
                    booking.RoomType,
                    booking.RoomNumber,
                    CheckIn = booking.CheckInDate.ToString("dd/MM/yyyy"),
                    CheckOut = booking.CheckOutDate.ToString("dd/MM/yyyy"),
                    Nights = (booking.CheckOutDate - booking.CheckInDate).Days,
                    booking.NumAdults,
                    booking.NumChildren,
                    booking.Status,
                    booking.PaymentMethod,
                    booking.TotalAmount,
                    booking.SpecialRequests,
                    booking.ExtraBeds,
                    booking.ExtraRooms,
                    booking.SpaService,
                    booking.RoomService,
                    booking.AirportTransfer
                };

                _logger.LogInformation("Successfully retrieved reservation details");
                return Ok(new { success = true, data = details });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching reservation details");
                return StatusCode(500, new { success = false, message = "Error retrieving reservation details", error = ex.Message });
            }
        }
    }

    public class BookingRequest
    {
        [Required(ErrorMessage = "Room type is required")]
        public string RoomType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Room number is required")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Booking type is required")]
        public string BookingType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Check-in date is required")]
        public string CheckInDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Check-out date is required")]
        public string CheckOutDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Guest name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string GuestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required")]
        [RegularExpression(@"^(\+63|0)[0-9]{10}$", ErrorMessage = "Please enter a valid Philippine phone number")]
        public string GuestContact { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string GuestEmail { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "Number of adults must be between 1 and 10")]
        public int Adults { get; set; } = 1;

        [Range(0, 10, ErrorMessage = "Number of children must be between 0 and 10")]
        public int Children { get; set; }

        [Range(0, 5, ErrorMessage = "Number of extra beds must be between 0 and 5")]
        public int ExtraBeds { get; set; }

        [Range(0, 5, ErrorMessage = "Number of extra rooms must be between 0 and 5")]
        public int ExtraRooms { get; set; }

        public bool SpaService { get; set; }
        public bool RoomService { get; set; }
        public bool AirportTransfer { get; set; }

        public string? SpecialRequests { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Total amount must be greater than or equal to 0")]
        public decimal TotalAmount { get; set; }
    }
} 