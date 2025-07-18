using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRSM.Data;
using HRSM.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace HRSM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly HRSMDbContext _context;
        private readonly ILogger<FeedbackController> _logger;
        private readonly AdminController _adminController;

        public FeedbackController(HRSMDbContext context, ILogger<FeedbackController> logger, AdminController adminController)
        {
            _context = context;
            _logger = logger;
            _adminController = adminController;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackModel model)
        {
            try
            {
                _logger.LogInformation("Received feedback: {@Feedback}", model);

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid feedback data", errors = ModelState });
                }

                // Get the current user ID
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
                {
                    return BadRequest(new { success = false, message = "Unable to identify user" });
                }

                // Get the user for activity logging
                var user = await _context.Users.FindAsync(userIdInt);
                if (user == null)
                {
                    return BadRequest(new { success = false, message = "User not found" });
                }

                // Check if user has a valid booking (checked in or recently checked out)
                var activeBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.UserId == userIdInt && 
                                         (b.Status == "CheckedIn" || b.Status == "CheckedOut"));
                
                if (activeBooking == null)
                {
                    return BadRequest(new { success = false, message = "You must be checked in or have recently checked out to submit feedback" });
                }

                // Create feedback
                var feedback = new Feedback
                {
                    UserId = userIdInt,
                    RoomType = model.RoomType,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    SubmitDate = DateTime.Now,
                    StayType = activeBooking.Status // "CheckedIn" or "CheckedOut"
                };

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                // Log the feedback submission activity
                string userName = $"{user.FirstName} {user.LastName}";
                string ratingStars = new string('★', model.Rating) + new string('☆', 5 - model.Rating);
                string commentSummary = model.Comment.Length > 50 ? model.Comment.Substring(0, 50) + "..." : model.Comment;

                await _adminController.LogActivityAsync(
                    $"New {model.Rating}-star feedback for {model.RoomType} room from {userName}: \"{commentSummary}\"",
                    "feedback",
                    GetFeedbackIcon(model.Rating),
                    feedback.FeedbackId,
                    "/Admin/FeedbackManagement"
                );

                // Create notification for the user
                await _adminController.CreateUserNotificationAsync(
                    userIdInt,
                    $"Thank you for submitting your {model.Rating}-star feedback for {model.RoomType} room",
                    "feedback",
                    GetFeedbackIcon(model.Rating),
                    feedback.FeedbackId,
                    "/Sidebar/ReviewFeedback"
                );

                return Ok(new { success = true, message = "Feedback submitted successfully", feedbackId = feedback.FeedbackId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting feedback");
                return StatusCode(500, new { success = false, message = "Error submitting feedback", error = ex.Message });
            }
        }

        // Helper method to get appropriate icon based on rating
        private string GetFeedbackIcon(int rating)
        {
            return rating switch
            {
                5 => "fas fa-star",
                4 => "fas fa-star-half-alt",
                _ => "far fa-star"
            };
        }
        
        [HttpPost("MarkRead/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkFeedbackAsRead(int id)
        {
            try
            {
                var feedback = await _context.Feedbacks.FindAsync(id);
                if (feedback == null)
                {
                    return NotFound(new { success = false, message = "Feedback not found" });
                }
                
                // Since Feedback doesn't have a Status property, 
                // we'll track this as an activity instead
                await _adminController.LogActivityAsync(
                    $"Feedback #{id} marked as read by admin",
                    "feedback",
                    "fas fa-check",
                    id,
                    "/Admin/FeedbackManagement"
                );
                
                return Ok(new { success = true, message = "Feedback marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking feedback as read");
                return StatusCode(500, new { success = false, message = "Server error", error = ex.Message });
            }
        }
        
        [HttpPost("Respond")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RespondToFeedback([FromBody] FeedbackResponseModel model)
        {
            try
            {
                var feedback = await _context.Feedbacks
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(f => f.FeedbackId == model.FeedbackId);
                    
                if (feedback == null)
                {
                    return NotFound(new { success = false, message = "Feedback not found" });
                }
                
                // Save the response in the feedback record
                feedback.AdminResponse = model.ResponseMessage;
                feedback.ResponseDate = DateTime.Now;
                
                await _context.SaveChangesAsync();
                
                string userName = feedback.User != null 
                    ? $"{feedback.User.FirstName} {feedback.User.LastName}" 
                    : "Guest";
                    
                await _adminController.LogActivityAsync(
                    $"Admin responded to feedback from {userName}: \"{model.ResponseMessage}\"",
                    "feedback",
                    "fas fa-reply",
                    model.FeedbackId,
                    "/Admin/FeedbackManagement"
                );

                // Create notification for the user about the response
                if (feedback.User != null)
                {
                    await _adminController.CreateUserNotificationAsync(
                        feedback.User.User_Id,
                        $"We've responded to your feedback about {feedback.RoomType} room",
                        "feedback",
                        "fas fa-reply",
                        feedback.FeedbackId,
                        "/Sidebar/ReviewFeedback"
                    );
                }
                
                return Ok(new { success = true, message = "Response sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error responding to feedback");
                return StatusCode(500, new { success = false, message = "Server error", error = ex.Message });
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            try
            {
                var feedback = await _context.Feedbacks.FindAsync(id);
                if (feedback == null)
                {
                    return NotFound(new { success = false, message = "Feedback not found" });
                }
                
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
                
                await _adminController.LogActivityAsync(
                    $"Feedback #{id} deleted by admin",
                    "feedback",
                    "fas fa-trash",
                    null,
                    "/Admin/FeedbackManagement"
                );
                
                return Ok(new { success = true, message = "Feedback deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feedback");
                return StatusCode(500, new { success = false, message = "Server error", error = ex.Message });
            }
        }
    }

    public class FeedbackModel
    {
        public string RoomType { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
    
    public class FeedbackResponseModel
    {
        public int FeedbackId { get; set; }
        public string ResponseMessage { get; set; } = string.Empty;
    }
} 