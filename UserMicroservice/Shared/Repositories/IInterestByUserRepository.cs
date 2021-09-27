using Models.UserMicroservice;

namespace Shared.Repositories
{
    public interface IInterestByUserRepository:IRepository<InterestByUser>
    {
        void AddOneByUserIdAndInterestId(int userId, int interestId);
    }
}
