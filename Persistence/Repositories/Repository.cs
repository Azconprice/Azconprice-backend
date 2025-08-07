using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        DbSet<T> Table => _context.Set<T>();

        public async Task<T?> AddAsync(T entity)
        {
            var entry = await Table.AddAsync(entity);
            if( entry.State != EntityState.Added)
                throw new InvalidOperationException("Entity could not be added to the database.");
            return entry.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await Table.AddRangeAsync(entities);
        }

        public IEnumerable<T?> GetAll(bool tracking = true)
        {
            if (tracking)
                return [.. Table];

            return [.. Table.AsNoTracking()];
        }

        public async Task<List<T>> GetAllAsync(bool tracking = true)
        {
            if (tracking)
                return await Table.ToListAsync();

            return await Table.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetAsync(string id) => await Table.FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));

        public async Task<T?> GetAsync(Expression<Func<T, bool>> expression) => await Table.FirstOrDefaultAsync(expression);

        public IEnumerable<T?> GetWhere(Expression<Func<T, bool>> expression) => Table.Where(expression);

        public bool Remove(T entity)
        {
            var entry = Table.Remove(entity);
            return entry.State == EntityState.Deleted;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            T? model = await Table.FindAsync(Guid.Parse(id));
            if (model == null) return false;
            var entry = Table.Remove(model);
            return entry.State == EntityState.Deleted;
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public bool Update(T entity)
        {
            var entry = Table.Update(entity);
            return entry.State == EntityState.Modified;
        }

        public IQueryable<T> Query()
        {
            return _context.Set<T>();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }
    }
}
