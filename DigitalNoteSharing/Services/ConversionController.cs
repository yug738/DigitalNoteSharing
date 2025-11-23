using System;
using System.IO;
using System.Threading.Tasks;
using DigitalNoteSharing.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalNoteSharing.Controllers
{
    [Authorize]
    public class ConversionController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public ConversionController(IWebHostEnvironment env) { _env = env; }

        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Convert(IFormFile file, string direction)
        {
            if (file == null || file.Length == 0) { TempData["err"] = "Pick a file."; return RedirectToAction("Index"); }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var tmpDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(tmpDir);

            var src = Path.Combine(tmpDir, $"{Guid.NewGuid():N}{ext}");
            using (var fs = new FileStream(src, FileMode.Create)) { await file.CopyToAsync(fs); }

            try
            {
                if (direction == "csv-to-xlsx" && ext == ".csv")
                {
                    var dest = Path.Combine(tmpDir, $"{Guid.NewGuid():N}.xlsx");
                    ConversionService.CsvToXlsx(src, dest);
                    return PhysicalFile(dest, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(dest));
                }
                else if (direction == "xlsx-to-csv" && ext == ".xlsx")
                {
                    var dest = Path.Combine(tmpDir, $"{Guid.NewGuid():N}.csv");
                    ConversionService.XlsxToCsv(src, dest);
                    return PhysicalFile(dest, "text/csv", Path.GetFileName(dest));
                }
            }
            catch (Exception ex)
            {
                TempData["err"] = "Conversion failed: " + ex.Message;
                return RedirectToAction("Index");
            }

            TempData["err"] = "Unsupported combination.";
            return RedirectToAction("Index");
        }
    }
}
