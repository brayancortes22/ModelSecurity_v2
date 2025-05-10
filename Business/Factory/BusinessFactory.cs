using Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Business.Factory
{
    /// <summary>
    /// Implementación de la fábrica de servicios de negocio
    /// </summary>
    public class BusinessFactory : IBusinessFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public BusinessFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Crea un servicio de negocio genérico para el tipo de DTO y tipo de ID especificados
        /// </summary>
        public IGenericBusiness<TDto, TId> CreateBusiness<TDto, TId>()
            where TDto : class
            where TId : IConvertible
        {
            return _serviceProvider.GetRequiredService<IGenericBusiness<TDto, TId>>();
        }

        /// <summary>
        /// Crea un servicio de negocio específico
        /// </summary>
        public TBusiness CreateSpecificBusiness<TBusiness>() where TBusiness : class
        {
            return _serviceProvider.GetRequiredService<TBusiness>();
        }
    }
}
