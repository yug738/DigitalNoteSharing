using Microsoft.AspNetCore.Identity;
using System;

namespace DigitalNoteSharing.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
        public string CollegeName { get; set; } = "";
        public string Degree { get; set; } = "";
        public string Major { get; set; } = "";
        public string YearOrSemester { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
