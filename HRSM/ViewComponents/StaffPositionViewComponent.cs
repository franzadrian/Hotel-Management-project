using Microsoft.AspNetCore.Mvc;
using HRSM.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HRSM.ViewComponents
{
    public class StaffPositionViewComponent : ViewComponent
    {
        private readonly HRSMDbContext _context;

        public StaffPositionViewComponent(HRSMDbContext context)
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

            var position = _context.Entry(user).Property<string>("Position").CurrentValue;
            return Content(position ?? "");
        }
    }
} 