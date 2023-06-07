using DineView.Application.infrastructure;
using DineView.Application.infrastructure.Repositories;
using DineView.Application.models;
using DineView.Webapp.Services;
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
        private readonly AuthService _authService;

        public IndexModel(RestaurantRepository restaurants, AuthService authService)
        {
            _restaurants = restaurants;
            _authService = authService;
        }

        [TempData]
        public string? Message { get; set; }

        public IReadOnlyList<RestaurantRepository.DinesWithMenusCount> Restaurants { get; private set; } = new List<RestaurantRepository.DinesWithMenusCount>();

        public void OnGet()
        {
            Restaurants = _restaurants.GetDinesWithMenus();
        }

        public bool CanEditRestaurant(Guid restaurantGuid) =>
            _authService.IsAdmin || Restaurants.FirstOrDefault(r => r.Guid == restaurantGuid)?.Manager?.Username == _authService.Username;
    }
}
