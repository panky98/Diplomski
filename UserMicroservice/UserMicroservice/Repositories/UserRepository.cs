using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Models;

namespace UserMicroservice.Repositories
{
    public class UserRepository:Repository<User>,IUserRepository
    {
        private DbSet<User> Users;
        public UserRepository(UserContext _context) : base(_context)
        {
            this.Users = _context.Set<User>();
        }

        public int Exist(string username, string pass)
        {
            User u = this.Users.Where(x => x.Username == username).FirstOrDefault();
            if(u!=null)
            {
                return BCrypt.Net.BCrypt.Verify(pass, u.Password)==true?u.Id:-1;
            }
            return -1;
        }
    }
}
