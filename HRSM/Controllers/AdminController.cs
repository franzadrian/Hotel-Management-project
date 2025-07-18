using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRSM.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using HRSM.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace HRSM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly HRSMDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(HRSMDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Utility method to log activities (can be used throughout the application)
        public async Task LogActivityAsync(string message, string activityType, string icon, int? relatedId = null, string? relatedUrl = null)
        {
            try
            {
                // Filter out login/logout and system initialization messages
                if (message != null && 
                    (message.Contains("logged in") || 
                     message.Contains("logged out") ||
                     message.Contains("Hotel Reservation Management System initialized") ||
                     message.Contains("User login") ||
                     message.Contains("User logout") ||
                     message.Contains("authentication")))
                {
                    // Skip logging these types of messages
                    _logger.LogInformation($"Skipped activity logging: {message}");
                    return;
                }
                
                var activity = new Activity
                {
                    Message = message ?? string.Empty,
                    ActivityType = activityType ?? string.Empty,
                    Icon = icon ?? "fa-bell",
                    RelatedId = relatedId,
                    CreatedAt = DateTime.Now,
                    RelatedUrl = relatedUrl // This is allowed to be null in the model
                };

                _context.Activities.Add(activity);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Activity logged: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging activity: {message}");
            }
        }

        // Utility method to create user notifications
        public async Task CreateUserNotificationAsync(int userId, string message, string type, string icon, int? relatedId = null, string? relatedUrl = null)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Message = message ?? string.Empty,
                    Type = type ?? string.Empty,
                    Icon = icon ?? "fas fa-bell",
                    RelatedId = relatedId,
                    CreatedAt = DateTime.Now,
                    IsRead = false,
                    RelatedUrl = relatedUrl
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"User notification created for user {userId}: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating user notification: {message}");
            }
        }

        public IActionResult AdminDashboard()
        {
            try
            {
                // Count guests and staff
                var guestCount = _context.Users.Count(u => u.Role == "Guest");
                var staffCount = _context.Users.Count(u => u.Role == "Staff");
                
                // Get room statistics
                var allRooms = _context.Rooms.ToList();
                var roomStats = new {
                    Total = allRooms.Count,
                    Available = allRooms.Count(r => r.Status == "available"),
                    Occupied = allRooms.Count(r => r.Status == "occupied"),
                    Cleaning = allRooms.Count(r => r.Status == "cleaning"),
                    Maintenance = allRooms.Count(r => r.Status == "maintenance")
                };
                
                // Room type statistics
                var standardRooms = allRooms.Where(r => r.RoomType.ToLower() == "standard").ToList();
                var deluxeRooms = allRooms.Where(r => r.RoomType.ToLower() == "deluxe").ToList();
                var suiteRooms = allRooms.Where(r => r.RoomType.ToLower() == "suite").ToList();
                
                ViewBag.GuestCount = guestCount;
                ViewBag.StaffCount = staffCount;
                ViewBag.RoomStats = roomStats;
                
                ViewBag.StandardRooms = new {
                    Total = standardRooms.Count,
                    Available = standardRooms.Count(r => r.Status == "available"),
                    Occupied = standardRooms.Count(r => r.Status == "occupied"),
                    Cleaning = standardRooms.Count(r => r.Status == "cleaning"),
                    Maintenance = standardRooms.Count(r => r.Status == "maintenance")
                };
                
                ViewBag.DeluxeRooms = new {
                    Total = deluxeRooms.Count,
                    Available = deluxeRooms.Count(r => r.Status == "available"),
                    Occupied = deluxeRooms.Count(r => r.Status == "occupied"),
                    Cleaning = deluxeRooms.Count(r => r.Status == "cleaning"),
                    Maintenance = deluxeRooms.Count(r => r.Status == "maintenance")
                };
                
                ViewBag.SuiteRooms = new {
                    Total = suiteRooms.Count,
                    Available = suiteRooms.Count(r => r.Status == "available"),
                    Occupied = suiteRooms.Count(r => r.Status == "occupied"),
                    Cleaning = suiteRooms.Count(r => r.Status == "cleaning"),
                    Maintenance = suiteRooms.Count(r => r.Status == "maintenance")
                };

                // Fetch recent activities - limited to 4 items
                var recentActivities = _context.Activities
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(4)
                    .ToList();
                ViewBag.RecentActivities = recentActivities;

                // For notifications, you would need a Notifications table
                // This is just a placeholder for now
                ViewBag.Notifications = new List<dynamic>();
                ViewBag.UnreadNotificationsCount = 0;

                // If there are no activities yet, log a system activity
                if (!recentActivities.Any())
                {
                    _ = LogActivityAsync(
                        "Hotel Reservation Management System initialized",
                        "system",
                        "fas fa-power-off",
                        null,
                        ""  // Use empty string instead of null for non-nullable string
                    );
                }

                _logger.LogInformation("Admin dashboard loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                // Use default values if there's an error
                ViewBag.GuestCount = 0;
                ViewBag.StaffCount = 0;
                ViewBag.RecentActivities = new List<Activity>();
            }
            
            return View();
        }

        public IActionResult ReservationManagement()
        {
            return View();
        }

        public IActionResult RoomManagement()
        {
            return View();
        }

        public IActionResult ServicesManagement()
        {
            return View();
        }

        // GET: /Admin/EventManagement
        public async Task<IActionResult> EventManagement()
        {
            try
            {
                // Get all events ordered by display order
                var events = await _context.Events
                    .OrderBy(e => e.DisplayOrder)
                    .ToListAsync();
                
                // Get the current event page title from settings
                string eventPageTitle = "Events";
                var titleSetting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == "EventPageTitle");
                if (titleSetting != null && !string.IsNullOrWhiteSpace(titleSetting.Value))
                {
                    eventPageTitle = titleSetting.Value;
                }
                // If not in database, check cookies
                else if (Request.Cookies.ContainsKey("EventPageTitle"))
                {
                    eventPageTitle = Request.Cookies["EventPageTitle"] ?? "Events";
                }
                
                ViewBag.EventPageTitle = eventPageTitle;
                
                return View(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading event management page");
                return View(new List<Event>());
            }
        }

        // GET: /Admin/UserManagement
        public async Task<IActionResult> UserManagement()
        {
            var users = await _context.Users
                .Include(u => u.Bookings)
                .ToListAsync();
            return View(users);
        }

        // POST: /Admin/AddUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser([FromForm] User user)
        {
            try
            {
                // Validate passwords match
                if (user.Password != user.ConfirmPassword)
                {
                    return Json(new { 
                        success = false, 
                        message = "Passwords do not match" 
                    });
                }

                ModelState.Remove("ConfirmPassword");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Validation failed", errors });
                }

                user.Role = user.Role ?? "Guest";
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                // Log activity
                await LogActivityAsync(
                    $"New user {user.FirstName} {user.LastName} ({user.Role}) added to system", 
                    "user", 
                    "fas fa-user-plus", 
                    user.User_Id, 
                    user.Role == "Staff" ? "/Admin/StaffManagement" : "/Admin/UserManagement"
                );
                
                return Json(new { 
                    success = true, 
                    message = "User added successfully",
                    user = new {
                        user.User_Id,
                        user.FirstName,
                        user.LastName,
                        user.Contact,
                        user.Email,
                        user.Role
                    }
                });
            }
            catch (DbUpdateException ex)
            {
                string errorMessage = ex.InnerException?.Message.Contains("IX_User_Email") == true 
                    ? "Email already exists" 
                    : "Database error occurred";
                return Json(new { 
                    success = false, 
                    message = errorMessage,
                    detailedError = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Server error occurred",
                    detailedError = ex.Message
                });
            }
        }

        // POST: /Admin/EditUser/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, [FromForm] User updatedUser)
        {
            try
            {
                if (id != updatedUser.User_Id)
                {
                    return Json(new { success = false, message = "ID mismatch" });
                }

                ModelState.Remove("ConfirmPassword");
                ModelState.Remove("Password"); // Password is optional in edits

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Validation failed", errors });
                }

                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Check email uniqueness
                if (existingUser.Email != updatedUser.Email && 
                    await _context.Users.AnyAsync(u => u.Email == updatedUser.Email))
                {
                    return Json(new { success = false, message = "Email already exists" });
                }

                // Update properties
                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.Contact = updatedUser.Contact;
                existingUser.Email = updatedUser.Email;
                existingUser.Role = updatedUser.Role;

                // Only update password if provided
                if (!string.IsNullOrEmpty(updatedUser.Password))
                {
                    existingUser.Password = updatedUser.Password;
                }

                await _context.SaveChangesAsync();
                
                return Json(new { 
                    success = true, 
                    message = "User updated successfully",
                    user = new {
                        existingUser.User_Id,
                        existingUser.FirstName,
                        existingUser.LastName,
                        existingUser.Contact,
                        existingUser.Email,
                        existingUser.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Error updating user",
                    detailedError = ex.Message
                });
            }
        }

        // POST: /Admin/DeleteUser/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                
                // Log activity
                string iconName = "fas fa-user-minus";
                await LogActivityAsync(
                    $"User {user.FirstName} {user.LastName} ({user.Role}) removed from system", 
                    "user", 
                    iconName, 
                    null, 
                    user.Role == "Staff" ? "/Admin/StaffManagement" : "/Admin/UserManagement"
                );
                
                return Json(new { success = true, message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Error deleting user",
                    detailedError = ex.Message
                });
            }
        }

        // GET: /Admin/StaffManagement
        public async Task<IActionResult> StaffManagement()
        {
            var staffMembers = await _context.Users
                .Where(u => u.Role == "Staff")
                .ToListAsync();
            
            ViewBag.StaffMembers = staffMembers;
            return View(new Staff());
        }

        // POST: /Admin/StaffManagement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StaffManagement([FromForm] Staff staff)
        {
            try
            {
                // Validate passwords match
                if (staff.Password != staff.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                    ViewData["Title"] = "Staff Management";
                    return View(staff);
                }

                ModelState.Remove("ConfirmPassword");

                if (!ModelState.IsValid)
                {
                    ViewData["Title"] = "Staff Management";
                    return View(staff);
                }

                // Make sure role is set to Staff
                staff.Role = "Staff";
                
                // Check email uniqueness
                if (await _context.Users.AnyAsync(u => u.Email == staff.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    ViewData["Title"] = "Staff Management";
                    return View(staff);
                }

                _context.Users.Add(staff);
                await _context.SaveChangesAsync();
                
                // Update Staff-specific properties using shadow properties
                _context.Entry(staff).Property("Department").CurrentValue = staff.Department;
                _context.Entry(staff).Property("HireDate").CurrentValue = staff.HireDate;
                
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Staff member added successfully";
                return RedirectToAction("StaffManagement");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff member");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the staff member");
                ViewData["Title"] = "Staff Management";
                return View(staff);
            }
        }

        // POST: /Admin/EditStaff/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaff(int id, [FromForm] Staff updatedStaff)
        {
            try
            {
                if (id != updatedStaff.User_Id)
                {
                    return Json(new { success = false, message = "Invalid staff ID" });
                }

                ModelState.Remove("ConfirmPassword");
                ModelState.Remove("Password");
                ModelState.Remove("Position");
                ModelState.Remove("EmployeeId");
                
                if (!ModelState.IsValid)
                {
                    // Simplify the error format to make it easier to handle in JavaScript
                    var errors = new Dictionary<string, string>();
                    foreach (var state in ModelState)
                    {
                        if (state.Value.Errors.Count > 0)
                        {
                            errors[state.Key] = string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage));
                        }
                    }
                    
                    return Json(new { success = false, message = "Please correct the validation errors", errors });
                }

                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null || existingUser.Role != "Staff")
                {
                    return Json(new { success = false, message = "Staff member not found" });
                }

                // Check email uniqueness
                if (existingUser.Email != updatedStaff.Email && 
                    await _context.Users.AnyAsync(u => u.Email == updatedStaff.Email))
                {
                    return Json(new { success = false, message = "Email already exists" });
                }

                // Update base User properties
                existingUser.FirstName = updatedStaff.FirstName;
                existingUser.LastName = updatedStaff.LastName;
                existingUser.Contact = updatedStaff.Contact;
                existingUser.Email = updatedStaff.Email;

                // Update Staff-specific properties using shadow properties
                _context.Entry(existingUser).Property("Department").CurrentValue = updatedStaff.Department;
                _context.Entry(existingUser).Property("HireDate").CurrentValue = updatedStaff.HireDate;
                
                await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Staff information updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff member: {Message}", ex.Message);
                return Json(new { success = false, message = "An error occurred while updating the staff member: " + ex.Message });
            }
        }

        // GET: /Admin/ActivityLog
        public async Task<IActionResult> ActivityLog(string type = "", DateTime? fromDate = null, DateTime? toDate = null, int page = 1)
        {
            try
            {
                // Set page size
                const int pageSize = 10;
                
                // Start with all activities and apply filters
                var query = _context.Activities.AsQueryable();
                
                // Apply type filter if provided
                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(a => a.ActivityType == type);
                }
                
                // Apply date filters if provided
                if (fromDate.HasValue)
                {
                    query = query.Where(a => a.CreatedAt >= fromDate.Value);
                }
                
                if (toDate.HasValue)
                {
                    // Add one day to include the entire end date
                    var nextDay = toDate.Value.AddDays(1);
                    query = query.Where(a => a.CreatedAt < nextDay);
                }
                
                // Get total count for pagination
                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                
                // Ensure page is valid
                page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));
                
                // Get activities for current page
                var activities = await query
                    .OrderByDescending(a => a.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                
                // Get all activity types for the filter dropdown
                var activityTypes = await _context.Activities
                    .Select(a => a.ActivityType)
                    .Distinct()
                    .ToListAsync();
                
                // Pass data to view
                ViewBag.ActivityTypes = activityTypes;
                ViewBag.SelectedType = type;
                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalItems = totalItems;
                
                return View(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading activity log");
                
                // Return an empty list in case of error
                ViewBag.ActivityTypes = new List<string>();
                ViewBag.ErrorMessage = "An error occurred while loading the activity log. Please try again.";
                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = 1;
                ViewBag.TotalItems = 0;
                
                return View(new List<Activity>());
            }
        }

        // GET: /Admin/AnnualReport
        public IActionResult AnnualReport()
        {
            try
            {
                // Get current and last year 
                int currentYear = DateTime.Now.Year; // 2025
                int lastYear = currentYear - 1;     // 2024
                
                // Arrays to store monthly revenue data
                int[] currentYearData = new int[12];
                int[] lastYearData = new int[12];
                
                // Room type revenue data organized by room type and year
                var roomTypeRevenueData = new Dictionary<string, Dictionary<int, decimal>> {
                    { "Standard", new Dictionary<int, decimal> { { currentYear, 0 }, { lastYear, 0 } } },
                    { "Deluxe", new Dictionary<int, decimal> { { currentYear, 0 }, { lastYear, 0 } } },
                    { "Suite", new Dictionary<int, decimal> { { currentYear, 0 }, { lastYear, 0 } } }
                };
                
                // Get ALL bookings for debugging
                var allBookings = _context.Bookings.ToList();
                _logger.LogInformation($"Found {allBookings.Count} total bookings in the database");

                // Check years present in the database
                var years = allBookings.Select(b => b.BookingDate.Year).Distinct().OrderBy(y => y).ToList();
                _logger.LogInformation($"Years with bookings: {string.Join(", ", years)}");
                
                // Get paid bookings for both years
                var paidBookings = allBookings
                    .Where(b => (b.BookingDate.Year == currentYear || b.BookingDate.Year == lastYear))
                    .Where(b => b.Status != "Cancelled") 
                    .Where(b => b.PaymentStatus == "Paid" || b.PaymentStatus == "Completed")
                    .ToList();
                
                var currentYearBookings = paidBookings.Where(b => b.BookingDate.Year == currentYear).ToList();
                var lastYearBookings = paidBookings.Where(b => b.BookingDate.Year == lastYear).ToList();
                
                _logger.LogInformation($"Found {currentYearBookings.Count} paid bookings from {currentYear}");
                _logger.LogInformation($"Found {lastYearBookings.Count} paid bookings from {lastYear}");
                
                // Process current year (2025) bookings
                foreach (var booking in currentYearBookings)
                {
                    int bookingMonth = booking.BookingDate.Month - 1; // 0-based index for arrays
                    decimal amount = booking.TotalAmount;
                    string roomType = booking.RoomType;
                    
                    // Add to monthly data
                    currentYearData[bookingMonth] += (int)amount;
                    
                    // Add to room type data
                    if (roomTypeRevenueData.ContainsKey(roomType))
                    {
                        roomTypeRevenueData[roomType][currentYear] += amount;
                    }
                    
                    _logger.LogInformation($"Added {currentYear} booking amount {amount} for month {bookingMonth+1} (Room type: {roomType})");
                }
                
                // Process last year (2024) bookings
                foreach (var booking in lastYearBookings)
                {
                    int bookingMonth = booking.BookingDate.Month - 1; // 0-based index for arrays
                    decimal amount = booking.TotalAmount;
                    string roomType = booking.RoomType;
                    
                    // Add to monthly data
                    lastYearData[bookingMonth] += (int)amount;
                    
                    // Add to room type data
                    if (roomTypeRevenueData.ContainsKey(roomType))
                    {
                        roomTypeRevenueData[roomType][lastYear] += amount;
                    }
                    
                    _logger.LogInformation($"Added {lastYear} booking amount {amount} for month {bookingMonth+1} (Room type: {roomType})");
                }
                
                // Add informational messages if needed
                bool showCurrentYearMessage = currentYearBookings.Count == 0;
                bool showLastYearMessage = lastYearBookings.Count == 0;
                
                if (showCurrentYearMessage)
                {
                    ViewBag.CurrentYearMessage = $"No booking data found for {currentYear}. Please check that bookings exist and are not cancelled.";
                    ViewBag.HasNoCurrentYearData = true;
                }
                
                if (showLastYearMessage)
                {
                    ViewBag.LastYearMessage = $"No booking data found for {lastYear}. Please check that bookings exist and are not cancelled.";
                    ViewBag.HasNoLastYearData = true;
                }
                
                // Transform room type data for the view
                var roomTypeData = new {
                    Standard = new { 
                        CurrentYear = (int)roomTypeRevenueData["Standard"][currentYear], 
                        LastYear = (int)roomTypeRevenueData["Standard"][lastYear] 
                    },
                    Deluxe = new { 
                        CurrentYear = (int)roomTypeRevenueData["Deluxe"][currentYear], 
                        LastYear = (int)roomTypeRevenueData["Deluxe"][lastYear] 
                    },
                    Suite = new { 
                        CurrentYear = (int)roomTypeRevenueData["Suite"][currentYear], 
                        LastYear = (int)roomTypeRevenueData["Suite"][lastYear] 
                    }
                };
                
                // Calculate totals
                int currentYearTotal = currentYearData.Sum();
                int lastYearTotal = lastYearData.Sum();
                
                // Year-to-date calculations
                int validMonths = DateTime.Now.Month; 
                int ytdCurrentYear = 0;
                int ytdLastYear = 0;
                
                for (int i = 0; i < validMonths; i++)
                {
                    ytdCurrentYear += currentYearData[i];
                    ytdLastYear += lastYearData[i];
                }
                
                double percentChange = ytdLastYear > 0 
                    ? Math.Round(((double)ytdCurrentYear - ytdLastYear) / ytdLastYear * 100, 1) 
                    : 0;
                
                // Monthly comparison data
                var monthlyComparison = new List<object>();
                for (int i = 0; i < 12; i++)
                {
                    int diff = currentYearData[i] - lastYearData[i];
                    double pctChange = lastYearData[i] > 0 
                        ? Math.Round(((double)diff) / lastYearData[i] * 100, 1) 
                        : 0;
                        
                    monthlyComparison.Add(new {
                        Month = new DateTime(currentYear, i + 1, 1).ToString("MMM"),
                        CurrentYear = currentYearData[i],
                        LastYear = lastYearData[i],
                        Difference = diff,
                        PercentChange = pctChange
                    });
                }
                
                // Pass all data to the view
                ViewBag.CurrentYear = currentYear;
                ViewBag.LastYear = lastYear;
                ViewBag.CurrentYearData = currentYearData;
                ViewBag.LastYearData = lastYearData;
                ViewBag.RoomTypeData = roomTypeData;
                ViewBag.CurrentYearTotal = currentYearTotal;
                ViewBag.LastYearTotal = lastYearTotal;
                ViewBag.ValidMonths = validMonths;
                ViewBag.YtdCurrentYear = ytdCurrentYear;
                ViewBag.YtdLastYear = ytdLastYear;
                ViewBag.PercentChange = percentChange;
                ViewBag.MonthlyComparison = monthlyComparison;
                ViewBag.CurrentYearBookingCount = currentYearBookings.Count;
                ViewBag.LastYearBookingCount = lastYearBookings.Count;
                ViewBag.TotalBookingsInDb = allBookings.Count;
                ViewBag.UsingAllRealData = true;
                
                // Add room statistics for room analytics and availability charts
                var allRooms = _context.Rooms.ToList();
                ViewBag.RoomStats = new {
                    Total = allRooms.Count,
                    Available = allRooms.Count(r => r.Status == "available"),
                    Occupied = allRooms.Count(r => r.Status == "occupied"),
                    Cleaning = allRooms.Count(r => r.Status == "cleaning"),
                    Maintenance = allRooms.Count(r => r.Status == "maintenance")
                };
                
                _logger.LogInformation($"Annual Report page accessed with real data for both {currentYear} and {lastYear}");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Annual Report page");
                // Provide empty data if there's an error
                ViewBag.CurrentYearData = new int[12];
                ViewBag.LastYearData = new int[12];
                ViewBag.CurrentYearTotal = 0;
                ViewBag.LastYearTotal = 0;
                ViewBag.ErrorMessage = "An error occurred while generating the report: " + ex.Message;
                
                // Add default room stats to prevent view errors
                ViewBag.RoomStats = new {
                    Total = 0,
                    Available = 0,
                    Occupied = 0,
                    Cleaning = 0,
                    Maintenance = 0
                };
                
                return View();
            }
        }

        public async Task<IActionResult> FeedbackManagement(string? roomType = null, int? rating = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                // Query feedback from the database
                var query = _context.Feedbacks
                    .Include(f => f.User)
                    .AsQueryable();
                
                // Apply filters if provided
                if (!string.IsNullOrEmpty(roomType))
                {
                    query = query.Where(f => f.RoomType == roomType);
                }
                
                if (rating.HasValue && rating.Value > 0)
                {
                    query = query.Where(f => f.Rating >= rating.Value);
                }
                
                if (fromDate.HasValue)
                {
                    query = query.Where(f => f.SubmitDate >= fromDate.Value);
                }
                
                if (toDate.HasValue)
                {
                    // Add one day to include the entire end date
                    var nextDay = toDate.Value.AddDays(1);
                    query = query.Where(f => f.SubmitDate < nextDay);
                }
                
                // Get the list of room types for filter dropdown
                var roomTypes = await _context.Feedbacks
                    .Select(f => f.RoomType)
                    .Distinct()
                    .ToListAsync();
                
                // Pass data to the view
                ViewBag.RoomTypes = roomTypes;
                ViewBag.SelectedRoomType = roomType;
                ViewBag.MinRating = rating;
                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
                
                // Get the feedback items
                var feedbacks = await query
                    .OrderByDescending(f => f.SubmitDate)
                    .ToListAsync();
                
                return View(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading feedback management page");
                return View(new List<Feedback>());
            }
        }
    }
}