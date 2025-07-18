using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRSM.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using HRSM.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Collections.Generic;

namespace HRSM.Controllers
{
    public class StaffController : Controller
    {
        private readonly HRSMDbContext _context;
        private readonly ILogger<StaffController> _logger;

        public StaffController(HRSMDbContext context, ILogger<StaffController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Staff/Dashboard
        [Authorize(Roles = "Staff")]
        public IActionResult Dashboard()
        {
            try
            {
                // Get current staff member info
                var userId = 0;
                var userIdString = User.FindFirst("UserId")?.Value;
                
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out userId))
                {
                    _logger.LogWarning("Unable to parse userId from claims");
                    return RedirectToAction("Login", "Account");
                }

                // Get department from claims
                var department = User.FindFirst("Department")?.Value ?? "Receptionist";
                
                // Redirect to appropriate dashboard based on department
                switch (department)
                {
                    case "Housekeeping":
                        return RedirectToAction("HousekeepingManagement");
                    case "Maintenance":
                        return RedirectToAction("MaintenanceManagement");
                    default: // Receptionist
                        return RedirectToAction("ReceptionistDashboard");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff dashboard");
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: /Staff/ReceptionistDashboard
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> ReceptionistDashboard()
        {
            try
            {
                // Get current staff member info
                var userId = 0;
                var userIdString = User.FindFirst("UserId")?.Value;
                
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out userId))
                {
                    _logger.LogWarning("Unable to parse userId from claims");
                    return RedirectToAction("Login", "Account");
                }

                var staff = await _context.Users.FindAsync(userId);
                
                if (staff == null)
                {
                    _logger.LogWarning("Staff user with ID {UserId} not found", userId);
                    return RedirectToAction("Login", "Account");
                }

                // Gather reservation statistics
                var today = DateTime.Today;
                
                // Count confirmed reservations
                var confirmedReservations = await _context.Bookings
                    .Where(b => b.Status == "Confirmed")
                    .CountAsync();
                
                // Count checked-in reservations
                var checkedInReservations = await _context.Bookings
                    .Where(b => b.Status == "CheckedIn")
                    .CountAsync();
                
                // Count checked-out reservations
                var checkedOutReservations = await _context.Bookings
                    .Where(b => b.Status == "CheckedOut")
                    .CountAsync();
                
                // Count pending reservations
                var pendingReservations = await _context.Bookings
                    .Where(b => b.Status == "Pending")
                    .CountAsync();
                
                // Today's check-ins: All bookings with check-in date = today
                var todayCheckIns = await _context.Bookings
                    .Where(b => b.CheckInDate.Date == today)
                    .CountAsync();
                
                // Today's check-outs: All bookings with check-out date = today
                var todayCheckOuts = await _context.Bookings
                    .Where(b => b.CheckOutDate.Date == today)
                    .CountAsync();

                // For debugging
                _logger.LogInformation("Today's Check-ins count: {count}", todayCheckIns);
                _logger.LogInformation("Today's Check-outs count: {count}", todayCheckOuts);
                
                ViewBag.ConfirmedReservations = confirmedReservations;
                ViewBag.CheckedInReservations = checkedInReservations;
                ViewBag.CheckedOutReservations = checkedOutReservations;
                ViewBag.PendingReservations = pendingReservations;
                ViewBag.TodayCheckIns = todayCheckIns;
                ViewBag.TodayCheckOuts = todayCheckOuts;
                ViewBag.StaffName = $"{staff.FirstName} {staff.LastName}";

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading receptionist dashboard: {Message}", ex.Message);
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: /Staff/RoomManagement
        [Authorize(Roles = "Staff")]
        public IActionResult RoomManagement()
        {
            return View();
        }

        // GET: /Staff/MaintenanceManagement
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> MaintenanceManagement()
        {
            try
            {
                // Get maintenance requests from database
                var maintenanceRequests = await _context.ServiceRequests
                    .Where(sr => sr.RequestType == "Maintenance")
                    .OrderByDescending(sr => sr.RequestDate)
                    .ToListAsync();

                return View(maintenanceRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading maintenance requests: {Message}", ex.Message);
                return View(new List<ServiceRequest>());
            }
        }

        // GET: /Staff/HousekeepingManagement
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> HousekeepingManagement()
        {
            try
            {
                // Get housekeeping tasks from database
                var housekeepingTasks = await _context.ServiceRequests
                    .Where(sr => sr.RequestType == "Housekeeping")
                    .OrderByDescending(sr => sr.RequestDate)
                    .ToListAsync();

                return View(housekeepingTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading housekeeping tasks: {Message}", ex.Message);
                return View(new List<ServiceRequest>());
            }
        }

        // Admin-only actions below this point
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var staffList = await _context.Users
                .Where(u => u.Role == "Staff")
                .ToListAsync();
            return View(staffList);
        }

        // GET: /Staff/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new Staff());
        }

        // POST: /Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] Staff staff)
        {
            try
            {
                // Validate passwords match
                if (staff.Password != staff.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                    return View(staff);
                }

                ModelState.Remove("ConfirmPassword");

                if (!ModelState.IsValid)
                {
                    return View(staff);
                }

                // Make sure role is set to Staff
                staff.Role = "Staff";
                
                // Check email uniqueness
                if (await _context.Users.AnyAsync(u => u.Email == staff.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(staff);
                }

                _context.Users.Add(staff);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Staff member added successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff member");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the staff member");
                return View(staff);
            }
        }

        // GET: /Staff/Edit/{id}
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.Role != "Staff")
            {
                return NotFound();
            }

            // Create a Staff object from the User
            var staff = new Staff
            {
                User_Id = user.User_Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Contact = user.Contact,
                Role = user.Role,
                ProfilePicturePath = user.ProfilePicturePath,
                MemberSince = user.MemberSince
            };

            // Get staff-specific properties from shadow properties
            staff.Department = _context.Entry(user).Property<string>("Department").CurrentValue ?? string.Empty;
            
            var hireDate = _context.Entry(user).Property<DateTime?>("HireDate").CurrentValue;
            if (hireDate.HasValue)
            {
                staff.HireDate = hireDate.Value;
            }

            return View(staff);
        }

        // POST: /Staff/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [FromForm] Staff updatedStaff)
        {
            try
            {
                if (id != updatedStaff.User_Id)
                {
                    return Request.Headers["X-Requested-With"] == "XMLHttpRequest" 
                        ? Json(new { success = false, message = "Invalid staff ID" })
                        : NotFound();
                }

                ModelState.Remove("ConfirmPassword");
                
                if (!ModelState.IsValid)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Please correct the validation errors", errors = ModelState });
                    }
                    return View(updatedStaff);
                }

                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null || existingUser.Role != "Staff")
                {
                    return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                        ? Json(new { success = false, message = "Staff member not found" })
                        : NotFound();
                }

                // Check email uniqueness
                if (existingUser.Email != updatedStaff.Email && 
                    await _context.Users.AnyAsync(u => u.Email == updatedStaff.Email))
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Email already exists" });
                    }
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(updatedStaff);
                }

                // Update base User properties
                existingUser.FirstName = updatedStaff.FirstName;
                existingUser.LastName = updatedStaff.LastName;
                existingUser.Contact = updatedStaff.Contact;
                existingUser.Email = updatedStaff.Email;
                
                // Only update password if provided
                if (!string.IsNullOrEmpty(updatedStaff.Password))
                {
                    existingUser.Password = updatedStaff.Password;
                }

                // Update Staff-specific properties using shadow properties
                _context.Entry(existingUser).Property("Department").CurrentValue = updatedStaff.Department;
                _context.Entry(existingUser).Property("HireDate").CurrentValue = updatedStaff.HireDate;

                await _context.SaveChangesAsync();
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    Response.Headers["Content-Type"] = "application/json";
                    return Json(new { success = true, message = "Staff information updated successfully" });
                }
                
                TempData["SuccessMessage"] = "Staff information updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff member: {Message}", ex.Message);
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    Response.Headers["Content-Type"] = "application/json";
                    return Json(new { success = false, message = $"An error occurred while updating the staff member: {ex.Message}" });
                }
                
                ModelState.AddModelError(string.Empty, "An error occurred while updating the staff member");
                return View(updatedStaff);
            }
        }

        // GET: /Staff/Details/{id}
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.Role != "Staff")
            {
                return NotFound();
            }

            // Create a Staff object from the User
            var staff = new Staff
            {
                User_Id = user.User_Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Contact = user.Contact,
                Role = user.Role,
                ProfilePicturePath = user.ProfilePicturePath,
                MemberSince = user.MemberSince
            };

            // Get staff-specific properties from shadow properties
            staff.Department = _context.Entry(user).Property<string>("Department").CurrentValue ?? string.Empty;
            
            var hireDate = _context.Entry(user).Property<DateTime?>("HireDate").CurrentValue;
            if (hireDate.HasValue)
            {
                staff.HireDate = hireDate.Value;
            }

            return View(staff);
        }

        // POST: /Staff/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null || user.Role != "Staff")
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        Response.Headers["Content-Type"] = "application/json";
                        return Json(new { success = false, message = "Staff member not found" });
                    }
                    return NotFound();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    Response.Headers["Content-Type"] = "application/json";
                    return Json(new { success = true, message = "Staff member deleted successfully" });
                }
                
                TempData["SuccessMessage"] = "Staff member deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff member: {Message}", ex.Message);
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    Response.Headers["Content-Type"] = "application/json";
                    return Json(new { success = false, message = $"An error occurred while deleting the staff member: {ex.Message}" });
                }
                
                TempData["ErrorMessage"] = "An error occurred while deleting the staff member";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Staff/GetDetails/{id} - AJAX endpoint for staff details modal
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDetails(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null || user.Role != "Staff")
                {
                    return NotFound();
                }

                // Create a Staff object from the User
                var staff = new Staff
                {
                    User_Id = user.User_Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Contact = user.Contact,
                    Role = user.Role,
                    ProfilePicturePath = user.ProfilePicturePath,
                    MemberSince = user.MemberSince
                };

                // Get staff-specific properties from shadow properties
                staff.Department = _context.Entry(user).Property<string>("Department").CurrentValue ?? string.Empty;
                
                var hireDate = _context.Entry(user).Property<DateTime?>("HireDate").CurrentValue;
                if (hireDate.HasValue)
                {
                    staff.HireDate = hireDate.Value;
                }

                return PartialView("~/Views/Admin/_StaffDetails.cshtml", staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching staff details for AJAX modal");
                return Content("Error loading staff details. Please try again later.");
            }
        }

        // GET: /Staff/GetEditForm/{id} - AJAX endpoint for edit staff modal
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEditForm(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null || user.Role != "Staff")
                {
                    return NotFound();
                }

                // Create a Staff object from the User
                var staff = new Staff
                {
                    User_Id = user.User_Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Contact = user.Contact,
                    Role = user.Role,
                    ProfilePicturePath = user.ProfilePicturePath,
                    MemberSince = user.MemberSince
                };

                // Get staff-specific properties from shadow properties
                staff.Department = _context.Entry(user).Property<string>("Department").CurrentValue ?? string.Empty;
                
                var hireDate = _context.Entry(user).Property<DateTime?>("HireDate").CurrentValue;
                if (hireDate.HasValue)
                {
                    staff.HireDate = hireDate.Value;
                }

                return PartialView("~/Views/Admin/_EditStaff.cshtml", staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching staff edit form for AJAX modal");
                return Content("Error loading edit form. Please try again later.");
            }
        }

        // GET: /Staff/StaffInfo
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> StaffInfo()
        {
            try
            {
                // Get current staff member info
                var userId = 0;
                var userIdString = User.FindFirst("UserId")?.Value;
                
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out userId))
                {
                    _logger.LogWarning("Unable to parse userId from claims");
                    return RedirectToAction("Login", "Account");
                }

                var staff = await _context.Users.FindAsync(userId);
                
                if (staff == null)
                {
                    _logger.LogWarning("Staff user with ID {UserId} not found", userId);
                    return RedirectToAction("Login", "Account");
                }

                // Convert to Staff model to access staff-specific properties
                var staffModel = new Staff
                {
                    User_Id = staff.User_Id,
                    FirstName = staff.FirstName ?? "Unknown",
                    LastName = staff.LastName ?? "User",
                    Email = staff.Email ?? "",
                    Contact = staff.Contact ?? "",
                    ProfilePicturePath = staff.ProfilePicturePath ?? "",
                    MemberSince = staff.MemberSince
                };

                // Get staff-specific properties from shadow properties with null handling
                try
                {
                    staffModel.Department = _context.Entry(staff).Property<string>("Department").CurrentValue ?? "Not Assigned";
                    
                    var hireDate = _context.Entry(staff).Property<DateTime?>("HireDate").CurrentValue;
                    if (hireDate.HasValue)
                    {
                        staffModel.HireDate = hireDate.Value;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving staff-specific properties for user {UserId}", userId);
                    // Use default values from Staff model
                }

                return View(staffModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff profile page: {Message}", ex.Message);
                return RedirectToAction("Login", "Account");
            }
        }

        // POST: /Staff/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateProfile(Staff model, IFormFile profilePicture)
        {
            try
            {
                var userIdString = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                // Update basic user properties
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Contact = model.Contact;

                // Handle profile picture upload
                if (profilePicture != null && profilePicture.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(profilePicture.FileName).ToLower();
                    
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ProfilePicture", "Only JPG, JPEG, PNG, and GIF images are allowed.");
                        return View("StaffInfo", model);
                    }

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(fileStream);
                    }

                    if (!string.IsNullOrEmpty(user.ProfilePicturePath) && 
                        !user.ProfilePicturePath.Equals("/images/default-profile.png", StringComparison.OrdinalIgnoreCase))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
                            user.ProfilePicturePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    user.ProfilePicturePath = $"/images/profiles/{uniqueFileName}";
                }

                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Profile updated successfully";
                return RedirectToAction("StaffInfo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff profile: {Message}", ex.Message);
                ModelState.AddModelError("", "An error occurred while updating your profile.");
                return View("StaffInfo", model);
            }
        }
    }
} 