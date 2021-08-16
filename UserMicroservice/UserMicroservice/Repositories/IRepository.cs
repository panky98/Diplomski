﻿using UserMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace UserMicroservice.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetOne(int id);
        bool Add(T obj);
        bool Delete(T obj);
        bool Update(T obj);
        ICollection<T> GetAll();
    }
}
