using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Models;

namespace UserMicroservice.Repositories
{
    public interface IInterestByUserRepository:IRepository<InterestByUser>
    {
        void AddOneByUserIdAndInterestId(int userId, int interestId);
    }
}
