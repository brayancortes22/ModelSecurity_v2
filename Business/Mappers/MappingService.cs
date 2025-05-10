using AutoMapper;
using Entity.Interfaces;
using System.Collections.Generic;

namespace Business.Mappers
{
    /// <summary>
    /// Interfaz para el servicio de mapeo
    /// </summary>
    public interface IMappingService
    {
        /// <summary>
        /// Método genérico de mapeo entre cualquier tipo de objetos
        /// </summary>
        TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class;

        /// <summary>
        /// Convierte una entidad a un DTO
        /// </summary>
        TDestination MapToDto<TSource, TDestination>(TSource source)
            where TSource : class, IEntity
            where TDestination : class;

        /// <summary>
        /// Convierte un DTO a una entidad
        /// </summary>
        TDestination MapToEntity<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class, IEntity;

        /// <summary>
        /// Actualiza una entidad existente con los datos de un DTO
        /// </summary>
        void UpdateEntityFromDto<TSource, TDestination>(TSource source, TDestination destination)
            where TSource : class
            where TDestination : class, IEntity;

        /// <summary>
        /// Convierte una colección de entidades a una colección de DTOs
        /// </summary>
        IEnumerable<TDestination> MapCollectionToDto<TSource, TDestination>(IEnumerable<TSource> source)
            where TSource : class, IEntity
            where TDestination : class;
    }

    /// <summary>
    /// Implementación del servicio de mapeo utilizando AutoMapper
    /// </summary>
    public class MappingService : IMappingService
    {
        private readonly IMapper _mapper;        public MappingService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class
        {
            return _mapper.Map<TDestination>(source);
        }

        public TDestination MapToDto<TSource, TDestination>(TSource source)
            where TSource : class, IEntity
            where TDestination : class
        {
            return _mapper.Map<TDestination>(source);
        }

        public TDestination MapToEntity<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class, IEntity
        {
            return _mapper.Map<TDestination>(source);
        }

        public void UpdateEntityFromDto<TSource, TDestination>(TSource source, TDestination destination)
            where TSource : class
            where TDestination : class, IEntity
        {
            _mapper.Map(source, destination);
        }

        public IEnumerable<TDestination> MapCollectionToDto<TSource, TDestination>(IEnumerable<TSource> source)
            where TSource : class, IEntity
            where TDestination : class
        {
            return _mapper.Map<IEnumerable<TDestination>>(source);
        }
    }
}
