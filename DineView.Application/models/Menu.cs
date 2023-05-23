using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineView.Application.models
{
    [Index(nameof(DishId), IsUnique = true)]
    public class Menu : IEntity<int>
    {
        public Menu(decimal price, Restaurant restaurant, Dish dish, bool isSpicy)
        {
            Price = price;
            IsSpicy = isSpicy;
            Restaurant = restaurant;
            RestaurantId = restaurant.Id;
            Dish = dish;
            DishId = dish.Id;
            Guid = Guid.NewGuid();
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Menu() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int Id { get; private set; }
        public decimal Price { get; set; }
        public bool IsSpicy { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        public int RestaurantId { get; set; }
        public virtual Dish Dish { get; set; }
        public int DishId { get; set; }
        public Guid Guid { get; private set; }
    }
}
