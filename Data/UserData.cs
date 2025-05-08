using Data.Interfaces;
using Data.Repositories;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Data
{
    /// <summary>
    /// Repositorio específico para la entidad User
    /// </summary>
    public class UserData : GenericRepository<User, int>, IGenericRepository<User, int>
    {
        public UserData(ApplicationDbContext context, ILogger<UserData> logger) 
            : base(context, logger)
        {
        }
        
        /// <summary>
        /// Obtiene un usuario por su nombre de usuario
        /// </summary>
        /// <param name="username">Nombre de usuario a buscar</param>
        /// <returns>Usuario encontrado o null si no existe</returns>
        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                return await _context.Set<User>()
                    .FirstOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener usuario con nombre de usuario {username}: {ex.Message}");
                throw;
            }
        }
    }
}

