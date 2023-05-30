using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineView.Application.models
{
    public record Address(string Street, string District);
    [Table("Restaurant")]
    public class Restaurant : IEntity<int>
    {
        public Restaurant(string name, Address address, TimeOnly openingTime, TimeOnly closedTime, Cuisine cuisine, bool isOrderable, string rating, string tel, string uRL, User? manager = null)
        {
            Name = name;
            Guid = Guid.NewGuid();
            Address = address;
            OpeningTime = openingTime;
            ClosedTime = closedTime;
            Cuisine = cuisine;
            CuisineStyle = cuisine.Style;
            IsOrderable = isOrderable;
            Rating = rating;
            Tel = tel;
            URL = uRL;
            Manager = manager;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Restaurant() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int Id { get; private set; }
        public Guid Guid { get; private set; }
        public string Name { get; set; }
        public Address Address { get; set; }

        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosedTime { get; set; }
        public virtual Cuisine Cuisine { get; set; }
        public string CuisineStyle { get; set; }
        public bool IsOrderable { get; set; }
        public string Rating { get; set; }
        public string Tel { get; set; }
        public string URL { get; set; }
        public int? ManagerId { get; set; }
        public User? Manager { get; set; }
        public ICollection<Menu> Menus { get; } = new List<Menu>();
        public void ChangeOrderable(bool isOrderable)
        {
            IsOrderable = isOrderable;
        }


    }
}
