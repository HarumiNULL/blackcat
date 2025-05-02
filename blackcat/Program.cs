using blackcat.Models;
using blackcat.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<UserServices>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddDbContext<BlackcatDbContext>();

// ⚡ Agrega la sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Opcional: expira tras 30 minutos de inactividad
    options.Cookie.HttpOnly = true; // Seguridad extra
    options.Cookie.IsEssential = true; // Necesario para GDPR
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    BlackcatDbContext context = scope.ServiceProvider.GetRequiredService<BlackcatDbContext>();
    context.Database.EnsureCreated();
    DbInitializer.Seed(context);
}

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