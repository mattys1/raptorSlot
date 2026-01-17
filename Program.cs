using raptorSlot.DAL;
using Microsoft.EntityFrameworkCore;
using raptorSlot.Models;
using Microsoft.AspNetCore.Identity;
using raptorSlot.Services;
using raptorSlot;
using System.Diagnostics;
using System.Text.Json.Serialization;
using OneOf.Serialization.SystemTextJson;
using raptorSlot.Services.Games;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddControllersWithViews()
	.AddJsonOptions(options => {
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});
builder.Services.AddDbContext<AppDBContext>(
options => options
	.UseSqlite(builder.Configuration.GetConnectionString("defaultConnection"))
);
builder.Services.AddIdentity<AppUser, IdentityRole>()
	.AddEntityFrameworkStores<AppDBContext>()
	.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(
options => {
	options.Password.RequireDigit = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 1;
	options.Password.RequiredUniqueChars = 0;
	options.SignIn.RequireConfirmedEmail = false;
	options.SignIn.RequireConfirmedAccount = false;
}
);

builder.Services.ConfigureApplicationCookie(
options => {
	options.LoginPath = "/Account/Login";
	options.AccessDeniedPath = "/Account/AccessDenied";
}
);


builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AdminPanelService>();
builder.Services.AddScoped<NumberDrawGameService>();
builder.Services.AddScoped<RouletteService>();
builder.Services.AddScoped<AvatarService>();
builder.Services.AddScoped<AvatarPathFactory>();
builder.Services.AddScoped<AppDBContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
	var userManager = services.GetRequiredService<UserManager<AppUser>>();

	if(!await roleManager.RoleExistsAsync(Roles.ADMIN)) {
		await roleManager.CreateAsync(new IdentityRole(Roles.ADMIN));
	}

	var adminUser = await userManager.FindByEmailAsync(EnvVars.ADMIN_EMAIL);
	if(adminUser == null) {
		adminUser = new AppUser { UserName = EnvVars.ADMIN_USERNAME, Email = EnvVars.ADMIN_EMAIL, EmailConfirmed = true };
		var createResult = await userManager.CreateAsync(adminUser, EnvVars.ADMIN_PASSWORD);
		var errors = string.Join("; ", createResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
		throw new InvalidOperationException($"Failed to create admin user: {errors}");
	}

	if(!await userManager.IsInRoleAsync(adminUser, Roles.ADMIN)) {
		await userManager.AddToRoleAsync(adminUser, Roles.ADMIN);

	}
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

// app.MapRazorPages();

app.Run();
