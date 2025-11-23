using DigitalNoteSharing.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalNoteSharing.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Note> Notes => Set<Note>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Note>()
                .HasOne(n => n.UploadedBy)
                .WithMany()
                .HasForeignKey(n => n.UploadedById)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
