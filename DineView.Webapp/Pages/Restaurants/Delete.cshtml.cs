using DineView.Application.infrastructure.Repositories;
using DineView.Application.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace DineView.Webapp.Pages.Restaurants
{
    public class DeleteModel : PageModel
    {
        private readonly RestaurantRepository _restaurants;
        [TempData]
        public string? Message { get; set; }
        public Restaurant Restaurant { get; set; } = default!;

        public DeleteModel(RestaurantRepository restaurants)
        {
            _restaurants = restaurants;
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Restaurants/Index");
        }

        public IActionResult OnPostDelete(Guid guid)
        {
            var restaurant = _restaurants.FindByGuid(guid);
            if (restaurant is null)
            {
                return RedirectToPage("/Restaurants/Index");
            }
            
            var (success, message) = _restaurants.Delete(restaurant);
            if (!success)
            {
                Message = message;
            }

            return RedirectToPage("/Restaurants/Index");
        }

        public IActionResult OnGet(Guid guid)
        {
            var restaurant = _restaurants.FindByGuid(guid);
            if (restaurant is null) 
            {
                return RedirectToPage("/Restaurants/Index");
            }
            Restaurant = restaurant;
            return Page();
        }
    }
}
