using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class UserRolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRolData> _logger;

        public UserRolData(ApplicationDbContext context, ILogger<UserRolData> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<UserRol>> GetAllAsync()
        {
            return await _context.Set<UserRol>().ToListAsync();
        }

        public async Task<UserRol?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<UserRol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener Rol-Usuario con ID {id}");
                throw;
            }
        }

        public async Task<UserRol> CreateAsync(UserRol rolUser)
        {
            try
            {
                await _context.Set<UserRol>().AddAsync(rolUser);
                await _context.SaveChangesAsync();
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la relación Rol-Usuario {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UserRol rolUser)
        {
            try
            {
                _context.Set<UserRol>().Update(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la relación Rol-Usuario {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolUser = await _context.Set<UserRol>().FindAsync(id);
                if (rolUser == null)
                    return false;

                _context.Set<UserRol>().Remove(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la relación Rol-Usuario {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Realiza un borrado lógico de una relación UserRol.
        /// </summary>
        /// <param name="id">ID de la relación UserRol a desactivar.</param>
        /// <returns>True si la desactivación fue exitosa, False si no se encontró.</returns>
        public async Task<bool> SoftDeleteAsync(int id)
        {
            try
            {
                var userRol = await _context.Set<UserRol>().FindAsync(id);
                if (userRol == null)
                {
                     _logger.LogInformation("No se encontró UserRol con ID {Id} para borrado lógico.", id);
                    return false;
                }

                if (!userRol.Active)
                {
                    _logger.LogInformation("UserRol con ID {Id} ya estaba inactivo.", id);
                    return true; // Ya está inactivo, considerar éxito
                }

                userRol.Active = false;
                userRol.DeleteDate = DateTime.UtcNow; // Establecer fecha de borrado
                _context.Entry(userRol).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Borrado lógico realizado para UserRol con ID {Id}.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al realizar borrado lógico de UserRol con ID {id}");
                // Podríamos querer lanzar la excepción o devolver false dependiendo del manejo deseado
                return false; 
            }
        }

        /// <summary>
        /// Obtiene todas las relaciones UserRol para un usuario específico
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Lista de relaciones UserRol para el usuario indicado</returns>
        public async Task<IEnumerable<UserRol>> GetRolesByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Set<UserRol>()
                    .Where(ur => ur.UserId == userId && ur.Active)
                    .Include(ur => ur.Rol)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener roles para el usuario con ID {userId}");
                throw;
            }
        }
    }
}
