using DineView.Application.infrastructure;
using DineView.Application.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DineView.Webapp.Pages.Restaurants
{
    public class DetailsModel : PageModel
    {
        private readonly DineContext _db;
        public DetailsModel(DineContext db)
        {
            _db = db;
        }

        public Restaurant Restaurant { get; private set; } = default!;

        public IActionResult OnGet(Guid guid)
        {
            var restaurant = _db.Restaurants
                .Include(r => r.Menus)
                .ThenInclude(m => m.Dish)
                .FirstOrDefault(r => r.Guid == guid);

            if (restaurant == null)
            {
                return RedirectToPage("/Restaurants/Index");
            }
            Restaurant = restaurant;
            return Page();
        }
    }
}
