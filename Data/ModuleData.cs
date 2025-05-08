using Data.Interfaces;
using Data.Repositories;
using Entity.Contexts;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio específico para la entidad Module
    /// </summary>
    public class ModuleData : GenericRepository<Module, int>, IGenericRepository<Module, int>
    {
        public ModuleData(ApplicationDbContext context, ILogger<ModuleData> logger) 
            : base(context, logger)
        {
        }
        
        // Aquí podrías agregar métodos específicos para Module si son necesarios
    }
}
