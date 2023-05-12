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
    public class Restaurant
    {
        public Restaurant(string name, Address address, TimeOnly openingTime, TimeOnly closedTime, Cuisine cuisine, Cuisine cuisineName, bool isOrderable, string rating, string tel, string uRL)
        {
            Name = name;
            Guid = Guid.NewGuid();
            Address = address;
            OpeningTime = openingTime;
            ClosedTime = closedTime;
            Cuisine = cuisine;
            CuisineName = cuisineName;
            IsOrderable = isOrderable;
            Rating = rating;
            Tel = tel;
            URL = uRL;
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
        public Cuisine Cuisine { get; set; }
        public Cuisine CuisineName { get; set; }
        public bool IsOrderable { get; set; }
        public string Rating { get; set; }
        public string Tel { get; set; }
        public string URL { get; set; }

        public void ChangeOrderable(bool isOrderable)
        {
            IsOrderable = isOrderable;
        }


    }
}
