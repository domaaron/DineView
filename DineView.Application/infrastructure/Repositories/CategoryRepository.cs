using DineView.Application.models;

namespace DineView.Application.infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category, int>
    {
        public CategoryRepository(DineContext db) : base(db) { }
    }
}
