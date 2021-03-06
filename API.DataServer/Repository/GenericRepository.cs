using API.DataServer.Data;
using API.DataServer.IRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataServer.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected ApiDbContext _context;
        internal DbSet<T> dbSet;
        protected readonly ILogger _logger;

        public GenericRepository(ApiDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            dbSet = context.Set<T>();
        }

        // This should be overridden in most implementations
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = await dbSet.ToListAsync();
            return entities;
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            return true;
        }

        // This will be unique to every repository
        public virtual async Task<bool> UpdateAsync(T entity)
        {            
            var result = dbSet.Update(entity);

            if(result.State != EntityState.Modified)
                return false;

            return true;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await dbSet.FindAsync(id);

            if (entity == null)
                return false;

            dbSet.Remove(entity);
            return true;
        }
    }
}
