using DineView.Application.infrastructure;
using DineView.Application.infrastructure.Repositories;
using DineView.Webapp.Dto;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var opt = new DbContextOptionsBuilder()
    .UseSqlite("Data source=DineView.db")  // Keep connection open (only needed with SQLite in memory db)
    .Options;

using (var db = new DineContext(opt))
{
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    db.Seed();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DineContext>(opt =>
{
    opt.UseSqlite("Data source=DineView.db");
});
builder.Services.AddTransient<RestaurantRepository>();
builder.Services.AddTransient<MenuRepository>();
builder.Services.AddTransient<DishRepository>();
builder.Services.AddTransient<CategoryRepository>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddRazorPages();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.Run();
