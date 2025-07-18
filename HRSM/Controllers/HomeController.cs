using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HRSM.Data;
using HRSM.Models;
using Microsoft.EntityFrameworkCore;

namespace HRSM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HRSMDbContext _context;

        public HomeController(ILogger<HomeController> logger, HRSMDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get active events for the _event partial
                var events = await _context.Events
                    .Where(e => e.IsActive == true)  // Explicitly check e.IsActive is true
                    .OrderBy(e => e.DisplayOrder)
                    .ToListAsync();
                
                // Log the event count for troubleshooting
                _logger.LogInformation($"HomeController.Index: Found {events.Count} active events");
                
                // If no events are found in the database, create a few default ones for display
                if (events == null || !events.Any())
                {
                    _logger.LogWarning("No active events found in database. Using default events for display.");
                    
                    // Get the next Saturday, Sunday, and Monday for the default events
                    var today = DateTime.Today;
                    var nextSaturday = today.AddDays((13 - (int)today.DayOfWeek) % 7);
                    var nextSunday = today.AddDays((14 - (int)today.DayOfWeek) % 7);
                    var nextMonday = today.AddDays((15 - (int)today.DayOfWeek) % 7);
                    
                    events = new List<Event>
                    {
                        new Event
                        {
                            EventId = -1, // Temporary ID
                            Title = "FINE DINE",
                            Location = "Dining Area",
                            Description = "Fine Dine",
                            EventDate = nextSaturday.ToString("yyyy-MM-dd"), // Standard ISO date format
                            Participants = "Everyone in Hotel",
                            Note = "P.S. You Can Bring Your Love One's",
                            ImagePath = "/images/evepic1.png",
                            DisplayOrder = 1,
                            IsActive = true
                        },
                        new Event
                        {
                            EventId = -2, // Temporary ID
                            Title = "FREE DRINKS",
                            Location = "Bar Area",
                            Description = "Free Drinks",
                            EventDate = nextSunday.ToString("yyyy-MM-dd"), // Standard ISO date format
                            Participants = "Everyone in Hotel",
                            Note = "P.S. You Can Bring Your Love One's",
                            ImagePath = "/images/evepic2.png",
                            DisplayOrder = 2,
                            IsActive = true
                        },
                        new Event
                        {
                            EventId = -3, // Temporary ID
                            Title = "POOL PARTY",
                            Location = "Rooftop Pool",
                            Description = "Pool Party",
                            EventDate = nextMonday.ToString("yyyy-MM-dd"), // Standard ISO date format
                            Participants = "Everyone in Hotel",
                            Note = "P.S. You Can Bring Your Love One's",
                            ImagePath = "/images/evepic3.png",
                            DisplayOrder = 3,
                            IsActive = true
                        }
                    };
                }
                
                // Get page title from settings
                var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == "EventPageTitle");
                if (setting != null && !string.IsNullOrWhiteSpace(setting.Value))
                {
                    ViewBag.EventPageTitle = setting.Value;
                }
                
                // Pass the events to ViewBag for the partial view
                ViewBag.Events = events;
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page events");
                
                // Provide fallback events in case of error
                ViewBag.Events = new List<Event>
                {
                    new Event
                    {
                        EventId = -1,
                        Title = "FINE DINE",
                        Description = "Fine dining experience",
                        EventDate = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd"),
                        ImagePath = "/images/evepic1.png",
                        IsActive = true
                    }
                };
                
                return View();
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> Home()
        {
            try
            {
                // Get active events for the _event partial
                var events = await _context.Events
                    .Where(e => e.IsActive == true)
                    .OrderBy(e => e.DisplayOrder)
                    .ToListAsync();
                
                // Log the event count for troubleshooting
                _logger.LogInformation($"HomeController.Home: Found {events.Count} active events");
                
                // If no events are found in the database, create a few default ones for display
                if (events == null || !events.Any())
                {
                    _logger.LogWarning("No active events found in database for Home view. Using default events.");
                    
                    // Get the next Saturday, Sunday, and Monday for the default events
                    var today = DateTime.Today;
                    var nextSaturday = today.AddDays((13 - (int)today.DayOfWeek) % 7);
                    var nextSunday = today.AddDays((14 - (int)today.DayOfWeek) % 7);
                    var nextMonday = today.AddDays((15 - (int)today.DayOfWeek) % 7);
                    
                    events = new List<Event>
                    {
                        new Event
                        {
                            EventId = -1,
                            Title = "FINE DINE",
                            Location = "Dining Area",
                            Description = "Fine Dine",
                            EventDate = nextSaturday.ToString("yyyy-MM-dd"),
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
                            EventDate = nextSunday.ToString("yyyy-MM-dd"),
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
                            EventDate = nextMonday.ToString("yyyy-MM-dd"),
                            Participants = "Everyone in Hotel",
                            Note = "P.S. You Can Bring Your Love One's",
                            ImagePath = "/images/evepic3.png",
                            DisplayOrder = 3,
                            IsActive = true
                        }
                    };
                }
                
                // Get page title from settings
                var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == "EventPageTitle");
                if (setting != null && !string.IsNullOrWhiteSpace(setting.Value))
                {
                    ViewBag.EventPageTitle = setting.Value;
                }
                
                // Pass the events to ViewBag for the partial view
                ViewBag.Events = events;
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page events in Home action");
                
                // Provide fallback events in case of error
                ViewBag.Events = new List<Event>
                {
                    new Event
                    {
                        EventId = -1,
                        Title = "FINE DINE",
                        Description = "Fine dining experience",
                        EventDate = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd"),
                        ImagePath = "/images/evepic1.png",
                        IsActive = true
                    }
                };
                
                return View();
            }
        }

        // Temporary action to access room management
        public IActionResult RoomManagement()
        {
            return View("~/Views/Staff/RoomManagement.cshtml");
        }
    }
}
