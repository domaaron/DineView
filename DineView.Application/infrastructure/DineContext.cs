using Bogus;
using DineView.Application.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineView.Application.infrastructure
{
    public class DineContext : DbContext
    {
        //Rounding preparationTime value to the nearest whole minute
        private static TimeSpan RoundToNearestMinuteAndSecond(TimeSpan timeSpan)
        {
            var totalSeconds = (int)timeSpan.TotalSeconds;
            var roundedMinutes = (totalSeconds + 30) / 60;
            var roundedSeconds = 0;
            return TimeSpan.FromMinutes(roundedMinutes).Add(TimeSpan.FromSeconds(roundedSeconds));
        }



        public DineContext(DbContextOptions opt) : base(opt) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Cuisine> Cuisines => Set<Cuisine>();
        public DbSet<Dish> Dishes => Set<Dish>();
        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<Restaurant> Restaurants => Set<Restaurant>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurant>().OwnsOne(r => r.Address);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var type = entity.ClrType;
                if (type.GetProperty("Guid") is not null)
                {
                    modelBuilder.Entity(type).HasAlternateKey("Guid");
                }
            }

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Designation)
                .IsUnique();
        }

        public void Seed()
        {
            Randomizer.Seed = new Random(2169);

            var category = new Faker<Category>("en").CustomInstantiator(c => new Category(
                designation: $"{c.Commerce.ProductMaterial()} {c.Commerce.ProductMaterial()}"
                ))
                .Generate(10)
                .ToList();
            Categories.AddRange(category);
            SaveChanges();
            
            var cuisine = new Faker<Cuisine>("en").CustomInstantiator(c => new Cuisine(
                style: $"{c.Commerce.ProductAdjective()} {c.Commerce.ProductAdjective()}"
                ))
                .Generate(10)
                .ToList();
            Cuisines.AddRange(cuisine);
            SaveChanges();
            
            var dish = new Faker<Dish>("en").CustomInstantiator(d => new Dish(
                name: d.Commerce.ProductName(),
                description: d.Commerce.ProductDescription(),
                calories: (float)Math.Round(d.Random.Float(10, 500)),
                preparationTime: RoundToNearestMinuteAndSecond(TimeSpan.FromMinutes(d.Random.Double(10, 120))),
                category: d.Random.ListItem(category)
                ))
                .Generate(100)
                .ToList();
            Dishes.AddRange(dish);
            SaveChanges();

            var restaurant = new Faker<Restaurant>().CustomInstantiator(r => new Restaurant(
                name: r.Company.CompanyName(),
                address: new Address(r.Address.StreetName(), r.Address.StreetSuffix()),
                openingTime: r.Date.BetweenTimeOnly(new TimeOnly(8, 0, 0), new TimeOnly(11, 0, 0)),
                closedTime: r.Date.BetweenTimeOnly(new TimeOnly(20, 0, 0), new TimeOnly(22, 0, 0)),
                cuisine: r.Random.ListItem(cuisine),
                isOrderable: r.Random.Bool(0.30f),
                rating: $"{r.Random.Int(1, 10)} / 10",
                tel: r.Phone.PhoneNumber(),
                uRL: r.Internet.Url()
                ))
                .Generate(10)
                .ToList();
            Restaurants.AddRange(restaurant);
            SaveChanges();

            var menu = new Faker<Menu>("en").CustomInstantiator(m => new Menu(
                price: Math.Round(m.Random.Decimal(3, 50)),
                isSpicy: m.Random.Bool(),
                restaurant: m.Random.ListItem(restaurant),
                dish: m.Random.ListItem(dish)
                ))
                .Generate(100)
                .DistinctBy(m => m.Dish)
                .ToList();
            Menus.AddRange(menu);
            SaveChanges();
        }
    }
}
