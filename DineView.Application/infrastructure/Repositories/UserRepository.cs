using DineView.Application.models;

namespace DineView.Application.infrastructure.Repositories
{
    public class UserRepository : Repository<User, int>
    {
        private readonly ICryptService _cryptService;
        public UserRepository(DineContext db, ICryptService cryptService) : base(db) 
        {
            _cryptService = cryptService;
        }
    }
}
