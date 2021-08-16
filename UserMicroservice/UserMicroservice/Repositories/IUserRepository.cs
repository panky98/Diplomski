using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Models;

namespace UserMicroservice.Repositories
{
    public interface IUserRepository:IRepository<User>
    {
        public int Exist(string username, string pass);
    }
}
