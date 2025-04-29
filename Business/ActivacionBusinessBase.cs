using System;
using System.Threading.Tasks;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Interfaces;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase base para operaciones de activación/desactivación en la capa de negocio
    /// </summary>
    /// <typeparam name="T">Tipo de entidad que implementa IActivable</typeparam>
    public abstract class ActivacionBusinessBase<T> : IActivacionBusiness<T, int> where T : class, IActivable
    {
        protected readonly IActivacionData<T, int> _activacionData;

        public ActivacionBusinessBase(IActivacionData<T, int> activacionData)
        {
            _activacionData = activacionData;
        }

        /// <summary>
        /// Activa una entidad por su ID
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <returns>True si se activó correctamente, false en caso contrario</returns>
        public virtual async Task<bool> ActivarAsync(int id)
        {
            try
            {
                return await _activacionData.CambiarEstadoActivacionAsync(id, true);
            }
            catch (Exception ex)
            {
                throw new BusinessExceptio($"Error al activar la entidad con ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Desactiva una entidad por su ID
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <returns>True si se desactivó correctamente, false en caso contrario</returns>
        public virtual async Task<bool> DesactivarAsync(int id)
        {
            try
            {
                return await _activacionData.CambiarEstadoActivacionAsync(id, false);
            }
            catch (Exception ex)
            {
                throw new BusinessExceptio($"Error al desactivar la entidad con ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cambia el estado de activación de una entidad
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <param name="estado">Nuevo estado (true = activo, false = inactivo)</param>
        /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
        public virtual async Task<bool> CambiarEstadoActivacionAsync(int id, bool estado)
        {
            try
            {
                return await _activacionData.CambiarEstadoActivacionAsync(id, estado);
            }
            catch (Exception ex)
            {
                throw new BusinessExceptio($"Error al cambiar el estado de activación de la entidad con ID {id}: {ex.Message}", ex);
            }
        }
    }
}