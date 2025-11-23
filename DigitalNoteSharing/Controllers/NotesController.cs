using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalNoteSharing.Data;
using DigitalNoteSharing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalNoteSharing.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        private readonly string[] _allowedExt = new[] { ".pdf", ".pptx", ".docx", ".xlsx", ".csv", ".png", ".jpg", ".jpeg" };
        private const long MaxFileSizeBytes = 20 * 1024 * 1024; // 20 MB

        public NotesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _db = db; _userManager = userManager; _env = env;
        }

        // Public library (anyone logged-in can view)
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? q, string? type, string? major, string? degree, string? year)
        {
            var notes = _db.Notes.Include(n => n.UploadedBy).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                notes = notes.Where(n => n.Title.Contains(q) || (n.Description ?? "").Contains(q));

            if (!string.IsNullOrWhiteSpace(type))
                notes = notes.Where(n => n.FileType == type);

            if (!string.IsNullOrWhiteSpace(major))
                notes = notes.Where(n => n.UploadedBy.Major == major);

            if (!string.IsNullOrWhiteSpace(degree))
                notes = notes.Where(n => n.UploadedBy.Degree == degree);

            if (!string.IsNullOrWhiteSpace(year))
                notes = notes.Where(n => n.UploadedBy.YearOrSemester == year);

            notes = notes.OrderByDescending(n => n.UploadedAt);
            return View(await notes.ToListAsync());
        }

        // My Notes
        public async Task<IActionResult> Mine()
        {
            var userId = _userManager.GetUserId(User)!;
            var myNotes = await _db.Notes.Where(n => n.UploadedById == userId)
                                         .OrderByDescending(n => n.UploadedAt)
                                         .ToListAsync();
            return View(myNotes);
        }

        // Upload GET
        public IActionResult Create() => View();

        // Upload POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note model, IFormFile? file)
        {
            // 1️⃣ Check if file was uploaded
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please choose a file to upload.");
                return View(model);
            }

            // 2️⃣ Validate file type
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExt = new[] { ".pdf", ".pptx", ".docx", ".xlsx", ".csv", ".png", ".jpg", ".jpeg" };

            if (!allowedExt.Contains(ext))
            {
                ModelState.AddModelError("", $"Unsupported file type. Allowed: {string.Join(", ", allowedExt)}");
                return View(model);
            }

            // 3️⃣ Validate file size (max 20MB)
            if (file.Length > 20 * 1024 * 1024)
            {
                ModelState.AddModelError("", "File too large (max 20MB).");
                return View(model);
            }

            // 4️⃣ Ensure uploads folder exists
            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsRoot);

            // 5️⃣ Save file with unique name
            var uniqueName = $"{Guid.NewGuid():N}{ext}";
            var savePath = Path.Combine(uploadsRoot, uniqueName);
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 6️⃣ Get logged-in user
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                TempData["msg"] = "User not logged in.";
                return RedirectToAction("Login", "Account");
            }

            // 7️⃣ Save note info to DB
            model.FilePath = $"/uploads/{uniqueName}";
            model.FileType = ext.Trim('.');
            model.UploadedById = userId;
            model.UploadedAt = DateTime.UtcNow;

            _db.Notes.Add(model);
            await _db.SaveChangesAsync();

            // 8️⃣ Success message and redirect
            TempData["msg"] = "✅ Note uploaded successfully!";
            return RedirectToAction(nameof(Mine));
        }


        // Edit
        public async Task<IActionResult> Edit(int id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound();
            if (note.UploadedById != _userManager.GetUserId(User)) return Forbid();
            return View(note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Note input)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound();
            if (note.UploadedById != _userManager.GetUserId(User)) return Forbid();

            if (!ModelState.IsValid) return View(input);

            note.Title = input.Title;
            note.Description = input.Description;
            await _db.SaveChangesAsync();

            TempData["msg"] = "Note updated.";
            return RedirectToAction(nameof(Mine));
        }

        // Delete
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound();
            if (note.UploadedById != _userManager.GetUserId(User)) return Forbid();
            return View(note);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound();
            if (note.UploadedById != _userManager.GetUserId(User)) return Forbid();

            // delete file from disk
            var full = Path.Combine(_env.WebRootPath, note.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(full))
                System.IO.File.Delete(full);

            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();

            TempData["msg"] = "Note deleted.";
            return RedirectToAction(nameof(Mine));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Download(int id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound();

            var full = Path.Combine(_env.WebRootPath, note.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(full)) return NotFound();

            var contentType = "application/octet-stream";
            return PhysicalFile(full, contentType, Path.GetFileName(full));
        }
    }
}
