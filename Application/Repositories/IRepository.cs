using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T?> GetAll(bool tracking = true);
        IEnumerable<T?> GetWhere(Expression<Func<T, bool>> expression);

        Task<T?> GetAsync(string id);
        Task<T?> GetAsync(Expression<Func<T, bool>> expression);

        Task<bool> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        bool Update(T entity);
        bool Remove(T entity);

        Task<bool> RemoveAsync(string id);

        Task<int> SaveChangesAsync();
    }
}
