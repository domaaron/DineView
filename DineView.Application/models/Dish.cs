﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineView.Application.models
{
    [Table("Dish")]
    public class Dish : IEntity<int>
    {
        public Dish(string name, string description, float calories, TimeSpan? preparationTime, Category category)
        {
            Name = name;
            Description = description;
            Calories = calories;
            PreparationTime = preparationTime;
            Guid = Guid.NewGuid();
            Category = category;
            CategoryId = category.Id;
        }



#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Dish() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Calories { get; set; }
        public TimeSpan? PreparationTime { get; set; }
        public Guid Guid { get; private set; }
        public virtual Category Category { get; set; }
        public int CategoryId { get; set; }
        public virtual ICollection<Menu> Menus { get; } = new List<Menu>();
    }
}
