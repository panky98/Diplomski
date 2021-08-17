using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Models;

namespace UserMicroservice.Repositories
{
    public class InterestByUserRepository:Repository<InterestByUser>,IInterestByUserRepository
    {
        private DbSet<InterestByUser> Interests;

        public InterestByUserRepository(UserContext context):base(context)
        {
            this.Interests = context.Set<InterestByUser>();
        }

        public void AddOneByUserIdAndInterestId(int userId, int interestId)
        {
            InterestByUser newInterestByUser = new InterestByUser()
            {
                UserId=userId,
                InterestId=interestId
            };
            this.Interests.Add(newInterestByUser);
        }
    }
}
