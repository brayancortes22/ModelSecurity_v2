using AutoMapper;
using Entity.DTOs;
using Entity.Model;

namespace Business.Mappers
{
    /// <summary>
    /// Perfil de mapeo para la entidad RolForm
    /// </summary>
    public class RolFormProfile : BaseMapperProfile
    {
        public RolFormProfile()
        {
            // Mapeo de RolForm a RolFormDto
            CreateMap<RolForm, RolFormDto>();

            // Mapeo de RolFormDto a RolForm
            _ = CreateMap<RolFormDto, RolForm>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0))
                .ForMember(dest => dest.RolId, opt => opt.MapFrom(src => src.RolId))
                .ForMember(dest => dest.FormId, opt => opt.MapFrom(src => src.FormId))
                .ForMember(dest => dest.Permission, opt => opt.MapFrom(src => src.Permission));


        }
    }
}
