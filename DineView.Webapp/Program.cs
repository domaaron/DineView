using DineView.Application.infrastructure;
using DineView.Application.infrastructure.Repositories;
using DineView.Application.models;
using DineView.Application.Services;
using DineView.Webapp.Dto;
using DineView.Webapp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var opt = new DbContextOptionsBuilder()
    .UseSqlite("Data source=DineView.db")  // Keep connection open (only needed with SQLite in memory db)
    .Options;

using (var db = new DineContext(opt))
{
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    db.Seed(new CryptService());
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DineContext>(opt =>
{
    opt.UseSqlite("Data source=DineView.db")
    .EnableSensitiveDataLogging(true);
});
builder.Services.AddTransient<RestaurantRepository>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<MenuRepository>();
builder.Services.AddTransient<DishRepository>();
builder.Services.AddTransient<CategoryRepository>();
builder.Services.AddTransient<DishImportService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddRazorPages();

// Services for authentication
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ICryptService, CryptService>();
builder.Services.AddTransient<AuthService>(provider => new AuthService(
    isDevelopment: builder.Environment.IsDevelopment(),
    db: provider.GetRequiredService<DineContext>(),
    cryptService: provider.GetRequiredService<ICryptService>(),
    httpContextAccessor: provider.GetRequiredService<IHttpContextAccessor>()));

builder.Services.AddAuthentication(
    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/User/Login";
        o.AccessDeniedPath = "/User/AccessDenied";
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("OwnerOrAdminRole", p => p.RequireRole(Usertype.Owner.ToString(), Usertype.Admin.ToString()));
});

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
