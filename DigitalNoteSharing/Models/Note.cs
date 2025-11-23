using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalNoteSharing.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = "";

        [StringLength(500)]
        public string? Description { get; set; }

        [Required, StringLength(20)]
        public string FileType { get; set; } = ""; // pdf, pptx, docx, xlsx, csv, png, jpg

        [Required]
        public string FilePath { get; set; } = ""; // /uploads/...

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // FK -> ApplicationUser
        [Required]
        public string UploadedById { get; set; } = "";

        [ForeignKey(nameof(UploadedById))]
        public ApplicationUser UploadedBy { get; set; } = null!;
    }
}
