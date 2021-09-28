using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UserMicroservice.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetOne(int id);
        bool Add(T obj);
        bool Delete(T obj);
        bool Update(T obj);
        ICollection<T> GetAll();
        ICollection<T> FindAll(Expression<Func<T, bool>> expression);
        T FindOneByExpression(Expression<Func<T, bool>> expression);

    }
}
