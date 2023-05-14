using AutoMapper;
using DineView.Application.infrastructure;
using DineView.Application.models;
using DineView.Webapp.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DineView.Webapp.Pages.Restaurants
{
    public class EditModel : PageModel
    {
        private readonly DineContext _db;
        private readonly IMapper _mapper;

        public EditModel(DineContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [BindProperty]
        public RestaurantDto Restaurant { get; set; } = null!;
        public IActionResult OnPost(Guid guid)
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var restaurant = _db.Restaurants.FirstOrDefault(r => r.Guid == guid);
            if (restaurant is null) 
            {
                return RedirectToPage("/Restaurants/Index");
            }
            _mapper.Map(Restaurant, restaurant);
            _db.Entry(restaurant).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException) 
            {
                ModelState.AddModelError("", "Error while writing to the database");
                return Page();
            }
            
            return RedirectToPage("/Restaurants/Index");
        }

        public IActionResult OnGet(Guid guid)
        {
            var restaurant = _db.Restaurants.FirstOrDefault(r => r.Guid == guid);
            if (restaurant is null)
            {
                return RedirectToPage("/Restaurants/Index");
            }
            Restaurant = _mapper.Map<RestaurantDto>(restaurant);
            return Page();
        }
    }
}
