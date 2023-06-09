using AutoMapper;
using AutoMapper.QueryableExtensions;
using DineView.Application.infrastructure;
using DineView.Application.infrastructure.Repositories;
using DineView.Application.models;
using DineView.Webapp.Dto;
using DineView.Webapp.Services;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly RestaurantRepository _restaurants;
        private readonly DishRepository _dishes;
        private readonly MenuRepository _menus;
        private readonly IMapper _mapper;
        private readonly AuthService _authService;

        public DetailsModel(IMapper mapper, RestaurantRepository restaurant, DishRepository dishes, MenuRepository menus, AuthService authService)
        {
            _mapper = mapper;
            _restaurants = restaurant;
            _dishes = dishes;
            _menus = menus;
            _authService = authService;
        }

        [FromRoute]
        public Guid Guid { get; set; }
        public MenuDto NewMenu { get; set; } = default!;
        public Restaurant Restaurant { get; private set; } = default!;
        public IReadOnlyList<Menu> Menus { get; private set; } = new List<Menu>();
        public Dictionary<Guid, MenuDto> EditMenus { get; set; } = new();
        public Dictionary<Guid, bool> MenusToDelete { get; set; } = new();
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

        public IActionResult OnPostDelete(Guid guid, Dictionary<Guid, bool> menusToDelete)
        {
            var menusDb = _menus.Set
                .Where(m => m.Restaurant.Guid == guid)
                .ToDictionary(m => m.Guid, m => m);

            var menusGuids = menusToDelete
                .Where(m => m.Value == true)
                .Select(m => m.Key);

            foreach (var m in menusGuids)
            {
                if (!menusDb.TryGetValue(m, out var menu))
                {
                    continue;
                }
                _menus.Delete(menu);
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
                .Include(r => r.Manager)
                .Include(r => r.Menus)
                .ThenInclude(r => r.Dish)
                .FirstOrDefault(r => r.Guid == Guid);

            if (restaurant is null)
            {
                context.Result = RedirectToPage("/Restaurants/Index");
                return;
            }

            var username = _authService.Username;
            if (!_authService.HasRole("Admin") && username != restaurant.Manager?.Username)
            {
                context.Result = new ForbidResult();
                return;
            }

            Restaurant = restaurant;
            Menus = restaurant.Menus.ToList();
            MenusToDelete = Menus.ToDictionary(m => m.Guid, m => false);
            EditMenus = _menus.Set.Where(m => m.Restaurant.Guid == Guid)
                .ProjectTo<MenuDto>(_mapper.ConfigurationProvider)
                .ToDictionary(m => m.guid, m => m);
        }
    }
}
