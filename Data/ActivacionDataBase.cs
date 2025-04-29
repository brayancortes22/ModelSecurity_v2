using System;
using System.Threading.Tasks;
using Data.Interfaces;
using Entity.Interfaces;
using Microsoft.EntityFrameworkCore;
using Entity.Contexts;

namespace Data
{
    /// <summary>
    /// Clase base para operaciones de activación/desactivación de entidades
    /// </summary>
    /// <typeparam name="T">Tipo de entidad que implementa IActivable</typeparam>
    public abstract class ActivacionDataBase<T> : IActivacionData<T, int> where T : class, IActivable
    {
        protected readonly ApplicationDbContext _context;

        public ActivacionDataBase(ApplicationDbContext context)
        {
            _context = context;
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
                var entidad = await ObtenerPorIdAsync(id);
                if (entidad == null)
                {
                    return false;
                }

                // Cambiar el estado de activación
                entidad.Active = estado;
                
                // Guardar el cambio en la base de datos
                _context.Update(entidad);
                await _context.SaveChangesAsync();
                
                return true;
            }
            catch (Exception)
            {
                // Manejar excepciones según sea necesario
                return false;
            }
        }

        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <returns>La entidad si existe, null en caso contrario</returns>
        public abstract Task<T> ObtenerPorIdAsync(int id);
    }
}