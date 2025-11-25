using src.Data;

namespace src.Repositories
{
    public class Repository<T> : RepositoryWithTypedId<T, long>, IRepository<T>
        where T : class
    {
        public Repository(ApplicationDbContext context, ICurrentUser currentUser) : base(context, currentUser)
        {
        }
    }
}