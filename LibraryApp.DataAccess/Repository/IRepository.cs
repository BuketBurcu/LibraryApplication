using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LibraryApp.DataAccess
{
    public interface IRepository
    {
        public interface IRepository<T> where T : class
        {
            List<T> GetAll();

            T Get(Expression<Func<T, bool>> predicate);

            void Add(T entity);

            void Update(Expression<Func<T, bool>> predicate, T entity);


            void Delete(Expression<Func<T, bool>> predicate, bool forceDelete = false);

        }
    }
}
