using AutoMapper;
using Entity.DTOs;
using Entity.Model;

namespace Business.Mappers
{
    /// <summary>
    /// Perfil de mapeo para la entidad UserRol
    /// </summary>
    public class UserRolProfile : BaseMapperProfile
    {
        public UserRolProfile()
        {
            // Mapeo de UserRol a UserRolDto
            CreateMap<UserRol, UserRolDto>();
            
            // Mapeo de UserRolDto a UserRol
            CreateMap<UserRolDto, UserRol>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0))
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignorar propiedades de auditoría
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeleteDate, opt => opt.Ignore())
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => true)) // Por defecto, está activo
                .ForMember(dest => dest.User, opt => opt.Ignore()) // Ignorar propiedades de navegación
                .ForMember(dest => dest.Rol, opt => opt.Ignore());
        }
    }
}
