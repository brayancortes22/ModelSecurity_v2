using AutoMapper;
using Entity.DTOs;
using Entity.Model;

namespace Business.Mappers
{
    /// <summary>
    /// Perfil de mapeo para la entidad ChangeLog
    /// </summary>
    public class ChangeLogProfile : BaseMapperProfile
    {
        public ChangeLogProfile()
        {
            // Mapeo de ChangeLog a ChangeLogDto
            CreateMap<ChangeLog, ChangeLogDto>();
            
            // Mapeo de ChangeLogDto a ChangeLog
            CreateMap<ChangeLogDto, ChangeLog>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0))
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // Se establecerá al guardar
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeleteDate, opt => opt.Ignore());
        }
    }
}
