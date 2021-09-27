using Shared.Repositories;
using System;


namespace UserMicroservice.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IInterestRepository Interests { get; }
        IInterestByUserRepository InterestsByUsers{ get; }

        int Commit();
    }
}
