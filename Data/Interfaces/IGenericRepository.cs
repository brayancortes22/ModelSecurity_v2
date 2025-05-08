using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    /// <summary>
    /// Interfaz genérica para repositorios que gestionan entidades con operaciones CRUD
    /// </summary>
    /// <typeparam name="TEntity">Tipo de entidad</typeparam>
    /// <typeparam name="TId">Tipo del identificador</typeparam>
    public interface IGenericRepository<TEntity, in TId> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(TId id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> PatchAsync(TId id, TEntity entity);
        
        Task<bool> ActivateAsync(TId id);
        Task<bool> DeleteAsync(TId id);
        Task<bool> SoftDeleteAsync(TId id);
    }
}