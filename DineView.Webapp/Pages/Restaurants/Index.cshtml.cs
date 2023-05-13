using DineView.Application.infrastructure;
using DineView.Application.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace DineView.Webapp.Pages.Restaurants
{
    public class IndexModel : PageModel
    {
        private readonly DineContext _db;
        public List<Restaurant> Restaurants { get; private set; } = new();
        public IndexModel(DineContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            Restaurants = _db.Restaurants.OrderBy(r => r.Id).ToList();
        }
    }
}
