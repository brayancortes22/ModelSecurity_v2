using AutoMapper;
using Entity.DTOs;
using Entity.Model;

namespace Business.Mappers
{
    /// <summary>
    /// Perfil de mapeo para la entidad Rol
    /// </summary>
    public class RolProfile : BaseMapperProfile
    {
        public RolProfile()
        {
            // Mapeo de Rol a RolDto
            CreateMap<Rol, RolDto>();
            
            // Mapeo de RolDto a Rol
            CreateMap<RolDto, Rol>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0))
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignorar propiedades de auditoría
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeleteDate, opt => opt.Ignore())
                .ForMember(dest => dest.RolForms, opt => opt.Ignore()) // Ignorar propiedades de navegación
                .ForMember(dest => dest.UserRols, opt => opt.Ignore());
        }
    }
}
