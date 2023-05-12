using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineView.Application.models
{
    public class Menu
    {
        public Menu(decimal price)
        {
            Price = price;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Menu() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int Id { get; private set; }
        public decimal Price { get; set; }
        protected List<Dish> _dishes = new();
        public virtual IReadOnlyCollection<Dish> Dishes => _dishes;
        public List<Category> Categories { get; } = new();
    }
}
