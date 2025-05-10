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
    /// Implementación de la lógica de negocio para Module utilizando AutoMapper
    /// </summary>
    public class AutoMapperModuleBusiness : AutoMapperGenericBusiness<Module, ModuleDto, int>, IGenericBusiness<ModuleDto, int>
    {
        public AutoMapperModuleBusiness(
            IRepositoryFactory repositoryFactory,
            ILogger<AutoMapperModuleBusiness> logger,
            IMappingService mappingService)
            : base(repositoryFactory, logger, mappingService)
        {
        }

        /// <summary>
        /// Validación específica para ID de Módulo
        /// </summary>
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con un módulo con ID inválido: {ModuleId}", id);
                throw new ValidationException("id", "El ID del módulo debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Validación específica para ModuleDto
        /// </summary>
        protected override void ValidateDto(ModuleDto moduleDto)
        {
            if (moduleDto == null)
            {
                throw new ValidationException("El objeto módulo no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(moduleDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un módulo con Name vacío");
                throw new ValidationException("Name", "El Name del módulo es obligatorio");
            }
        }

        /// <summary>
        /// Implementación para actualizar parcialmente una entidad Module
        /// </summary>
        protected override bool PatchEntityFromDto(ModuleDto moduleDto, Module module)
        {
            bool updated = false;

            if (!string.IsNullOrWhiteSpace(moduleDto.Name) && moduleDto.Name != module.Name)
            {
                module.Name = moduleDto.Name;
                updated = true;
            }
            
            if (moduleDto.Description != null && moduleDto.Description != module.Description)
            {
                module.Description = moduleDto.Description;
                updated = true;
            }

            return updated;
        }
    }
}
