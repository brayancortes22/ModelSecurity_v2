using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repository encargado de la gestión de la entidad ChangeLog en la base de datos.
    /// </summary>
    public class ChangeLogData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChangeLogData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public ChangeLogData(ApplicationDbContext context, ILogger<ChangeLogData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los ChangeLog almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de ChangeLog.</returns>
        public async Task<IEnumerable<ChangeLog>> GetAllAsync()
        {
            return await _context.Set<ChangeLog>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un ChangeLog por su ID.
        /// </summary>
        /// <param name="id">Identificador único del ChangeLog.</param>
        /// <returns>El ChangeLog con el ID especificado.</returns>
        public async Task<ChangeLog?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<ChangeLog>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener ChangeLog con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo ChangeLog en la base de datos.
        /// </summary>
        /// <param name="changeLog">Instancia del ChangeLog a crear.</param>
        /// <returns>El ChangeLog creado.</returns>
        public async Task<ChangeLog> CreateAsync(ChangeLog changeLog)
        {
            try
            {
                await _context.Set<ChangeLog>().AddAsync(changeLog);
                await _context.SaveChangesAsync();
                return changeLog;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el ChangeLog {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un ChangeLog existente en la base de datos.
        /// </summary>
        /// <param name="changeLog">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(ChangeLog changeLog)
        {
            try
            {
                _context.Set<ChangeLog>().Update(changeLog);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el ChangeLog {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un ChangeLog en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del ChangeLog a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var changeLog = await _context.Set<ChangeLog>().FindAsync(id);
                if (changeLog == null)
                    return false;

                _context.Set<ChangeLog>().Remove(changeLog);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el ChangeLog {ex.Message}");
                return false;
            }
        }
    }
}


