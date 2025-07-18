using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRSM.Data;
using HRSM.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace HRSM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestController : ControllerBase
    {
        private readonly HRSMDbContext _context;
        private readonly ILogger<ServiceRequestController> _logger;
        private readonly AdminController _adminController;

        public ServiceRequestController(HRSMDbContext context, ILogger<ServiceRequestController> logger, AdminController adminController)
        {
            _context = context;
            _logger = logger;
            _adminController = adminController;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateServiceRequest([FromBody] ServiceRequestModel model)
        {
            try
            {
                _logger.LogInformation("Received service request: {@ServiceRequest}", model);

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid request data", errors = ModelState });
                }

                // Get the current user ID
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
                {
                    return BadRequest(new { success = false, message = "Unable to identify user" });
                }

                // Get user info for the activity log
                var user = await _context.Users.FindAsync(userIdInt);
                if (user == null)
                {
                    return BadRequest(new { success = false, message = "User not found" });
                }

                // Check if user is checked in
                var activeBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.UserId == userIdInt && b.Status == "CheckedIn");
                
                if (activeBooking == null)
                {
                    return BadRequest(new { success = false, message = "You must be checked in to submit a service request" });
                }

                // Build housekeeping needs string if applicable
                string? housekeepingNeeds = null;
                if (model.RequestType == "housekeeping" && model.HousekeepingNeeds != null && model.HousekeepingNeeds.Any())
                {
                    housekeepingNeeds = string.Join(",", model.HousekeepingNeeds);
                }

                // Create service request
                var serviceRequest = new ServiceRequest
                {
                    UserId = userIdInt,
                    RoomNumber = model.RoomNumber,
                    RequestType = model.RequestType,
                    ServiceCategory = model.ServiceCategory ?? string.Empty,
                    HousekeepingNeeds = housekeepingNeeds,
                    Description = model.Description,
                    RequestDate = DateTime.Now,
                    Status = "Pending"
                };

                // Save the service request
                _context.ServiceRequests.Add(serviceRequest);
                await _context.SaveChangesAsync();
                
                // Log activity
                string requestType = serviceRequest.RequestType;
                string activityIcon = requestType.ToLower() == "maintenance" ? "fas fa-tools" : "fas fa-broom";
                
                await _adminController.LogActivityAsync(
                    $"New {requestType} request created for room {serviceRequest.RoomNumber} by {user.FirstName} {user.LastName}",
                    "service",
                    activityIcon,
                    serviceRequest.RequestId,
                    "/Admin/ServiceRequestManagement"
                );

                // Create notification for the user
                await _adminController.CreateUserNotificationAsync(
                    userIdInt,
                    $"Your {requestType} request for room {serviceRequest.RoomNumber} has been submitted successfully",
                    "service",
                    activityIcon,
                    serviceRequest.RequestId,
                    "/Sidebar/ServiceRequest"
                );
                
                return Ok(new { success = true, message = "Service request created successfully", requestId = serviceRequest.RequestId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting service request");
                return StatusCode(500, new { success = false, message = "Error submitting service request", error = ex.Message });
            }
        }

        [HttpPost("UpdateStatus")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateStatus([FromQuery] int requestId, [FromQuery] string status)
        {
            try
            {
                _logger.LogInformation("Updating service request status: ID={RequestId}, Status={Status}", requestId, status);

                // Validate status
                var validStatuses = new[] { "pending", "inprogress", "completed", "cancelled", "delayed" };
                if (!validStatuses.Contains(status.ToLower()))
                {
                    return BadRequest(new { success = false, message = "Invalid status value" });
                }

                // Get the service request with user info
                var serviceRequest = await _context.ServiceRequests
                    .Include(sr => sr.User)
                    .FirstOrDefaultAsync(sr => sr.RequestId == requestId);
                    
                if (serviceRequest == null)
                {
                    return NotFound(new { success = false, message = "Service request not found" });
                }

                string oldStatus = serviceRequest.Status;

                // Format status properly for display (capitalize first letter)
                string formattedStatus = char.ToUpper(status[0]) + status.Substring(1).ToLower();
                if (formattedStatus == "Inprogress")
                {
                    formattedStatus = "In Progress";
                }

                // Update the status
                serviceRequest.Status = formattedStatus;
                await _context.SaveChangesAsync();

                // Log the status change activity
                string userName = serviceRequest.User != null 
                    ? $"{serviceRequest.User.FirstName} {serviceRequest.User.LastName}" 
                    : "Unknown Guest";
                    
                string requestType = serviceRequest.RequestType.ToLower() == "maintenance" 
                    ? $"Maintenance ({serviceRequest.ServiceCategory})" 
                    : "Housekeeping";
                
                string icon = serviceRequest.RequestType.ToLower() == "maintenance" 
                    ? "fas fa-tools" 
                    : "fas fa-broom";
                
                await _adminController.LogActivityAsync(
                    $"{requestType} request for Room {serviceRequest.RoomNumber} status changed from {oldStatus} to {formattedStatus}",
                    "service",
                    icon,
                    serviceRequest.RequestId,
                    "/Admin/ServicesManagement"
                );

                return Ok(new { success = true, message = $"Status updated to {formattedStatus}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service request status");
                return StatusCode(500, new { success = false, message = "Error updating status", error = ex.Message });
            }
        }
    }

    public class ServiceRequestModel
    {
        public required string RoomNumber { get; set; }
        public required string RequestType { get; set; } // maintenance or housekeeping
        public string? ServiceCategory { get; set; } // For maintenance: Plumbing, Electrical, etc.
        public List<string>? HousekeepingNeeds { get; set; } // For housekeeping
        public required string Description { get; set; }
    }
} 