using Business.Base;
using Business.Interfaces;
using Business.Mappers;
using Data.Factory;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

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
        /// Validación específica para ID de Rol
        /// </summary>
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con un rol con ID inválido: {RolId}", id);
                throw new ValidationException("id", "El ID del rol debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Validación específica para RolDto
        /// </summary>
        protected override void ValidateDto(RolDto rolDto)
        {
            if (rolDto == null)
            {
                throw new ValidationException("El objeto rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(rolDto.TypeRol))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con TypeRol vacío");
                throw new ValidationException("TypeRol", "El tipo de rol es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(rolDto.Description))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con Description vacío");
                throw new ValidationException("Description", "La descripción del rol es obligatoria");
            }
        }

        /// <summary>
        /// Implementación para actualizar parcialmente una entidad Rol
        /// </summary>
        protected override bool PatchEntityFromDto(RolDto rolDto, Rol rol)
        {
            bool updated = false;

            if (!string.IsNullOrWhiteSpace(rolDto.TypeRol) && rolDto.TypeRol != rol.TypeRol)
            {
                rol.TypeRol = rolDto.TypeRol;
                updated = true;
            }
            
            if (!string.IsNullOrWhiteSpace(rolDto.Description) && rolDto.Description != rol.Description)
            {
                rol.Description = rolDto.Description;
                updated = true;
            }

            return updated;
        }
    }
}
