using DineView.Application.infrastructure;
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
builder.Services.AddRazorPages();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.Run();
