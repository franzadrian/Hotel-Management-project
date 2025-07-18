using Microsoft.AspNetCore.Mvc;
using HRSM.Data;
using HRSM.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HRSM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly HRSMDbContext _context;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(HRSMDbContext context, ILogger<NotificationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetNotifications")]
        public async Task<IActionResult> GetNotifications(int count = 5)
        {
            try
            {
                _logger.LogInformation("Fetching notifications");
                
                // Get current user ID if authenticated
                int? userId = null;
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int id))
                    {
                        userId = id;
                    }
                }

                if (!userId.HasValue)
                {
                    // For guests, return empty notifications
                    return Ok(new { success = true, notifications = new List<object>(), totalUnread = 0, totalCount = 0 });
                }

                // For logged-in users, get their specific notifications
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId.Value)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(count)
                    .Select(n => new
                    {
                        n.Id,
                        n.Message,
                        Type = n.Type,
                        n.Icon,
                        n.RelatedId,
                        n.CreatedAt,
                        n.IsRead,
                        n.RelatedUrl
                    })
                    .ToListAsync();

                // Get total unread count
                var totalUnread = await _context.Notifications
                    .Where(n => n.UserId == userId.Value && !n.IsRead)
                    .CountAsync();

                // Get total notification count
                var totalCount = await _context.Notifications
                    .Where(n => n.UserId == userId.Value)
                    .CountAsync();

                return Ok(new { success = true, notifications, totalUnread, totalCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching notifications");
                return StatusCode(500, new { success = false, message = "Error retrieving notifications", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] NotificationCreateModel model)
        {
            try
            {
                _logger.LogInformation("Creating new notification");
                
                if (model == null)
                {
                    return BadRequest(new { success = false, message = "Invalid notification data" });
                }

                // Find the user associated with the service request or feedback
                int? userId = null;
                
                if (model.ServiceRequestId.HasValue)
                {
                    // Find service request
                    var serviceRequest = await _context.ServiceRequests
                        .FirstOrDefaultAsync(sr => sr.RequestId == model.ServiceRequestId.Value);
                    
                    if (serviceRequest != null)
                    {
                        // Find the booking associated with the room
                        var booking = await _context.Bookings
                            .FirstOrDefaultAsync(b => b.RoomNumber == serviceRequest.RoomNumber 
                                && b.Status != "Cancelled" 
                                && b.Status != "Completed");
                        
                        if (booking != null)
                        {
                            userId = booking.UserId;
                        }
                    }
                }
                else if (model.FeedbackId.HasValue)
                {
                    // Find feedback
                    var feedback = await _context.Feedbacks
                        .FirstOrDefaultAsync(f => f.FeedbackId == model.FeedbackId.Value);
                    
                    if (feedback != null)
                    {
                        userId = feedback.UserId;
                    }
                }
                
                // If no user found, log warning and return success (silent fail)
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Could not find user to notify. ServiceRequestId: {ServiceRequestId}, FeedbackId: {FeedbackId}",
                        model.ServiceRequestId, model.FeedbackId);
                    
                    return Ok(new { success = true, message = "No user found to notify" });
                }
                
                // Create notification
                var notification = new Notification
                {
                    UserId = userId.Value,
                    Message = model.Message,
                    Type = model.Type,
                    Icon = model.Icon ?? "fas fa-bell",
                    RelatedId = model.ServiceRequestId ?? model.FeedbackId,
                    CreatedAt = DateTime.Now,
                    IsRead = false,
                    RelatedUrl = model.RelatedUrl
                };
                
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully created notification for user {UserId}", userId.Value);
                return Ok(new { success = true, notificationId = notification.Id, userId = userId.Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating notification");
                return StatusCode(500, new { success = false, message = "Error creating notification", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("{notificationId}/MarkAsRead")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                _logger.LogInformation("Marking notification as read: {NotificationId}", notificationId);
                
                var notification = await _context.Notifications.FindAsync(notificationId);
                
                if (notification == null)
                {
                    _logger.LogWarning("Notification not found with ID: {NotificationId}", notificationId);
                    return NotFound(new { success = false, message = "Notification not found" });
                }

                // Check if user has permission to mark this notification as read
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
                    {
                        if (notification.UserId != userId && !User.IsInRole("Admin"))
                        {
                            _logger.LogWarning("User {UserId} attempted to mark notification {NotificationId} as read but does not have permission", userId, notificationId);
                            return Forbid();
                        }
                    }
                }
                else
                {
                    return Unauthorized();
                }

                notification.IsRead = true;
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully marked notification as read");
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while marking notification as read");
                return StatusCode(500, new { success = false, message = "Error marking notification as read", error = ex.Message });
            }
        }
        
        [HttpPost]
        [Route("MarkAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                _logger.LogInformation("Marking all notifications as read");
                
                if (User.Identity == null || !User.Identity.IsAuthenticated)
                {
                    return Unauthorized();
                }
                
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user" });
                }

                // Get all unread notifications for the user
                var unreadNotifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();
                
                // Mark all as read
                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                }
                
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully marked all notifications as read for user {UserId}", userId);
                return Ok(new { success = true, count = unreadNotifications.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while marking all notifications as read");
                return StatusCode(500, new { success = false, message = "Error marking all notifications as read", error = ex.Message });
            }
        }
        
        [HttpGet]
        [Route("GetAllNotifications")]
        public async Task<IActionResult> GetAllNotifications()
        {
            try
            {
                _logger.LogInformation("Fetching all notifications");
                
                if (User.Identity == null || !User.Identity.IsAuthenticated)
                {
                    return Unauthorized();
                }
                
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user" });
                }

                // Get all notifications for the user
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => new
                    {
                        n.Id,
                        n.Message,
                        Type = n.Type,
                        n.Icon,
                        n.RelatedId,
                        n.CreatedAt,
                        n.IsRead,
                        n.RelatedUrl
                    })
                    .ToListAsync();

                return Ok(new { success = true, notifications });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all notifications");
                return StatusCode(500, new { success = false, message = "Error retrieving all notifications", error = ex.Message });
            }
        }
    }

    // Model for notification creation
    public class NotificationCreateModel
    {
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int? ServiceRequestId { get; set; }
        public int? FeedbackId { get; set; }
        public string? RelatedUrl { get; set; }
    }
} 