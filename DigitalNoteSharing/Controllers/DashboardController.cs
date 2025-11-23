using System.Linq;
using System.Threading.Tasks;
using DigitalNoteSharing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigitalNoteSharing.Models;

namespace DigitalNoteSharing.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db; _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var uid = _userManager.GetUserId(User)!;
            var totalMine = await _db.Notes.CountAsync(n => n.UploadedById == uid);
            var totalAll = await _db.Notes.CountAsync();
            ViewBag.TotalMine = totalMine;
            ViewBag.TotalAll = totalAll;
            return View();
        }
    }
}
