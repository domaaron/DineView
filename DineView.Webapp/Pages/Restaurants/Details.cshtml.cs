using AutoMapper;
using AutoMapper.QueryableExtensions;
using DineView.Application.infrastructure;
using DineView.Application.models;
using DineView.Webapp.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DineView.Webapp.Pages.Restaurants
{
    public class DetailsModel : PageModel
    {
        private readonly DineContext _db;
        private readonly IMapper _mapper;

        public DetailsModel(DineContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [FromRoute]
        public Guid Guid { get; set; }

        public MenuDto NewMenu { get; set; } = default!;
        public Restaurant Restaurant { get; private set; } = default!;
        public IReadOnlyList<Menu> Menus { get; private set; } = new List<Menu>();
        public Dictionary<Guid, MenuDto> EditMenus { get; set; } = new();
        public IEnumerable<SelectListItem> DishSelectList =>
            _db.Dishes.OrderBy(d => d.Name).Select(d => new SelectListItem(d.Name, d.Guid.ToString()));

        public IActionResult OnPostEditMenu(Guid guid, Guid menuGuid, Dictionary<Guid, MenuDto> editMenus)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var menu = _db.Menus.FirstOrDefault(m => m.Guid == menuGuid);
            
            if (menu is null)
            {
                return RedirectToPage();
            }
            
            _mapper.Map(editMenus[menuGuid], menu);
            _db.Entry(menu).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
            catch(DbUpdateException)
            {
                ModelState.AddModelError("", "Error while writing to the database");
                return Page();
            }

            return RedirectToPage();
        }
        public IActionResult OnPostNewMenu(Guid guid, MenuDto newMenu)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var menu = _mapper.Map<Menu>(newMenu);
                menu.Dish = _db.Dishes.FirstOrDefault(d => d.Guid == newMenu.DishGuid)
                    ?? throw new ApplicationException("Invalid dish");
                menu.Restaurant = _db.Restaurants.FirstOrDefault(r => r.Guid == guid)
                    ?? throw new ApplicationException("Invalid restaurant");
                _db.Menus.Add(menu);
                _db.SaveChanges();
            }
            catch(ApplicationException e)
            {
                ModelState.AddModelError("", e.Message);
                return Page();
            }
            catch(DbUpdateException)
            {
                ModelState.AddModelError("", "Error while writing to the database");
                return Page();
            }

            return RedirectToPage();
        }
        public IActionResult OnGet(Guid guid)
        {
            return Page();
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var restaurant = _db.Restaurants.FirstOrDefault(r => r.Guid == Guid);

            if (restaurant is null)
            {
                context.Result = RedirectToPage("/Restaurants/Index");
                return;
            }
            Restaurant = restaurant;

            Menus = _db.Menus
                .Include(m => m.Dish)
                .Where(m => m.Restaurant.Guid == Guid)
                .ToList();

            EditMenus = _db.Menus.Where(m => m.Restaurant.Guid == Guid)
                .ProjectTo<MenuDto>(_mapper.ConfigurationProvider)
                .ToDictionary(m => m.guid, m => m);
        }
    }
}
