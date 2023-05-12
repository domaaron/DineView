using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineView.Application.models
{
    public class Menu
    {
        public Menu(Dish dish, string dishName, Category category, string categoryName, string description, decimal price)
        {
            Dish = dish;
            DishName = dishName;
            Category = category;
            CategoryName = categoryName;
            Description = description;
            Price = price;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Menu() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int Id { get; private set; }
        public Dish Dish { get; set; }
        public string DishName { get; set; }
        public Category Category { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        protected List<Dish> _dishes = new();
        public IReadOnlyCollection<Dish> Dishes => _dishes;
    }
}
