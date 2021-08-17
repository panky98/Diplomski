using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Linq.Expressions;
using UserMicroservice.Models;

namespace UserMicroservice.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly UserContext _context;

        public Repository(UserContext context)
        {
            this._context = context;
        }
        public bool Add(T obj)
        {
            this._context.Set<T>().Add(obj);
            return true;
        }

        public bool Delete(T obj)
        {
            this._context.Set<T>().Remove(obj);
           // _context.Entry(obj).State = EntityState.Detached;
            return true;
        }

        public ICollection<T> FindAll(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression).ToList();
        }

        public T FindOneByExpression(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression).FirstOrDefault();
        }

        public virtual ICollection<T> GetAll()
        {
            return new List<T>(this._context.Set<T>().ToListAsync<T>().Result.ToArray());
        }
        public virtual T GetOne(int id)
        {
            return this._context.Set<T>().Find(id);
        }

        public bool Update(T obj)
        {
            //_context.Entry(obj).State = EntityState.Modified;
            this._context.Set<T>().Update(obj);
            return true;
        }
    }
}
