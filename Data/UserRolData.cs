using Data.Interfaces;
using Data.Repositories;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    /// <summary>
    /// Repositorio específico para la entidad UserRol
    /// </summary>
    public class UserRolData : GenericRepository<UserRol, int>, IGenericRepository<UserRol, int>
    {
        public UserRolData(ApplicationDbContext context, ILogger<UserRolData> logger) 
            : base(context, logger)
        {
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

        /// <summary>
        /// Obtiene todas las relaciones UserRol para un rol específico
        /// </summary>
        /// <param name="rolId">ID del rol</param>
        /// <returns>Lista de relaciones UserRol para el rol indicado</returns>
        public async Task<IEnumerable<UserRol>> GetUsersByRolIdAsync(int rolId)
        {
            try
            {
                return await _context.Set<UserRol>()
                    .Where(ur => ur.RolId == rolId && ur.Active)
                    .Include(ur => ur.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener usuarios para el rol con ID {rolId}");
                throw;
            }
        }
    }
}
