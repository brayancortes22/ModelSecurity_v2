using Data.Interfaces;
using Data.Repositories;
using Entity.Contexts;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio específico para la entidad Form
    /// </summary>
    public class FormData : GenericRepository<Form, int>, IGenericRepository<Form, int>
    {
        public FormData(ApplicationDbContext context, ILogger<FormData> logger) 
            : base(context, logger)
        {
        }
        
        // Aquí podrías agregar métodos específicos para Form si son necesarios
    }
}
