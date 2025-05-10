using System;
using System.Threading.Tasks;
using Business.Interfaces;
using Data.Factory;
using Data.Interfaces;
using Entity.Interfaces;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Interfaz para una fábrica de repositorios de activación
    /// </summary>
    public interface IActivacionDataFactory
    {
        /// <summary>
        /// Crea un repositorio de activación para el tipo de entidad especificado
        /// </summary>
        IActivacionData<T, int> CreateActivacionData<T>() where T : class, IActivable;
    }

    /// <summary>
    /// Implementación de la fábrica de repositorios de activación
    /// </summary>
    public class ActivacionDataFactory : IActivacionDataFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ActivacionDataFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IActivacionData<T, int> CreateActivacionData<T>() where T : class, IActivable
        {
            var activacionData = _serviceProvider.GetService(typeof(IActivacionData<T, int>));
            if (activacionData == null)
            {
                throw new InvalidOperationException(
                    $"No se pudo resolver un servicio de tipo IActivacionData<{typeof(T).Name}, int>");
            }
            return (IActivacionData<T, int>)activacionData;
        }
    }
}
