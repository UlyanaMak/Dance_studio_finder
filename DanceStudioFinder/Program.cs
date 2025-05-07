using DanceStudioFinder.Data;
using DanceStudioFinder.Models;
using DanceStudioFinder.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//подключение DbContext для работы с БД
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IPasswordHasher<Admin>, PasswordHasher<Admin>>();
builder.Services.AddScoped<AdminStudioService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<OpenStreetMapService>();
builder.Services.AddScoped<InfoAdminStudioService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


//степциальный путь для информации о студии для незарегистрированного пользователя
app.MapControllerRoute(
    name: "studioDetails",
    pattern: "StudioDetails/{studioId:int}",
    defaults: new { controller = "StudioDetails", action = "Index" });

//путь для администратора студии
app.MapControllerRoute(
    name: "adminStudio",
    pattern: "AdminStudio/{adminId?}",
    defaults: new { controller = "AdminStudio", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{adminId?}/{studioId?}");  //мой шаблон url адреса

app.Run();
