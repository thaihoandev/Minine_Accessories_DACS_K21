
using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Accessories_Store.Data_Access;
using Accessories_Store.Models;
using Accessories_Store.Models.Entities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Emit;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Accessories_Store;
using Microsoft.Extensions.Options;
using Accessories_Store.Services;
using Microsoft.Office.Interop.Word;
using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.ViewModels;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Hangfire để sử dụng SQL Server
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
 .AddDefaultTokenProviders()
 .AddDefaultUI()
 .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";

});


// Inject IOptions<EmailOptions> vào các dịch vụ khác
builder.Services.AddTransient<IMyEmailSender, MyEmailSender>();

builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<IProductRepo, EFProductRepo>();
builder.Services.AddScoped<ICategoryRepo, EFCategoryRepo>();
builder.Services.AddScoped<IProductVariantRepo, EFProductVariantRepo>();
builder.Services.AddScoped<IOrderRepo, EFOrderRepo>();
builder.Services.AddScoped<IRatingRepo, EFRatingRepo>();
builder.Services.AddScoped<IBlogRepo, EFBlogRepo>();
builder.Services.AddScoped<ICommentRepo, EFCommentRepo>();
builder.Services.AddScoped<IVoucherRepo, EFVoucherRepo>();


builder.Services.AddScoped<IProductMaterialRepo, EFProductMaterialRepo>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<ShoppingCart>();


builder.Services.AddNotyf(config => { config.DurationInSeconds = 3; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}


app.UseStaticFiles();

app.UseSession();

app.UseRouting();


app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}",
        defaults: new { area = "User" } 
    );

    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

// Cấu hình Hangfire Dashboard (tùy chọn)
app.UseHangfireDashboard();

app.Run();


RecurringJob.AddOrUpdate(
    "update-expired-coupons-status",
    () => PaymentCoupon.UpdateExpiredCouponsStatus(app.Services.GetRequiredService<ApplicationDbContext>()),
    Cron.Daily); // Runs daily