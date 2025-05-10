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
            CreateMap<RolFormDto, RolForm>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0))
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignorar propiedades de auditoría
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeleteDate, opt => opt.Ignore())
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => true)) // Por defecto, está activo
                .ForMember(dest => dest.Rol, opt => opt.Ignore()) // Ignorar propiedades de navegación
                .ForMember(dest => dest.Form, opt => opt.Ignore());
        }
    }
}
