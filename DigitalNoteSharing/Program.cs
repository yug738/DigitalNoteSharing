using DigitalNoteSharing.Data;
using DigitalNoteSharing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1️⃣ Database Connection
// ============================================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================
// 2️⃣ Identity Configuration (users + roles)
// ============================================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// ============================================================
// 3️⃣ MVC Controllers and Views
// ============================================================
builder.Services.AddControllersWithViews();

// ============================================================
// 4️⃣ Build the app
// ============================================================
var app = builder.Build();

// ============================================================
// 5️⃣ Middleware pipeline
// ============================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Identity middlewares (must be in this order)
app.UseAuthentication();
app.UseAuthorization();

// ============================================================
// 6️⃣ Routing setup
// ============================================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // for Identity pages (Register/Login)

app.Run();
