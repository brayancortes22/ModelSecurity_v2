using AutoMapper;
using Entity.DTOs;
using Entity.Model;

namespace Business.Mappers
{
    /// <summary>
    /// Perfil de mapeo para la entidad FormModule
    /// </summary>
    public class FormModuleProfile : BaseMapperProfile
    {
        public FormModuleProfile()
        {
            // Mapeo de FormModule a FormModuleDto
            CreateMap<FormModule, FormModuleDto>()
                .ForMember(dest => dest.Module, opt => opt.MapFrom(src => src.Module))
                .ForMember(dest => dest.Form, opt => opt.MapFrom(src => src.Form));
            
            // Mapeo de FormModuleDto a FormModule
            CreateMap<FormModuleDto, FormModule>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0))
                .ForMember(dest => dest.FormId, opt => opt.MapFrom(src => src.FormId))
                .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId))
                .ForMember(dest => dest.StatusProcedure, opt => opt.MapFrom(src => src.StatusProcedure))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => true)) // Por defecto, está activo
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignorar propiedades de auditoría
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeleteDate, opt => opt.Ignore())
                .ForMember(dest => dest.Module, opt => opt.Ignore()) // Ignorar propiedades de navegación
                .ForMember(dest => dest.Form, opt => opt.Ignore());
        }
    }
}
