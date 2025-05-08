using Data.Interfaces;
using Data.Repositories;
using Entity.Contexts;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio específico para la entidad Rol
    /// </summary>
    public class RolData : GenericRepository<Rol, int>, IGenericRepository<Rol, int>
    {
        public RolData(ApplicationDbContext context, ILogger<RolData> logger) 
            : base(context, logger)
        {
        }
        
        // Aquí podrías agregar métodos específicos para Rol si son necesarios
    }
}