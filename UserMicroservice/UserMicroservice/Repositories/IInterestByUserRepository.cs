
using Models.UserMicroservice;

namespace UserMicroservice.Repositories
{
    public interface IInterestByUserRepository:IRepository<InterestByUser>
    {
        void AddOneByUserIdAndInterestId(int userId, int interestId);
    }
}
