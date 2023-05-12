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
        public DineContext(DbContextOptions opt) : base(opt) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Cuisine> Cuisines => Set<Cuisine>();
        public DbSet<Dish> Dishes => Set<Dish>();
        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<Restaurant> Restaurants => Set<Restaurant>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurant>().OwnsOne(r => r.Address);
        }

        public void Seed()
        {
            Randomizer.Seed = new Random(2169);

            var category = new Faker<Category>("en").CustomInstantiator(c => new Category(
                name: c.Commerce.ProductMaterial()
                ))
                .Generate(10)
                .ToList();
            Categories.AddRange(category);
            SaveChanges();

            var cuisine = new Faker<Cuisine>("en").CustomInstantiator(c => new Cuisine(
                name: c.Commerce.ProductAdjective()
                ))
                .Generate(10)
                .ToList();
            Cuisines.AddRange(cuisine);
            SaveChanges();

            var dish = new Faker<Dish>("en").CustomInstantiator(d => new Dish(
                name: d.Commerce.ProductName()
                ))
                .Generate(10)
                .ToList();
            Dishes.AddRange(dish);
            SaveChanges();

            var menu = new Faker<Menu>("en").CustomInstantiator(m => new Menu(
                dishName: m.Random.ListItem(dish),
                categoryName: m.Random.ListItem(category),
                description: m.Commerce.ProductDescription(),
                price: m.Random.Decimal(3, 50)
                ))
                .Generate(10)
                .ToList();
            Menus.AddRange(menu);
            SaveChanges();

            var restaurant = new Faker<Restaurant>().CustomInstantiator(r => new Restaurant(
                name: r.Company.CompanyName(),
                address: new Address(r.Address.StreetName(), r.Address.StreetSuffix()),
                openingTime: r.Date.BetweenTimeOnly(new TimeOnly(8, 0, 0), new TimeOnly(11, 0, 0)),
                closedTime: r.Date.BetweenTimeOnly(new TimeOnly(20, 0, 0), new TimeOnly(22, 0, 0)),
                cuisineName: r.Random.ListItem(cuisine),
                isOrderable: r.Random.Bool(),
                rating: $"{r.Random.Int(1, 10)} / 10",
                tel: r.Phone.PhoneNumber(),
                uRL: r.Internet.Url()
                ))
                .Generate(10)
                .ToList();
            Restaurants.AddRange(restaurant);
            SaveChanges();
        }
    }
}
