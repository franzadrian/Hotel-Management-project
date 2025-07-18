using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRSM.Models;
using HRSM.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace HRSM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly HRSMDbContext _context;
        private readonly ILogger<RoomController> _logger;
        private readonly AdminController _adminController;

        public RoomController(HRSMDbContext context, ILogger<RoomController> logger, AdminController adminController)
        {
            _context = context;
            _logger = logger;
            _adminController = adminController;
        }

        // GET: api/Room
        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                var rooms = await _context.Rooms.ToListAsync();
                return Ok(new { success = true, data = rooms });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all rooms");
                return StatusCode(500, new { success = false, message = "Error retrieving rooms" });
            }
        }

        // GET: api/Room/ByType/{type}
        [HttpGet("ByType/{type}")]
        public async Task<IActionResult> GetRoomsByType(string type)
        {
            try
            {
                if (string.IsNullOrEmpty(type) || (type != "standard" && type != "deluxe" && type != "suite"))
                {
                    return BadRequest(new { success = false, message = "Invalid room type" });
                }

                var rooms = await _context.Rooms
                    .Where(r => r.RoomType == type)
                    .ToListAsync();

                return Ok(new { success = true, data = rooms });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rooms by type");
                return StatusCode(500, new { success = false, message = "Error retrieving rooms" });
            }
        }

        // GET: api/Room/ByStatus/{status}
        [HttpGet("ByStatus/{status}")]
        public async Task<IActionResult> GetRoomsByStatus(string status)
        {
            try
            {
                if (string.IsNullOrEmpty(status) || 
                    (status != "available" && status != "occupied" && 
                     status != "cleaning" && status != "maintenance"))
                {
                    return BadRequest(new { success = false, message = "Invalid room status" });
                }

                var rooms = await _context.Rooms
                    .Where(r => r.Status == status)
                    .ToListAsync();

                return Ok(new { success = true, data = rooms });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rooms by status");
                return StatusCode(500, new { success = false, message = "Error retrieving rooms" });
            }
        }

        // GET: api/Room/{roomNumber}
        [HttpGet("{roomNumber}")]
        public async Task<IActionResult> GetRoomByNumber(string roomNumber)
        {
            try
            {
                var room = await _context.Rooms
                    .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);

                if (room == null)
                {
                    return NotFound(new { success = false, message = "Room not found" });
                }

                return Ok(new { success = true, data = room });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving room by number");
                return StatusCode(500, new { success = false, message = "Error retrieving room" });
            }
        }

        // PUT: api/Room/{roomNumber}/Status
        [HttpPut("{roomNumber}/Status")]
        public async Task<IActionResult> UpdateRoomStatus(string roomNumber, [FromBody] string newStatus)
        {
            try
            {
                if (string.IsNullOrEmpty(newStatus) || 
                    (newStatus != "available" && newStatus != "occupied" && 
                     newStatus != "cleaning" && newStatus != "maintenance"))
                {
                    return BadRequest(new { success = false, message = "Invalid room status" });
                }

                var room = await _context.Rooms
                    .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);

                if (room == null)
                {
                    return NotFound(new { success = false, message = "Room not found" });
                }

                string oldStatus = room.Status;

                // Get staff name from claims
                string staffName = "Unknown Staff";
                
                if (User.Identity?.IsAuthenticated == true)
                {
                    var firstName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
                    var lastName = User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value;
                    
                    if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                    {
                        staffName = $"{firstName} {lastName}";
                    }
                    else if (!string.IsNullOrEmpty(firstName))
                    {
                        staffName = firstName;
                    }
                }

                // Update room status
                room.Status = newStatus;
                room.StatusUpdatedAt = DateTime.UtcNow;
                room.UpdatedBy = staffName;

                await _context.SaveChangesAsync();

                // Log room status change activity
                string activityIcon = GetStatusIcon(newStatus);
                await _adminController.LogActivityAsync(
                    $"Room {roomNumber} ({room.RoomType}) status changed from {oldStatus} to {newStatus} by {staffName}",
                    "room",
                    activityIcon,
                    room.RoomId,
                    "/Admin/RoomManagement"
                );

                return Ok(new { success = true, data = room });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating room status");
                return StatusCode(500, new { success = false, message = "Error updating room status" });
            }
        }

        // PUT: api/Room/ResetAllStatuses
        [HttpPut("ResetAllStatuses")]
        public async Task<IActionResult> ResetAllRoomStatuses()
        {
            try
            {
                // Get staff name from claims
                string staffName = "System Reset";
                
                if (User.Identity?.IsAuthenticated == true)
                {
                    var firstName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
                    var lastName = User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value;
                    
                    if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                    {
                        staffName = $"{firstName} {lastName}";
                    }
                    else if (!string.IsNullOrEmpty(firstName))
                    {
                        staffName = firstName;
                    }
                }
                
                // Get all rooms
                var rooms = await _context.Rooms.ToListAsync();

                // Update all rooms to available status
                foreach (var room in rooms)
                {
                    string oldStatus = room.Status;
                    room.Status = "available";
                    room.StatusUpdatedAt = DateTime.UtcNow;
                    room.UpdatedBy = staffName + " (Global Reset)";
                }

                await _context.SaveChangesAsync();

                // Log global room reset activity
                await _adminController.LogActivityAsync(
                    $"All room statuses have been reset to Available by {staffName}",
                    "room",
                    "fas fa-sync-alt",
                    null,
                    "/Admin/RoomManagement"
                );

                return Ok(new { success = true, message = "All room statuses have been reset to Available" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting all room statuses");
                return StatusCode(500, new { success = false, message = "Error resetting room statuses" });
            }
        }

        // Helper method to get appropriate icon for room status
        private string GetStatusIcon(string status)
        {
            return status.ToLower() switch
            {
                "available" => "fas fa-check-circle",
                "occupied" => "fas fa-user",
                "cleaning" => "fas fa-broom",
                "maintenance" => "fas fa-tools",
                _ => "fas fa-door-open"
            };
        }
    }
} 