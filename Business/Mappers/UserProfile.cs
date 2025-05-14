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
                .ForMember(dest => dest.Active, opt => opt.Ignore())
                .ForMember(dest => dest.Username, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.PersonId, opt => opt.Ignore());
        }
    }
}
