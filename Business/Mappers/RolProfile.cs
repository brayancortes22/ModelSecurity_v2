using AutoMapper;
using Entity.DTOs;
using Entity.Model;

namespace Business.Mappers
{
    /// <summary>
    /// Perfil de mapeo para la entidad Rol
    /// </summary>
    public class RolProfile : BaseMapperProfile
    {        public RolProfile()
        {            // Mapeo de Rol a RolDto
            CreateMap<Rol, RolDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TypeRol, opt => opt.MapFrom(src => src.TypeRol))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active));
            
            // Mapeo de RolDto a Rol
            CreateMap<RolDto, Rol>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0))
                .ForMember(dest => dest.TypeRol, opt => opt.MapFrom(src => src.TypeRol))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active));
        }
    }
}
