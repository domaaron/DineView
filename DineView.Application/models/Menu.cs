using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineView.Application.models
{
    public class Menu
    {
        public Menu(decimal price, Restaurant restaurant, Dish dish)
        {
            Price = price;
            Restaurant = restaurant;
            RestaurantId = restaurant.Id;
            Dish = dish;
            DishId = dish.Id;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Menu() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int Id { get; private set; }
        public decimal Price { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        public int RestaurantId { get; set; }
        public virtual Dish Dish { get; set; }
        public int DishId { get; set; }
    }
}
