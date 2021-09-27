using Models.UserMicroservice;

namespace Shared.Repositories
{
    public interface IUserRepository:IRepository<User>
    {
        public int Exist(string username, string pass);
    }
}
