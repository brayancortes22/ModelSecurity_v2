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
    ///Repository encargado de la gestion de la entidad de tol en la base de base de datos 
    /// </summary>
    public class RolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolData> _logger;
        /// <summary>
        /// Constructor que recibe el contexto de la base de datos 
        /// </summary>
        /// <param name="context"> instancia de <see cref="ApplicationDbContext"/>para la conexion con la base de datos</param>
        public RolData(ApplicationDbContext context, ILogger<RolData> logger)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Obtiene todos los Roles almacenados en la base de datos
        /// </summary>
        /// <returns> Lista de Roles </returns>
        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _context.Set<Rol>().ToListAsync();
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Rol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogError($"Error al obtener Rol con ID {id}");
                throw; // Re-lanza la excepcion para que sea manejada en capas superiores
            }

        }

        /// <summary>
        /// Crea un nuevo Rol en la base de datos 
        /// </summary>
        /// <param name="Rol">instancia del Rol a crear.</param>
        /// <returns>el Rol creado</returns>
        public async Task<Rol> CreateAsync(Rol Rol)
        {
            try
            {
                await _context.Set<Rol>().AddAsync(Rol);
                await _context.SaveChangesAsync();
                return Rol;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Rol {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Rol existente en la base de datos 
        /// </summary>
        /// <param name="Rol">Objeto con la infromacion actualizada</param>
        /// <returns>True si la operacion fue exitosa, False en caso contrario.</returns>
            public async Task<bool> UpdateAsync(Rol Rol)
        {
            try
            {
                _context.Set<Rol>().Update(Rol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Rol {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un Rol en la base de datos 
        /// </summary>
        /// <param name="id">Identificador unico del Rol a eliminar</param>
        /// <returns>True si la eliminacion fue exitosa, False en caso contrario.</returns>
            public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var Rol = await _context.Set<Rol>().FindAsync(id);
                if (Rol == null)
                    return false;

                _context.Set<Rol>().Remove(Rol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el Rol {ex.Message}");
                return false;
            }
        }
    } 
}