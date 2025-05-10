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
    /// Implementación de la lógica de negocio para RolForm utilizando AutoMapper
    /// </summary>
    public class AutoMapperRolFormBusiness : AutoMapperGenericBusiness<RolForm, RolFormDto, int>, IGenericBusiness<RolFormDto, int>
    {
        private readonly IRolFormRepository _rolFormRepository;

        public AutoMapperRolFormBusiness(
            IRepositoryFactory repositoryFactory,
            IRolFormRepository rolFormRepository,
            ILogger<AutoMapperRolFormBusiness> logger,
            IMappingService mappingService)
            : base(repositoryFactory, logger, mappingService)
        {
            _rolFormRepository = rolFormRepository ?? throw new ArgumentNullException(nameof(rolFormRepository));
        }

        /// <summary>
        /// Validación específica para ID de RolForm
        /// </summary>
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con una relación Rol-Formulario con ID inválido: {RolFormId}", id);
                throw new ValidationException("id", "El ID de la relación Rol-Formulario debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Validación específica para RolFormDto
        /// </summary>
        protected override void ValidateDto(RolFormDto rolFormDto)
        {
            if (rolFormDto == null)
            {
                throw new ValidationException("El objeto relación Rol-Formulario no puede ser nulo");
            }

            if (rolFormDto.RolId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación Rol-Formulario con RolId inválido: {RolId}", rolFormDto.RolId);
                throw new ValidationException("RolId", "El ID del rol debe ser mayor que cero");
            }

            if (rolFormDto.FormId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación Rol-Formulario con FormId inválido: {FormId}", rolFormDto.FormId);
                throw new ValidationException("FormId", "El ID del formulario debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Implementación para actualizar parcialmente una entidad RolForm
        /// </summary>
        protected override bool PatchEntityFromDto(RolFormDto rolFormDto, RolForm rolForm)
        {
            bool updated = false;

            if (rolFormDto.RolId > 0 && rolFormDto.RolId != rolForm.RolId)
            {
                rolForm.RolId = rolFormDto.RolId;
                updated = true;
            }

            if (rolFormDto.FormId > 0 && rolFormDto.FormId != rolForm.FormId)
            {
                rolForm.FormId = rolFormDto.FormId;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(rolFormDto.Permission) && rolFormDto.Permission != rolForm.Permission)
            {
                rolForm.Permission = rolFormDto.Permission;
                updated = true;
            }

            return updated;
        }

        /// <summary>
        /// Obtener todas las relaciones RolForm para un rol específico
        /// </summary>
        public async Task<IEnumerable<RolFormDto>> GetByRolIdAsync(int rolId)
        {
            if (rolId <= 0)
            {
                throw new ValidationException("rolId", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var rolForms = await _rolFormRepository.GetByRolIdAsync(rolId);
                List<RolFormDto> rolFormsDTO = new List<RolFormDto>();
                
                foreach (var rolForm in rolForms)
                {
                    rolFormsDTO.Add(_mappingService.MapToDto<RolForm, RolFormDto>(rolForm));
                }
                
                return rolFormsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las relaciones Rol-Formulario para el rol con ID {rolId}");
                throw new ExternalServiceException("Base de datos", $"Error al recuperar las relaciones Rol-Formulario para el rol con ID {rolId}", ex);
            }
        }

        /// <summary>
        /// Obtener todas las relaciones RolForm para un formulario específico
        /// </summary>
        public async Task<IEnumerable<RolFormDto>> GetByFormIdAsync(int formId)
        {
            if (formId <= 0)
            {
                throw new ValidationException("formId", "El ID del formulario debe ser mayor que cero");
            }

            try
            {
                var rolForms = await _rolFormRepository.GetByFormIdAsync(formId);
                List<RolFormDto> rolFormsDTO = new List<RolFormDto>();
                
                foreach (var rolForm in rolForms)
                {
                    rolFormsDTO.Add(_mappingService.MapToDto<RolForm, RolFormDto>(rolForm));
                }
                
                return rolFormsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las relaciones Rol-Formulario para el formulario con ID {formId}");
                throw new ExternalServiceException("Base de datos", $"Error al recuperar las relaciones Rol-Formulario para el formulario con ID {formId}", ex);
            }
        }
    }
}
