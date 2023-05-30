using DineView.Application.infrastructure.Repositories;
using DineView.Application.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DineView.Webapp.Pages.Dishes
{
    [Authorize(Roles = "Admin, Owner")]
    public class IndexModel : PageModel
    {
        private readonly DishRepository _dishes;

        public IndexModel(DishRepository dishes)
        {
            _dishes = dishes;
        }

        public IEnumerable<Dish> Dishes =>
            _dishes.Set
            .Include(d => d.Category)
            .OrderBy(d => d.Id);
        
        public void OnGet()
        {
        }
    }
}
