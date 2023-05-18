using DineView.Application.models;

namespace DineView.Application.infrastructure.Repositories
{
    public class DishRepository : Repository<Dish, int>
    {
        public DishRepository(DineContext db) : base(db) { }
    }
}
