﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Models;

namespace UserMicroservice.Repositories
{
    public class InterestRepository:Repository<Interest>,IInterestRepository
    {
        private DbSet<Interest> Interests;
        public InterestRepository(UserContext _context) : base(_context)
        {
            this.Interests = _context.Set<Interest>();
        }
    }
}
