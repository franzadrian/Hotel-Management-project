using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRSM.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HRSM.Models;

namespace HRSM.Controllers
{
    public class SidebarController : Controller
    {
        private readonly HRSMDbContext _context;
        private readonly ILogger<SidebarController> _logger;

        public SidebarController(HRSMDbContext context, ILogger<SidebarController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        [Authorize]
        public async Task<IActionResult> ServiceRequest()
        {
            // Get the current user's ID
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if the user has an active check-in (booking with status "CheckedIn")
            var activeBooking = await _context.Bookings
                .Where(b => b.UserId == userIdInt && b.Status == "CheckedIn")
                .FirstOrDefaultAsync();

            // Set ViewBag values based on results
            ViewBag.IsCheckedIn = (activeBooking != null);
            
            if (activeBooking != null)
            {
                ViewBag.UserRoom = activeBooking.RoomNumber;
                ViewBag.RoomType = activeBooking.RoomType;
            }

            // Get the user's recent service requests
            var recentRequests = await _context.ServiceRequests
                .Where(r => r.UserId == userIdInt)
                .OrderByDescending(r => r.RequestDate)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentRequests = recentRequests;

            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult PaymentInvoice()
        {
            return View();
        }
        
        public async Task<IActionResult> Event()
        {
            try
            {
                // Get all active events ordered by display order - explicitly setting IsActive = true
                var events = await _context.Events
                    .Where(e => e.IsActive == true)
                    .OrderBy(e => e.DisplayOrder)
                    .ToListAsync();
                
                // Log the number of events found for troubleshooting
                _logger.LogInformation($"Found {events.Count} active events for display");
                
                // If no events are found, create default ones
                if (events == null || events.Count == 0)
                {
                    _logger.LogWarning("No active events found in database. Using default events for SidebarController.");
                    
                    // Get dates for next few weeks
                    var nextWeek = DateTime.Today.AddDays(7);
                    var twoWeeksOut = DateTime.Today.AddDays(14);
                    var threeWeeksOut = DateTime.Today.AddDays(21);
                    
                    events = new List<Event>
                    {
                        new Event
                        {
                            EventId = -1,
                            Title = "FINE DINE",
                            Location = "Dining Area",
                            Description = "Fine Dine",
                            EventDate = nextWeek.ToString("yyyy-MM-dd"),
                            Participants = "Everyone in Hotel",
                            Note = "P.S. You Can Bring Your Love One's",
                            ImagePath = "/images/evepic1.png",
                            DisplayOrder = 1,
                            IsActive = true
                        },
                        new Event
                        {
                            EventId = -2,
                            Title = "FREE DRINKS",
                            Location = "Bar Area",
                            Description = "Free Drinks",
                            EventDate = twoWeeksOut.ToString("yyyy-MM-dd"),
                            Participants = "Everyone in Hotel",
                            Note = "P.S. You Can Bring Your Love One's",
                            ImagePath = "/images/evepic2.png",
                            DisplayOrder = 2,
                            IsActive = true
                        },
                        new Event
                        {
                            EventId = -3,
                            Title = "POOL PARTY",
                            Location = "Rooftop Pool",
                            Description = "Pool Party",
                            EventDate = threeWeeksOut.ToString("yyyy-MM-dd"),
                            Participants = "Everyone in Hotel",
                            Note = "P.S. You Can Bring Your Love One's",
                            ImagePath = "/images/evepic3.png",
                            DisplayOrder = 3,
                            IsActive = true
                        }
                    };
                }
                
                // Ensure all events have properly formatted dates
                foreach (var evt in events)
                {
                    if (evt.EventDate == null || !DateTime.TryParse(evt.EventDate, out _))
                    {
                        // If date is not parseable, set it to a date two weeks from now
                        evt.EventDate = DateTime.Today.AddDays(14).ToString("yyyy-MM-dd");
                    }
                }
                
                // Set default page title
                string pageTitle = "Events";
                
                // First check the database for the page title setting
                var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == "EventPageTitle");
                if (setting != null && !string.IsNullOrWhiteSpace(setting.Value))
                {
                    pageTitle = setting.Value;
                }
                // If not found in database, check cookies as fallback
                else if (Request.Cookies.ContainsKey("EventPageTitle"))
                {
                    var cookieTitle = Request.Cookies["EventPageTitle"];
                    if (!string.IsNullOrWhiteSpace(cookieTitle))
                    {
                        pageTitle = cookieTitle.Trim();
                        
                        // Save to database for future use
                        if (setting == null)
                        {
                            _context.Settings.Add(new Settings
                            {
                                Key = "EventPageTitle",
                                Value = pageTitle,
                                Description = "Title displayed on the Events page",
                                LastUpdated = DateTime.UtcNow
                            });
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                
                ViewBag.EventPageTitle = pageTitle;
                
                return View(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading events page");
                return View(new List<Event>());
            }
        }

        [Authorize]
        [HttpGet("Sidebar/ReviewFeedback")]
        public async Task<IActionResult> ReviewFeedback(int page = 1)
        {
            // Get the current user's ID
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if the user has an active check-in or recent check-out
            var activeBooking = await _context.Bookings
                .Where(b => b.UserId == userIdInt && (b.Status == "CheckedIn" || b.Status == "CheckedOut"))
                .OrderByDescending(b => b.CheckOutDate)
                .FirstOrDefaultAsync();

            // Set ViewBag values based on results
            ViewBag.CanSubmitFeedback = (activeBooking != null);
            ViewBag.StayStatus = activeBooking?.Status ?? "";
            
            // Get all available room types
            var roomTypes = await _context.Rooms
                .Select(r => r.RoomType)
                .Distinct()
                .ToListAsync();
            
            ViewBag.RoomTypes = roomTypes;
            
            // Add the user's current room type to ViewBag
            if (activeBooking != null)
            {
                // Get the room type directly from the booking
                ViewBag.UserRoomType = activeBooking.RoomType;
            }
            else
            {
                ViewBag.UserRoomType = "";
            }

            // Pagination setup
            const int pageSize = 2; // Display 2 items per page
            
            // Count total feedback items
            var totalItems = await _context.Feedbacks.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            // Ensure page is valid
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            // Get feedbacks for the current page
            var allFeedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .OrderByDescending(f => f.SubmitDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Feedbacks = allFeedbacks;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View();
        }

        [Authorize]
        public IActionResult RoomBooking()
        {
            return View();
        }
    }
}