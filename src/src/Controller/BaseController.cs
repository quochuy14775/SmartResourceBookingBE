using Microsoft.AspNetCore.Mvc;
using src.Repositories;

namespace src.Controller
{
    // BaseController với Generic cho các entity thông thường
    public class BaseController<TEntity> : ControllerBase where TEntity : class
    {
        protected readonly IRepository<TEntity> _baseRepository;
        protected readonly ICurrentUser _currentUser;

        public BaseController(IRepository<TEntity> baseRepository,
            ICurrentUser currentUser)
        {
            _baseRepository = baseRepository;
            _currentUser = currentUser;
        }
    }

    // BaseController đơn giản cho các controller đặc biệt (như UserController)
    public class BaseController : ControllerBase
    {
        protected readonly ICurrentUser _currentUser;

        public BaseController(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }
    }
}