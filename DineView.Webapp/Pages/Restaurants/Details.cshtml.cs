using AutoMapper;
using AutoMapper.QueryableExtensions;
using DineView.Application.infrastructure;
using DineView.Application.infrastructure.Repositories;
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
        private readonly RestaurantRepository _restaurants;
        private readonly DishRepository _dishes;
        private readonly MenuRepository _menus;


        private readonly IMapper _mapper;

        public DetailsModel(IMapper mapper, RestaurantRepository restaurant, DishRepository dishes, MenuRepository menus)
        {
            _mapper = mapper;
            _restaurants = restaurant;
            _dishes = dishes;
            _menus = menus;
        }
        [FromRoute]
        public Guid Guid { get; set; }

        public MenuDto NewMenu { get; set; } = default!;
        public Restaurant Restaurant { get; private set; } = default!;
        public IReadOnlyList<Menu> Menus { get; private set; } = new List<Menu>();
        public Dictionary<Guid, MenuDto> EditMenus { get; set; } = new();
        public IEnumerable<SelectListItem> DishSelectList =>
            _dishes.Set.OrderBy(d => d.Name).Select(d => new SelectListItem(d.Name, d.Guid.ToString()));

        public IActionResult OnPostEditMenu(Guid guid, Guid menuGuid, Dictionary<Guid, MenuDto> editMenus)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var menu = _menus.FindByGuid(menuGuid);
            
            if (menu is null)
            {
                return RedirectToPage();
            }
            
            _mapper.Map(editMenus[menuGuid], menu);

            var (success, message) = _menus.Update(menu);
            if (!success)
            {
                ModelState.AddModelError("", message);
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

            var (success, message) = _menus.Insert(
                price: newMenu.Price,
                IsSpicy: newMenu.IsSpicy,
                dishGuid: newMenu.DishGuid,
                restaurantGuid: guid);

            if (!success)
            {
                ModelState.AddModelError("", message);
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
            var restaurant = _restaurants.Set
                .Include(r => r.Menus)
                .ThenInclude(r => r.Dish)
                .FirstOrDefault(r => r.Guid == Guid);

            if (restaurant is null)
            {
                context.Result = RedirectToPage("/Restaurants/Index");
                return;
            }
            Restaurant = restaurant;

            Menus = restaurant.Menus.ToList();

            EditMenus = _menus.Set.Where(m => m.Restaurant.Guid == Guid)
                .ProjectTo<MenuDto>(_mapper.ConfigurationProvider)
                .ToDictionary(m => m.guid, m => m);
        }
    }
}
