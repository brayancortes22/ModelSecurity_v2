using AutoMapper;
using Entity.Interfaces;

namespace Business.Mappers
{
    /// <summary>
    /// Interfaz para marcar perfiles de mapeo para AutoMapper
    /// </summary>
    public interface IMapperProfile { }

    /// <summary>
    /// Clase base para los perfiles de mapeo
    /// </summary>
    public abstract class BaseMapperProfile : Profile, IMapperProfile
    {
        protected BaseMapperProfile()
        {
            // Configuración general que aplica a todos los mapeos
        }
    }
}
