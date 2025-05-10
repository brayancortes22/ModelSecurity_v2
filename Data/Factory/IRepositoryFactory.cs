using Data.Interfaces;
using Entity.Interfaces;
using System;

namespace Data.Factory
{
    /// <summary>
    /// Interfaz para fábrica de repositorios
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Crea un repositorio genérico para el tipo de entidad y tipo de ID especificados
        /// </summary>
        /// <typeparam name="TEntity">Tipo de entidad</typeparam>
        /// <typeparam name="TId">Tipo de ID</typeparam>
        /// <returns>Instancia del repositorio</returns>
        IGenericRepository<TEntity, TId> CreateRepository<TEntity, TId>()
            where TEntity : class, IEntity
            where TId : IConvertible;

        /// <summary>
        /// Crea un repositorio específico
        /// </summary>
        /// <typeparam name="TRepository">Tipo del repositorio</typeparam>
        /// <returns>Instancia del repositorio específico</returns>
        TRepository CreateSpecificRepository<TRepository>() where TRepository : class;
    }
}
