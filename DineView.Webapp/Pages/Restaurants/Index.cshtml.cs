using DineView.Application.infrastructure;
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
        public record DinesWithMenusCount(
            Guid Guid,
            string Name,
            int menusCount,
            string Street,
            string District,
            TimeOnly OpeningTime,
            TimeOnly ClosedTime,
            string CuisineStyle,
            bool IsOrderable,
            string Rating,
            string Tel,
            string URL
            );

        private readonly DineContext _db;
        public List<DinesWithMenusCount> Restaurants { get; private set; } = new();
        public IndexModel(DineContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            Restaurants = _db.Restaurants
                .OrderBy(r => r.Name)
                .Select(r => new DinesWithMenusCount(
                r.Guid,
                r.Name,
                r.Menus.Count(),
                r.Address.Street,
                r.Address.District,
                r.OpeningTime,
                r.ClosedTime,
                r.CuisineStyle,
                r.IsOrderable,
                r.Rating,
                r.Tel,
                r.URL
                ))
                .ToList();
        }
    }
}
