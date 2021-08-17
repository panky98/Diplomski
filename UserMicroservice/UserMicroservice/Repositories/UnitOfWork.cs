using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Models;

namespace UserMicroservice.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserContext _context;

        public IUserRepository Users { get; private set; }
        public IInterestRepository Interests { get; private set; }
        public IInterestByUserRepository InterestsByUsers { get; private set; }
        public UnitOfWork(UserContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Interests = new InterestRepository(_context);
            InterestsByUsers = new InterestByUserRepository(_context);
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
