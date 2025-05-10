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
    /// Implementación de la lógica de negocio para Rol utilizando AutoMapper
    /// </summary>
    public class AutoMapperRolBusiness : AutoMapperGenericBusiness<Rol, RolDto, int>, IGenericBusiness<RolDto, int>
    {
        public AutoMapperRolBusiness(
            IRepositoryFactory repositoryFactory,
            ILogger<AutoMapperRolBusiness> logger,
            IMappingService mappingService)
            : base(repositoryFactory, logger, mappingService)
        {
        }

        /// <summary>
        /// Validación específica para Rol antes de guardar
        /// </summary>
        protected override void ValidateBeforeSave(RolDto dto)
        {
            base.ValidateBeforeSave(dto);

            // Validaciones adicionales específicas para Rol
            if (string.IsNullOrWhiteSpace(dto.TypeRol))
            {
                throw new Utilities.exeptions.BusinessExceptio("El tipo de rol es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(dto.Description))
            {
                throw new Utilities.exeptions.BusinessExceptio("La descripción del rol es obligatoria");
            }
        }
    }
}
