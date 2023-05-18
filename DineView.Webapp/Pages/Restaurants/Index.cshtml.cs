using DineView.Application.infrastructure;
using DineView.Application.infrastructure.Repositories;
using DineView.Application.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DineView.Webapp.Pages.Restaurants
{
    public class IndexModel : PageModel
    {
        private readonly RestaurantRepository _restaurants;

        public IndexModel(RestaurantRepository restaurants)
        {
            _restaurants = restaurants;
        }

        public IReadOnlyList<RestaurantRepository.DinesWithMenusCount> Restaurants { get; private set; } = new List<RestaurantRepository.DinesWithMenusCount>();

        public void OnGet()
        {
            Restaurants = _restaurants.GetDinesWithMenus();
        }
    }
}
