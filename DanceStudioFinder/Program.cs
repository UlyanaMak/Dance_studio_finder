using DanceStudioFinder.Data;
using DanceStudioFinder.Models;
using DanceStudioFinder.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//����������� DbContext ��� ������ � ��
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


//������������ ���� ��� ���������� � ������ ��� ��������������������� ������������
app.MapControllerRoute(
    name: "studioDetails",
    pattern: "StudioDetails/{studioId:int}",
    defaults: new { controller = "StudioDetails", action = "Index" });

//���� ��� �������������� ������
app.MapControllerRoute(
    name: "adminStudio",
    pattern: "AdminStudio/{adminId?}",
    defaults: new { controller = "AdminStudio", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{adminId?}/{studioId?}");  //��� ������ url ������

app.Run();
