using AutoMapper;
using Entity.DTOs;
using Entity.Model;

namespace Business.Mappers
{
    /// <summary>
    /// Perfil de mapeo para la entidad Form
    /// </summary>
    public class FormProfile : BaseMapperProfile
    {
        public FormProfile()
        {
            // Mapeo de Form a FormDto
            CreateMap<Form, FormDto>();
            
            // Mapeo de FormDto a Form
            CreateMap<FormDto, Form>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0))
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Ignorar propiedades de auditoría
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeleteDate, opt => opt.Ignore())
                .ForMember(dest => dest.FormModules, opt => opt.Ignore()) // Ignorar propiedades de navegación
                .ForMember(dest => dest.RolForms, opt => opt.Ignore());
        }
    }
}
