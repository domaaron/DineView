using DineView.Application.models;

namespace DineView.Application.infrastructure.Repositories
{
    public class RestaurantRepository : Repository<Restaurant, int>
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

        public RestaurantRepository(DineContext db) : base(db) { }
        public IReadOnlyList<DinesWithMenusCount> GetDinesWithMenus()
        {
            return _db.Restaurants
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

        public override (bool success, string message) Delete(Restaurant restaurant)
        {
            if (restaurant.Rating.Equals("10 / 10")) {
                return (false, $"The restaurant {restaurant.Name} can not be closed.");
            }

            return base.Delete(restaurant);
        }
    }
}
