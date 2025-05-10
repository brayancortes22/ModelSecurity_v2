using Business.Base;
using Business.Interfaces;
using Business.Mappers;
using Data;
using Data.Factory;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Implementación de la lógica de negocio para FormModule utilizando AutoMapper
    /// </summary>
    public class AutoMapperFormModuleBusiness : AutoMapperGenericBusiness<FormModule, FormModuleDto, int>, IGenericBusiness<FormModuleDto, int>
    {
        private readonly FormModuleData _formModuleData;

        public AutoMapperFormModuleBusiness(
            IRepositoryFactory repositoryFactory,
            FormModuleData formModuleData,
            ILogger<AutoMapperFormModuleBusiness> logger,
            IMappingService mappingService)
            : base(repositoryFactory, logger, mappingService)
        {
            _formModuleData = formModuleData ?? throw new ArgumentNullException(nameof(formModuleData));
        }

        /// <summary>
        /// Validación específica para ID de FormModule
        /// </summary>
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con una relación Formulario-Módulo con ID inválido: {FormModuleId}", id);
                throw new ValidationException("id", "El ID de la relación Formulario-Módulo debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Validación específica para FormModuleDto
        /// </summary>
        protected override void ValidateDto(FormModuleDto formModuleDto)
        {
            if (formModuleDto == null)
            {
                throw new ValidationException("El objeto relación Formulario-Módulo no puede ser nulo");
            }

            if (formModuleDto.FormId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación Formulario-Módulo con FormId inválido: {FormId}", formModuleDto.FormId);
                throw new ValidationException("FormId", "El ID del formulario debe ser mayor que cero");
            }

            if (formModuleDto.ModuleId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación Formulario-Módulo con ModuleId inválido: {ModuleId}", formModuleDto.ModuleId);
                throw new ValidationException("ModuleId", "El ID del módulo debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Implementación para actualizar parcialmente una entidad FormModule
        /// </summary>
        protected override bool PatchEntityFromDto(FormModuleDto formModuleDto, FormModule formModule)
        {
            bool updated = false;

            // En este caso, solo podemos actualizar los IDs si se proporcionan y son diferentes
            if (formModuleDto.FormId > 0 && formModuleDto.FormId != formModule.FormId)
            {
                formModule.FormId = formModuleDto.FormId;
                updated = true;
            }

            if (formModuleDto.ModuleId > 0 && formModuleDto.ModuleId != formModule.ModuleId)
            {
                formModule.ModuleId = formModuleDto.ModuleId;
                updated = true;
            }

            return updated;
        }
    }
}
