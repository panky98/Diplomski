using Microsoft.EntityFrameworkCore;
using Models;
using Models.UserMicroservice;
using UserMicroservice.Repositories;

namespace Shared.Repositories
{
    public class InterestRepository:Repository<Interest>,IInterestRepository
    {
        private DbSet<Interest> Interests;
        public InterestRepository(UserContext context) : base(context)
        {
            this.Interests = context.Set<Interest>();
        }
    }
}
