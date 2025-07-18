using Microsoft.AspNetCore.Mvc;
using HRSM.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HRSM.ViewComponents
{
    public class StaffDepartmentViewComponent : ViewComponent
    {
        private readonly HRSMDbContext _context;

        public StaffDepartmentViewComponent(HRSMDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Content("");
            }

            var department = _context.Entry(user).Property<string>("Department").CurrentValue;
            return Content(department ?? "");
        }
    }
} 