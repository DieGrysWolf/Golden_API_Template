using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataServer.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        // Get all entities
        Task<IEnumerable<T>> GetAllAsync();

        // Get by id
        Task<T> GetByIdAsync(Guid id);

        // Add new entity
        Task<bool> AddAsync(T entity);

        // Update existing entity
        Task<bool> UpdateAsync(T entity);

        // Delete entity
        Task<bool> DeleteAsync(Guid id);
    }
}
