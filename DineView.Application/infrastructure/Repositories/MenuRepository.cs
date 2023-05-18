using DineView.Application.models;

namespace DineView.Application.infrastructure.Repositories
{
    public class MenuRepository : Repository<Menu, int>
    {
        public MenuRepository(DineContext db) : base(db) { }
        public override (bool success, string? message) Insert(Menu menu)
        {
            return base.Insert(menu);
        }

        public (bool success, string message) Insert(decimal price, bool IsSpicy, Guid dishGuid, Guid restaurantGuid)
        {
            var dish = _db.Dishes.FirstOrDefault(d => d.Guid == dishGuid);
            if (dish is null)
            {
                return (false, "Invalid dish");
            }

            var restaurant = _db.Restaurants.FirstOrDefault(d => d.Guid == restaurantGuid);
            if (restaurant is null)
            {
                return (false, "Invalid restaurant");
            }

            return base.Insert(new Menu(
                price: price,
                restaurant: restaurant,
                dish: dish,
                isSpicy: IsSpicy
                ));
        }

    }
}
