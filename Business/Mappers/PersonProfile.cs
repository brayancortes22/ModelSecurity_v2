using AutoMapper;
using Entity.DTOs;
using Entity.Model;

namespace Business.Mappers
{
    /// <summary>
    /// Perfil de mapeo para la entidad Person
    /// </summary>
    public class PersonProfile : BaseMapperProfile
    {
        public PersonProfile()
        {
            // Mapeo de Person a PersonDto
            CreateMap<Person, PersonDto>()
                // Corregido para concatenar FirstName y LastName correctamente
                .ForMember(dest => dest.Name, opt => 
                    opt.MapFrom(src => $"{src.FirstName} {src.FirstLastName}"));
            
            // Mapeo de PersonDto a Person
            CreateMap<PersonDto, Person>()
                // Solo mapear el Id si es mayor que 0
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0))
                // Mapeos para cada propiedad
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.SecondName))
                .ForMember(dest => dest.FirstLastName, opt => opt.MapFrom(src => src.FirstLastName))
                .ForMember(dest => dest.SecondLastName, opt => opt.MapFrom(src => src.SecondLastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.TypeIdentification, opt => opt.MapFrom(src => src.TypeIdentification))
                .ForMember(dest => dest.NumberIdentification, opt => opt.MapFrom(src => src.NumberIdentification))
                .ForMember(dest => dest.Signing, opt => opt.MapFrom(src => src.Signing))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active))
                // Tratamiento especial para Email con lógica condicional
                .ForMember(dest => dest.Email, opt =>
                    opt.MapFrom(src => string.IsNullOrEmpty(src.Email)
                        ? $"{src.FirstName.ToLower()}.{src.FirstLastName.ToLower()}@example.com"
                        : src.Email))
                // Ignorar propiedades de auditoría, serán manejadas por la capa de datos
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeleteDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                // Ignorar propiedades de navegación
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
