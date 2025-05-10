using AutoMapper;
using Entity.DTOs;
using Entity.Model;

namespace Business.Mappers
{
    /// <summary>
    /// Perfil de mapeo para la entidad User
    /// </summary>
    public class UserProfile : BaseMapperProfile
    {
        public UserProfile()
        {
            // Mapeo de User a UserDto
            CreateMap<User, UserDto>();
            
            // Mapeo de UserDto a User
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0))
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignorar propiedades de auditoría
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeleteDate, opt => opt.Ignore())
                .ForMember(dest => dest.Person, opt => opt.Ignore()) // Ignorar propiedades de navegación
                .ForMember(dest => dest.UserRols, opt => opt.Ignore());
        }
    }
}
