using Business.Interfaces;
using System;

namespace Business.Factory
{
    /// <summary>
    /// Interfaz para fábrica de servicios de negocio
    /// </summary>
    public interface IBusinessFactory
    {
        /// <summary>
        /// Crea un servicio de negocio genérico para el tipo de DTO y tipo de ID especificados
        /// </summary>
        /// <typeparam name="TDto">Tipo de DTO</typeparam>
        /// <typeparam name="TId">Tipo de ID</typeparam>
        /// <returns>Instancia del servicio de negocio</returns>
        IGenericBusiness<TDto, TId> CreateBusiness<TDto, TId>()
            where TDto : class
            where TId : IConvertible;

        /// <summary>
        /// Crea un servicio de negocio específico
        /// </summary>
        /// <typeparam name="TBusiness">Tipo del servicio de negocio</typeparam>
        /// <returns>Instancia del servicio de negocio específico</returns>
        TBusiness CreateSpecificBusiness<TBusiness>() where TBusiness : class;
    }
}
