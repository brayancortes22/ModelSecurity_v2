using Business.Base;
using Business.Interfaces;
using Business.Mappers;
using Data;
using Data.Factory;
using Data.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los módulos de formulario en el sistema.
    /// </summary>
    public class FormModuleBusiness : GenericBusiness<FormModule, FormModuleDto, int>, IGenericBusiness<FormModuleDto, int>
    {
        private readonly FormModuleData _formModuleDataSpecific;
        private readonly IMappingService _mappingService;

        public FormModuleBusiness(
            IRepositoryFactory repositoryFactory,
            FormModuleData formModuleDataSpecific,
            ILogger<FormModuleBusiness> logger,
            IMappingService mappingService)
            : base(repositoryFactory, logger)
        {
            _formModuleDataSpecific = formModuleDataSpecific ?? throw new ArgumentNullException(nameof(formModuleDataSpecific));
            _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
        }

        public FormModuleBusiness(
            IGenericRepository<FormModule, int> repository,
            FormModuleData formModuleDataSpecific,
            ILogger<FormModuleBusiness> logger,
            IMappingService mappingService)
            : base(repository, logger)
        {
            _formModuleDataSpecific = formModuleDataSpecific ?? throw new ArgumentNullException(nameof(formModuleDataSpecific));
            _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
        }

        /// <summary>
        /// Obtiene todos los formularios asignados a un módulo específico
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        public async Task<IEnumerable<FormModuleDto>> GetFormsByModuleIdAsync(int moduleId)
        {
            if (moduleId <= 0)
            {
                _logger.LogWarning("Se intentó obtener formularios con ID de módulo inválido: {ModuleId}", moduleId);
                throw new ValidationException("moduleId", "El ID del módulo debe ser mayor que cero");
            }

            try
            {
                var moduleForms = await _formModuleDataSpecific.GetFormsByModuleIdAsync(moduleId);
                return MapToDtoList(moduleForms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener formularios del módulo con ID: {ModuleId}", moduleId);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar los formularios para el módulo con ID {moduleId}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los módulos asignados a un formulario específico
        /// </summary>
        /// <param name="formId">ID del formulario</param>
        public async Task<IEnumerable<FormModuleDto>> GetModulesByFormIdAsync(int formId)
        {
            if (formId <= 0)
            {
                _logger.LogWarning("Se intentó obtener módulos con ID de formulario inválido: {FormId}", formId);
                throw new ValidationException("formId", "El ID del formulario debe ser mayor que cero");
            }

            try
            {
                var formModules = await _formModuleDataSpecific.GetModulesByFormIdAsync(formId);
                return MapToDtoList(formModules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener módulos del formulario con ID: {FormId}", formId);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar los módulos para el formulario con ID {formId}", ex);
            }
        }

        /// <summary>
        /// Sobrescribe el método para validar un ID
        /// </summary>
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar una operación con ID inválido: {Id}", id);
                throw new ValidationException("Id", "El ID debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Sobrescribe el método para validar un DTO de FormModule
        /// </summary>
        protected override void ValidateDto(FormModuleDto dto)
        {
            if (dto == null)
            {
                throw new ValidationException("El objeto módulo de formulario no puede ser nulo");
            }

            if (dto.FormId <= 0 || dto.ModuleId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un módulo de formulario con FormId o ModuleId inválidos");
                throw new ValidationException("FormId/ModuleId", "El FormId y el ModuleId del módulo de formulario son obligatorios y deben ser mayores que cero");
            }
        }

        

        /// <summary>
        /// Actualiza una entidad existente a partir de un DTO
        /// </summary>
        protected override void UpdateEntityFromDto(FormModuleDto dto, FormModule entity)
        {
            _mappingService.UpdateEntityFromDto(dto, entity);
        }

        /// <summary>
        /// Aplica cambios parciales a una entidad existente a partir de un DTO
        /// </summary>
        protected override bool PatchEntityFromDto(FormModuleDto dto, FormModule entity)
        {
            bool changed = false;

            if (!string.IsNullOrEmpty(dto.StatusProcedure) && dto.StatusProcedure != entity.StatusProcedure)
            {
                entity.StatusProcedure = dto.StatusProcedure;
                changed = true;
            }

            if (dto.FormId > 0 && dto.FormId != entity.FormId)
            {
                entity.FormId = dto.FormId;
                changed = true;
            }

            if (dto.ModuleId > 0 && dto.ModuleId != entity.ModuleId)
            {
                entity.ModuleId = dto.ModuleId;
                changed = true;
            }

            return changed;
        }

        /// <summary>
        /// Mapea una colección de entidades a sus DTOs correspondientes
        /// </summary>
        protected override IEnumerable<FormModuleDto> MapToDtoList(IEnumerable<FormModule> entities)
        {
            return _mappingService.MapCollectionToDto<FormModule, FormModuleDto>(entities);
        }

        protected override FormModuleDto MapToDto(FormModule entity)
        {
            throw new NotImplementedException();
        }

        protected override FormModule MapToEntity(FormModuleDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
