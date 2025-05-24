using blackcat.Models;
using blackcat.Services;
using blackcat.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<UserServices>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ModServices>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<AdminServices>();
builder.Services.AddScoped<InformacionRepository>();
builder.Services.AddHttpContextAccessor();




builder.Services.AddDbContext<BlackcatDbContext>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
  options.LoginPath="/User/ViewLogin";
  options.AccessDeniedPath = "/User/AccessDenied";
});

// ⚡ Agrega la sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Opcional: expira tras 30 minutos de inactividad
    options.Cookie.HttpOnly = true; // Seguridad extra
    options.Cookie.IsEssential = true; // Necesario para GDPR
});

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     BlackcatDbContext context = scope.ServiceProvider.GetRequiredService<BlackcatDbContext>();
//     context.Database.EnsureCreated();
//     DbInitializer.Seed(context);
// }

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) 
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ⚡ Habilita la sesión aquí
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();