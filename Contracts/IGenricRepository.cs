using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Leave_Management.Contracts
{
    public interface IGenricRepository<T> where T : class
    {
        Task<IList<T>> FindAll(Expression<Func<T, bool>> expression = null,
                                Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
                                Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);

        Task<T> Find(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);

        Task<bool> IsExists(Expression<Func<T, bool>> expression = null);

        Task Create(T entity);

        void Update(T entity);

        void Delete(T entity);

        //Task Save();
    }
}
