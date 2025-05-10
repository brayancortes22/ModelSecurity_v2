using AutoMapper;
using Entity.DTOs;
using Entity.Model;

namespace Business.Mappers
{
    /// <summary>
    /// Perfil de mapeo para la entidad Module
    /// </summary>
    public class ModuleProfile : BaseMapperProfile
    {
        public ModuleProfile()
        {
            // Mapeo de Module a ModuleDto
            CreateMap<Module, ModuleDto>();
            
            // Mapeo de ModuleDto a Module
            CreateMap<ModuleDto, Module>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0))
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignorar propiedades de auditoría
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeleteDate, opt => opt.Ignore());
        }
    }
}
