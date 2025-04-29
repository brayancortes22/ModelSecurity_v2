using System;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    /// <summary>
    /// Interfaz para operaciones de activación/desactivación de entidades en la capa de negocio
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <typeparam name="TKey">Tipo de clave primaria</typeparam>
    public interface IActivacionBusiness<T, TKey> where T : class
    {
        /// <summary>
        /// Activa una entidad por su ID
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <returns>True si se activó correctamente, false en caso contrario</returns>
        Task<bool> ActivarAsync(TKey id);
        
        /// <summary>
        /// Desactiva una entidad por su ID
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <returns>True si se desactivó correctamente, false en caso contrario</returns>
        Task<bool> DesactivarAsync(TKey id);
        
        /// <summary>
        /// Cambia el estado de activación de una entidad
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <param name="estado">Nuevo estado (true = activo, false = inactivo)</param>
        /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
        Task<bool> CambiarEstadoActivacionAsync(TKey id, bool estado);
    }
}