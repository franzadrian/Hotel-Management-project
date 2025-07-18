using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRSM.Data;
using HRSM.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace HRSM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly HRSMDbContext _context;
        private readonly ILogger<EventController> _logger;
        private readonly AdminController _adminController;

        public EventController(HRSMDbContext context, ILogger<EventController> logger, AdminController adminController)
        {
            _context = context;
            _logger = logger;
            _adminController = adminController;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            try
            {
                var events = await _context.Events
                    .OrderBy(e => e.DisplayOrder)
                    .ToListAsync();
                
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting events");
                return StatusCode(500, new { success = false, message = "Error retrieving events" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            try
            {
                var evt = await _context.Events.FindAsync(id);
                
                if (evt == null)
                {
                    return NotFound(new { success = false, message = "Event not found" });
                }
                
                // Ensure the event date is in a consistent format
                if (!string.IsNullOrEmpty(evt.EventDate))
                {
                    // Try to parse and format the date consistently
                    if (DateTime.TryParse(evt.EventDate, out DateTime parsedDate))
                    {
                        // Store in standard ISO format (YYYY-MM-DD)
                        evt.EventDate = parsedDate.ToString("yyyy-MM-dd");
                        _logger.LogInformation($"Formatted event date to {evt.EventDate}");
                    }
                }
                
                return Ok(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting event");
                return StatusCode(500, new { success = false, message = "Error retrieving event" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEvent([FromBody] Event eventModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid event data", errors = ModelState });
                }

                _context.Events.Add(eventModel);
                await _context.SaveChangesAsync();
                
                // Log activity
                await _adminController.LogActivityAsync(
                    $"New event '{eventModel.Title}' created",
                    "event",
                    "fas fa-calendar",
                    eventModel.EventId,
                    "/Admin/EventManagement"
                );
                
                return CreatedAtAction(nameof(GetEvent), new { id = eventModel.EventId }, eventModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return StatusCode(500, new { success = false, message = "Error creating event", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event eventModel)
        {
            try
            {
                if (id != eventModel.EventId)
                {
                    return BadRequest(new { success = false, message = "ID mismatch" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid event data", errors = ModelState });
                }

                // Check if event exists
                var existingEvent = await _context.Events.FindAsync(id);
                if (existingEvent == null)
                {
                    return NotFound(new { success = false, message = "Event not found" });
                }
                
                // Update the entity
                _context.Entry(existingEvent).CurrentValues.SetValues(eventModel);
                await _context.SaveChangesAsync();
                
                // Log activity
                await _adminController.LogActivityAsync(
                    $"Event '{eventModel.Title}' updated",
                    "event",
                    "fas fa-edit",
                    eventModel.EventId,
                    "/Admin/EventManagement"
                );
                
                return Ok(new { success = true, message = "Event updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event");
                return StatusCode(500, new { success = false, message = "Error updating event", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var evt = await _context.Events.FindAsync(id);
                if (evt == null)
                {
                    return NotFound(new { success = false, message = "Event not found" });
                }

                // Permanently delete the event instead of just setting it inactive
                _context.Events.Remove(evt);
                await _context.SaveChangesAsync();
                
                // Log activity
                await _adminController.LogActivityAsync(
                    $"Event '{evt.Title}' deleted",
                    "event",
                    "fas fa-trash",
                    id,
                    "/Admin/EventManagement"
                );
                
                return Ok(new { success = true, message = "Event deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event");
                return StatusCode(500, new { success = false, message = "Error deleting event", error = ex.Message });
            }
        }
        
        [HttpPut("reorder")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReorderEvents([FromBody] List<EventOrder> eventOrders)
        {
            try
            {
                foreach (var order in eventOrders)
                {
                    var evt = await _context.Events.FindAsync(order.EventId);
                    if (evt != null)
                    {
                        evt.DisplayOrder = order.DisplayOrder;
                    }
                }
                
                await _context.SaveChangesAsync();
                
                // Log activity
                await _adminController.LogActivityAsync(
                    "Event display order updated",
                    "event",
                    "fas fa-sort",
                    null,
                    "/Admin/EventManagement"
                );
                
                return Ok(new { success = true, message = "Events reordered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reordering events");
                return StatusCode(500, new { success = false, message = "Error reordering events", error = ex.Message });
            }
        }

        [HttpPut("pageTitle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePageTitle([FromBody] PageTitleUpdate titleUpdate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titleUpdate.PageTitle))
                {
                    return BadRequest(new { success = false, message = "Page title cannot be empty" });
                }

                // Make sure we're dealing with a trimmed value
                string trimmedTitle = titleUpdate.PageTitle.Trim();

                // Save to database
                var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == "EventPageTitle");
                if (setting == null)
                {
                    // Create new setting if it doesn't exist
                    setting = new Settings
                    {
                        Key = "EventPageTitle",
                        Value = trimmedTitle,
                        Description = "Title displayed on the Events page",
                        LastUpdated = DateTime.UtcNow
                    };
                    _context.Settings.Add(setting);
                }
                else
                {
                    // Update existing setting
                    setting.Value = trimmedTitle;
                    setting.LastUpdated = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();

                // Store the new title in a cookie as well for redundancy
                Response.Cookies.Append("EventPageTitle", trimmedTitle, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = false,
                    Path = "/",
                    SameSite = SameSiteMode.Lax
                });
                
                // Log activity
                await _adminController.LogActivityAsync(
                    $"Events page title updated to '{trimmedTitle}'",
                    "event",
                    "fas fa-heading",
                    null,
                    "/Admin/EventManagement"
                );
                
                return Ok(new { success = true, message = "Page title updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating page title");
                return StatusCode(500, new { success = false, message = "Error updating page title", error = ex.Message });
            }
        }

        [HttpGet("getCurrentTitle")]
        public async Task<IActionResult> GetCurrentTitle()
        {
            try
            {
                string title = "Events";
                
                // Try to get from database first
                var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == "EventPageTitle");
                if (setting != null && !string.IsNullOrWhiteSpace(setting.Value))
                {
                    title = setting.Value;
                }
                // If not in database, check cookies
                else if (Request.Cookies.ContainsKey("EventPageTitle"))
                {
                    title = Request.Cookies["EventPageTitle"] ?? "Events";
                }
                
                return Ok(new { title });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current title");
                return StatusCode(500, new { success = false, message = "Error retrieving current title" });
            }
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadEvent([FromForm] IFormCollection form, IFormFile? eventImage = null)
        {
            try
            {
                // Log incoming data for debugging
                _logger.LogInformation("Event upload called with form data: {@FormData}", form.Keys.ToDictionary(k => k, k => form[k].ToString()));
                
                // Parse form data
                int eventId;
                
                // Safely parse eventId
                if (!int.TryParse(form["eventId"].ToString(), out eventId))
                {
                    _logger.LogWarning("Invalid eventId format: {EventId}", form["eventId"].ToString());
                    eventId = 0;
                }
                
                bool isNewEvent = eventId == 0;
                
                // Get and format the event date
                string eventDate = form["eventDate"].ToString() ?? string.Empty;
                // Make sure the date is in a consistent format
                if (!string.IsNullOrEmpty(eventDate))
                {
                    if (DateTime.TryParse(eventDate, out DateTime parsedDate))
                    {
                        // Store in standard ISO format (YYYY-MM-DD)
                        eventDate = parsedDate.ToString("yyyy-MM-dd");
                        _logger.LogInformation($"Formatted submitted event date to {eventDate}");
                    }
                    else
                    {
                        _logger.LogWarning($"Could not parse event date: {eventDate}, using as-is");
                    }
                }
                else
                {
                    // Use today's date as fallback
                    eventDate = DateTime.Today.ToString("yyyy-MM-dd");
                    _logger.LogWarning("No event date provided, using today's date");
                }
                
                // Safely parse display order
                int displayOrder;
                if (!int.TryParse(form["displayOrder"].ToString(), out displayOrder))
                {
                    _logger.LogWarning("Invalid displayOrder format: {DisplayOrder}", form["displayOrder"].ToString());
                    displayOrder = 0;
                }
                
                // Check for isActive value
                bool isActive = true; // default to true
                if (form.ContainsKey("isActive"))
                {
                    isActive = form["isActive"].ToString().ToLower() == "true";
                }
                
                // Create event model from form data with safe defaults
                var eventModel = new Event
                {
                    EventId = eventId,
                    Title = form.ContainsKey("title") ? form["title"].ToString() ?? string.Empty : string.Empty,
                    Location = form.ContainsKey("location") ? form["location"].ToString() ?? string.Empty : string.Empty,
                    Description = form.ContainsKey("description") ? form["description"].ToString() ?? string.Empty : string.Empty,
                    EventDate = eventDate, // Use our formatted date
                    Participants = form.ContainsKey("participants") ? form["participants"].ToString() ?? string.Empty : string.Empty,
                    Note = form.ContainsKey("note") ? form["note"].ToString() ?? string.Empty : string.Empty,
                    DisplayOrder = displayOrder,
                    IsActive = isActive
                };
                
                // Process image if provided
                if (eventImage != null && eventImage.Length > 0)
                {
                    // Create uploads directory if it doesn't exist
                    string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "events");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }
                    
                    // Generate unique filename
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(eventImage.FileName)}";
                    string filePath = Path.Combine(uploadsDir, fileName);
                    
                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await eventImage.CopyToAsync(stream);
                    }
                    
                    // Set the image path in the model
                    eventModel.ImagePath = $"/uploads/events/{fileName}";
                }
                else if (isNewEvent)
                {
                    // For new events with no image, use a default
                    eventModel.ImagePath = "/images/evepic1.png";
                }
                else
                {
                    // For updates, keep the existing image path
                    var existingEvent = await _context.Events.FindAsync(eventId);
                    if (existingEvent != null)
                    {
                        eventModel.ImagePath = existingEvent.ImagePath;
                    }
                    else
                    {
                        return NotFound(new { success = false, message = "Event not found" });
                    }
                }
                
                if (isNewEvent)
                {
                    // Add new event
                    _context.Events.Add(eventModel);
                    await _context.SaveChangesAsync();
                    
                    // Log activity
                    await _adminController.LogActivityAsync(
                        $"New event '{eventModel.Title}' created",
                        "event",
                        "fas fa-calendar",
                        eventModel.EventId,
                        "/Admin/EventManagement"
                    );
                    
                    return Ok(new { success = true, eventId = eventModel.EventId });
                }
                else
                {
                    // Update existing event
                    var existingEvent = await _context.Events.FindAsync(eventId);
                    if (existingEvent == null)
                    {
                        return NotFound(new { success = false, message = "Event not found" });
                    }
                    
                    // Update properties
                    existingEvent.Title = eventModel.Title;
                    existingEvent.Location = eventModel.Location;
                    existingEvent.Description = eventModel.Description;
                    existingEvent.EventDate = eventModel.EventDate;
                    existingEvent.Participants = eventModel.Participants;
                    existingEvent.Note = eventModel.Note;
                    existingEvent.DisplayOrder = eventModel.DisplayOrder;
                    existingEvent.IsActive = eventModel.IsActive;
                    
                    // Only update ImagePath if a new image was uploaded
                    if (eventImage != null && eventImage.Length > 0)
                    {
                        existingEvent.ImagePath = eventModel.ImagePath;
                    }
                    
                    await _context.SaveChangesAsync();
                    
                    // Log activity
                    await _adminController.LogActivityAsync(
                        $"Event '{eventModel.Title}' updated",
                        "event",
                        "fas fa-edit",
                        eventModel.EventId,
                        "/Admin/EventManagement"
                    );
                    
                    return Ok(new { success = true });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading event: {ErrorMessage}", ex.Message);
                // Return more detailed error information for debugging
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error processing event data",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("upload/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEventWithUpload(int id, [FromForm] IFormCollection form, IFormFile? eventImage = null)
        {
            // Redirect to the main upload method
            return await UploadEvent(form, eventImage);
        }
    }
    
    public class EventOrder
    {
        public int EventId { get; set; }
        public int DisplayOrder { get; set; }
    }
    
    public class PageTitleUpdate
    {
        public string PageTitle { get; set; } = string.Empty;
    }
} 