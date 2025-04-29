using System;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    /// <summary>
    /// Interfaz para operaciones de activación y desactivación de entidades
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <typeparam name="TKey">Tipo de clave primaria de la entidad</typeparam>
    public interface IActivacionData<T, TKey> where T : class
    {
        /// <summary>
        /// Cambia el estado de activación de una entidad
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <param name="estado">Nuevo estado (true = activo, false = inactivo)</param>
        /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
        Task<bool> CambiarEstadoActivacionAsync(TKey id, bool estado);
        
        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <returns>La entidad si existe, null en caso contrario</returns>
        Task<T> ObtenerPorIdAsync(TKey id);
    }
}