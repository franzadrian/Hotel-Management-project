using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRSM.Models;
using HRSM.ViewModels;
using HRSM.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace HRSM.Controllers
{
    public class AccountController : Controller
    {
        private readonly HRSMDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<AccountController> _logger;
        private readonly AdminController _adminController;
        private const int MaxFileSize = 5 * 1024 * 1024; // 5MB
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public AccountController(HRSMDbContext context, IWebHostEnvironment hostingEnvironment, ILogger<AccountController> logger, AdminController adminController)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _adminController = adminController;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                _logger.LogInformation("Starting registration process for email: {Email}", user.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state: {@ModelErrors}", 
                        ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                    return View(user);
                }

                if (user.Password != user.ConfirmPassword)
                {
                    _logger.LogWarning("Password mismatch for email: {Email}", user.Email);
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                    return View(user);
                }

                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Email already exists: {Email}", user.Email);
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(user);
                }

                // Set default values
                user.Role = "Guest";
                user.MemberSince = DateTime.Now;
                user.ProfilePicturePath = "/images/default-profile.png";

                _logger.LogInformation("Adding new user to database: {@UserData}", new { 
                    user.FirstName, 
                    user.LastName, 
                    user.Email, 
                    user.Role 
                });

                // Add user to database
                await _context.Users.AddAsync(user);

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User successfully registered: {Email}", user.Email);
                    
                    // Log the registration activity
                    await _adminController.LogActivityAsync(
                        $"New user registered: {user.FirstName} {user.LastName} ({user.Email})",
                        "user",
                        "fas fa-user-plus",
                        user.User_Id,
                        "/Admin/UserManagement"
                    );
                    
                    return RedirectToAction("Login", "Account");
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Database error while saving user: {Message}", dbEx.InnerException?.Message ?? dbEx.Message);
                    ModelState.AddModelError("", $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration: {Message}", ex.Message);
                ModelState.AddModelError("", $"Registration error: {ex.Message}");
                return View(user);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

                if (user != null && user.Password == model.Password)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.User_Id.ToString()),
                        new Claim("UserId", user.User_Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.GivenName, user.FirstName),
                        new Claim(ClaimTypes.Surname, user.LastName),
                        new Claim("Contact", user.Contact),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("ProfilePicture", user.ProfilePicturePath ?? "/images/default-profile.png")
                    };

                    // For staff users, get department information
                    string department = "";
                    if (user.Role == "Staff")
                    {
                        try
                        {
                            department = _context.Entry(user).Property<string>("Department").CurrentValue ?? "Receptionist";
                            claims.Add(new Claim("Department", department));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error retrieving department for staff user {UserId}", user.User_Id);
                            department = "Receptionist"; // Default to Receptionist if error
                        }
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                        
                    // Log the login activity
                    string icon = user.Role == "Admin" ? "fas fa-user-shield" : 
                                 (user.Role == "Staff" ? "fas fa-user-tie" : "fas fa-user");
                    
                    await _adminController.LogActivityAsync(
                        $"User login: {user.FirstName} {user.LastName} ({user.Role})",
                        "authentication",
                        icon,
                        user.User_Id,
                        user.Role == "Admin" ? "/Admin/AdminDashboard" : 
                            (user.Role == "Staff" ? "/Staff/ReceptionistDashboard" : null)
                    );

                    // Redirect based on user role
                    switch (user.Role)
                    {
                        case "Admin":
                            return RedirectToAction("AdminDashboard", "Admin");
                        case "Staff":
                            // Redirect based on department
                            switch (department)
                            {
                                case "Housekeeping":
                                    return RedirectToAction("HousekeepingManagement", "Staff");
                                case "Maintenance":
                                    return RedirectToAction("MaintenanceManagement", "Staff");
                                default:
                                    return RedirectToAction("ReceptionistDashboard", "Staff");
                            }
                        default:
                            return RedirectToAction("Home", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> UserInfo()
        {
            try
            {
                _logger.LogInformation("Starting UserInfo page load");
                
                // Get user ID from claims
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    _logger.LogWarning("Invalid user ID in claims");
                    return RedirectToAction("Login");
                }

                // Load user without including bookings
                var user = await _context.Users.FirstOrDefaultAsync(u => u.User_Id == userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", userId);
                    return NotFound();
                }

                // Load bookings separately
                var currentDate = DateTime.Now.Date;
                var bookings = await _context.Bookings
                    .AsNoTracking()
                    .Where(b => b.UserId == userId)
                    .ToListAsync();

                var activeReservations = new List<Booking>();
                var pastReservations = new List<Booking>();

                // Manual separation of bookings
                foreach (var booking in bookings)
                {
                    var status = (booking.Status ?? "").ToLower().Replace(" ", "");
                    var isCheckout = status == "checkout" || status == "checkedout";
                    var isPastDate = booking.CheckOutDate.Date < currentDate;

                    if (status == "cancelled" || isCheckout || isPastDate)
                    {
                        pastReservations.Add(booking);
                    }
                    else
                    {
                        activeReservations.Add(booking);
                    }
                }

                // Sort the lists
                activeReservations = activeReservations
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();
                pastReservations = pastReservations
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();

                ViewBag.ActiveReservations = activeReservations;
                ViewBag.PastReservations = pastReservations;

                _logger.LogInformation("Successfully loaded UserInfo page for user {UserId}", userId);
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading UserInfo page: {Message}", ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(User model, IFormFile profilePicture)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return RedirectToAction("Login");
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Contact = model.Contact;

                if (profilePicture != null && profilePicture.Length > 0)
                {
                    var extension = Path.GetExtension(profilePicture.FileName).ToLower();
                    if (!AllowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ProfilePicture", "Only JPG, JPEG, PNG, and GIF images are allowed.");
                        return await GetErrorView(userId);
                    }

                    if (profilePicture.Length > MaxFileSize)
                    {
                        ModelState.AddModelError("ProfilePicture", "File size exceeds 5MB limit.");
                        return await GetErrorView(userId);
                    }

                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "profiles");
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
                        var oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, 
                            user.ProfilePicturePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    user.ProfilePicturePath = $"/images/profiles/{uniqueFileName}";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("UserInfo");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating your profile.");
                
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return RedirectToAction("Login");
                }
                
                return await GetErrorView(userId);
            }
        }

        private async Task<IActionResult> GetErrorView(int userId)
        {
            var currentUser = await _context.Users.FindAsync(userId);
            return View("UserInfo", currentUser ?? new User());
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var userName = $"{User.FindFirstValue(ClaimTypes.GivenName)} {User.FindFirstValue(ClaimTypes.Surname)}";

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                
                // Log the logout activity if we have the user ID
                if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
                {
                    string icon = userRole == "Admin" ? "fas fa-user-shield" : 
                                 (userRole == "Staff" ? "fas fa-user-tie" : "fas fa-user");
                    
                    await _adminController.LogActivityAsync(
                        $"User logout: {userName} ({userRole})",
                        "authentication",
                        icon,
                        userIdInt,
                        null
                    );
                }
                
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return RedirectToAction("Login");
            }
        }
        
        [Authorize]
        public async Task<IActionResult> Notifications()
        {
            try
            {
                // Get user ID from claims
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return RedirectToAction("Login");
                }

                // Get all notifications for the user
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                return View(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading notifications");
                return RedirectToAction("UserInfo");
            }
        }
    }
}