using Data.Interfaces;
using Data.Repositories;
using Entity.Contexts;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio específico para la entidad Person
    /// </summary>
    public class PersonData : GenericRepository<Person, int>, IGenericRepository<Person, int>
    {
        public PersonData(ApplicationDbContext context, ILogger<PersonData> logger) 
            : base(context, logger)
        {
        }
        
        // Aquí podrías agregar métodos específicos para Person si son necesarios
    }
}

