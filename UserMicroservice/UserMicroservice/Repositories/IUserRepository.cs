
using Models.UserMicroservice;

namespace UserMicroservice.Repositories
{
    public interface IUserRepository:IRepository<User>
    {
        public int Exist(string username, string pass);
    }
}
