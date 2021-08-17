﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
