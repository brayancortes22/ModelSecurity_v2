using Business.Base;
using Business.Interfaces;
using Business.Mappers;
using Data.Factory;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Business
{
    /// <summary>
    /// Implementación de la lógica de negocio para Person utilizando AutoMapper
    /// </summary>
    public class AutoMapperPersonBusiness : AutoMapperGenericBusiness<Person, PersonDto, int>, IGenericBusiness<PersonDto, int>
    {
        public AutoMapperPersonBusiness(
            IRepositoryFactory repositoryFactory,
            ILogger<AutoMapperPersonBusiness> logger,
            IMappingService mappingService)
            : base(repositoryFactory, logger, mappingService)
        {
        }

        /// <summary>
        /// Validación específica para Person antes de guardar
        /// </summary>
        protected override void ValidateBeforeSave(PersonDto dto)
        {
            base.ValidateBeforeSave(dto);

            // Validaciones adicionales específicas para Person
            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                throw new Utilities.exeptions.BusinessExceptio("El nombre de la persona es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(dto.FirstLastName))
            {
                throw new Utilities.exeptions.BusinessExceptio("El apellido de la persona es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new Utilities.exeptions.BusinessExceptio("El email de la persona es obligatorio");
            }

            // Validar formato de email
            if (!dto.Email.Contains("@") || !dto.Email.Contains("."))
            {
                throw new Utilities.exeptions.BusinessExceptio("El formato del email no es válido");
            }
        }
    }
}
