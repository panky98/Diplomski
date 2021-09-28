using Microsoft.EntityFrameworkCore;
using Models.UserMicroservice;
using UserMicroservice.Configuration;
using UserMicroservice.Repositories;

namespace UserMicroservice.Repositories
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
