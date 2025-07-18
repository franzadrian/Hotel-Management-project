using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HRSM.Data
{
    public class StaffDepartmentMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<StaffDepartmentMiddleware> _logger;

        public StaffDepartmentMiddleware(RequestDelegate next, ILogger<StaffDepartmentMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only apply this to the Staff controller paths
            if (context.Request.Path.StartsWithSegments("/Staff"))
            {
                // Check if user is authenticated and is a staff member
                if (context.User.Identity?.IsAuthenticated == true &&
                    context.User.IsInRole("Staff"))
                {
                    var path = context.Request.Path.Value?.ToLower();
                    var department = context.User.FindFirstValue("Department") ?? "Receptionist";

                    // Check if the staff member is accessing a page that matches their department
                    bool isAuthorized = true;

                    // If path contains "housekeeping" but user is not from housekeeping department
                    if (path?.Contains("/housekeeping") == true && department != "Housekeeping")
                    {
                        isAuthorized = false;
                    }
                    // If path contains "maintenance" but user is not from maintenance department
                    else if (path?.Contains("/maintenance") == true && department != "Maintenance")
                    {
                        isAuthorized = false;
                    }
                    // If path contains "receptionist" but user is not from receptionist department
                    else if (path?.Contains("/receptionist") == true && department != "Receptionist")
                    {
                        isAuthorized = false;
                    }

                    if (!isAuthorized)
                    {
                        _logger.LogWarning("Staff member {UserId} from {Department} department attempted to access unauthorized area: {Path}",
                            context.User.FindFirstValue(ClaimTypes.NameIdentifier),
                            department,
                            context.Request.Path);

                        // Redirect to the appropriate dashboard based on department
                        switch (department)
                        {
                            case "Housekeeping":
                                context.Response.Redirect("/Staff/HousekeepingManagement");
                                return;
                            case "Maintenance":
                                context.Response.Redirect("/Staff/MaintenanceManagement");
                                return;
                            default: // Receptionist
                                context.Response.Redirect("/Staff/ReceptionistDashboard");
                                return;
                        }
                    }
                }
            }

            // Continue processing the request
            await _next(context);
        }
    }

    // Extension method to make it easier to add the middleware to the pipeline
    public static class StaffDepartmentMiddlewareExtensions
    {
        public static IApplicationBuilder UseStaffDepartmentRedirection(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<StaffDepartmentMiddleware>();
        }
    }
} 