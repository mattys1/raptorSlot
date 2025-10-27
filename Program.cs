using raptorSlot.DAL;
using Microsoft.EntityFrameworkCore;
using raptorSlot.Models;
using Microsoft.AspNetCore.Identity;
using raptorSlot.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDBContext>(
		options => options.UseSqlite(builder.Configuration.GetConnectionString("defaultConnection"))
);
builder.Services.AddDefaultIdentity<AppUser>(
		options => options.SignIn.RequireConfirmedAccount = true
).AddEntityFrameworkStores<AppDBContext>().AddDefaultUI();

builder.Services.Configure<IdentityOptions>(
		options => {
			options.Password.RequireDigit = false;
			options.Password.RequireLowercase = false;
			options.Password.RequireUppercase = false;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequiredLength = 1;
			options.Password.RequiredUniqueChars = 0;
		}
);

builder.Services.AddScoped<AccountService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
).WithStaticAssets();

app.MapRazorPages();

app.Run();
