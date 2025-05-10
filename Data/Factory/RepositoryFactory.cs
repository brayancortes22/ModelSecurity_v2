using Data.Interfaces;
using Entity.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Data.Factory
{
    /// <summary>
    /// Implementación de la fábrica de repositorios
    /// </summary>
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Crea un repositorio genérico para el tipo de entidad y tipo de ID especificados
        /// </summary>
        public IGenericRepository<TEntity, TId> CreateRepository<TEntity, TId>()
            where TEntity : class, IEntity
            where TId : IConvertible
        {
            return _serviceProvider.GetRequiredService<IGenericRepository<TEntity, TId>>();
        }

        /// <summary>
        /// Crea un repositorio específico
        /// </summary>
        public TRepository CreateSpecificRepository<TRepository>() where TRepository : class
        {
            return _serviceProvider.GetRequiredService<TRepository>();
        }
    }
}
