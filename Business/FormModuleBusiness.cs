using Business.Interfaces;
using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los módulos de formulario en el sistema.
    /// </summary>
    public class FormModuleBusiness
    {
        private readonly FormModuleData _formModuleData;
        private readonly ILogger<FormModuleBusiness> _logger;

        public FormModuleBusiness(FormModuleData formModuleData, ILogger<FormModuleBusiness> logger)
        {
            _formModuleData = formModuleData;
            _logger = logger;
        }

        // Método para obtener todos los módulos de formulario como DTOs
        public async Task<IEnumerable<FormModuleDto>> GetAllFormModulesAsync()
        {
            try
            {
                var formModules = await _formModuleData.GetAllAsync();
                return MapToDTOList(formModules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos de formulario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de módulos de formulario", ex);
            }
        }

        // Método para obtener un módulo de formulario por ID como DTO
        public async Task<FormModuleDto> GetFormModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un módulo de formulario con ID inválido: {FormModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del módulo de formulario debe ser mayor que cero");
            }

            try
            {
                var formModule = await _formModuleData.GetByIdAsync(id);
                if (formModule == null)
                {
                    _logger.LogInformation("No se encontró ningún módulo de formulario con ID: {FormModuleId}", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                return MapToDTO(formModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo de formulario con ID: {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el módulo de formulario con ID {id}", ex);
            }
        }

        // Método para crear un módulo de formulario desde un DTO
        public async Task<FormModuleDto> CreateFormModuleAsync(FormModuleDto formModuleDto)
        {
            try
            {
                ValidateFormModule(formModuleDto);
                var formModule = MapToEntity(formModuleDto);
                var formModuleCreado = await _formModuleData.CreateAsync(formModule);
                return MapToDTO(formModuleCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo módulo de formulario");
                throw new ExternalServiceException("Base de datos", "Error al crear el módulo de formulario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateFormModule(FormModuleDto formModuleDto)
        {
            if (formModuleDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto módulo de formulario no puede ser nulo");
            }

            if (formModuleDto.FormId <= 0 || formModuleDto.ModuleId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un módulo de formulario con FormId o ModuleId inválidos");
                throw new Utilities.Exceptions.ValidationException("FormId/ModuleId", "El FormId y el ModuleId del módulo de formulario son obligatorios y deben ser mayores que cero");
            }
        }

        // Método para actualizar una relación formulario-módulo existente (reemplazo completo)
        public async Task<FormModuleDto> UpdateFormModuleAsync(int id, FormModuleDto formModuleDto)
        {
            if (id <= 0 || id != formModuleDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar una relación formulario-módulo con ID inválido o no coincidente: {FormModuleId}, DTO ID: {DtoId}", id, formModuleDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateFormModule(formModuleDto); // Reutilizamos la validación

            try
            {
                var existingRelation = await _formModuleData.GetByIdAsync(id);
                if (existingRelation == null)
                {
                    _logger.LogInformation("No se encontró la relación formulario-módulo con ID {FormModuleId} para actualizar", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                // Mapear el DTO a la entidad existente (actualización completa)
                existingRelation.FormId = formModuleDto.FormId;
                existingRelation.ModuleId = formModuleDto.ModuleId;
                existingRelation.StatusProcedure = formModuleDto.StatusProcedure;

                await _formModuleData.UpdateAsync(existingRelation);
                return MapToDTO(existingRelation);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Podría ser violación de FK
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar la relación formulario-módulo con ID {FormModuleId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación formulario-módulo con ID {id}. Verifique la existencia de Form y Module.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar la relación formulario-módulo con ID {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación formulario-módulo con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente una relación formulario-módulo (PATCH)
        // Principalmente para actualizar StatusProcedure
        public async Task<FormModuleDto> PatchFormModuleAsync(int id, FormModuleDto formModuleDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a una relación formulario-módulo con ID inválido: {FormModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación formulario-módulo debe ser mayor que cero.");
            }

            try
            {
                var existingRelation = await _formModuleData.GetByIdAsync(id);
                if (existingRelation == null)
                {
                    _logger.LogInformation("No se encontró la relación formulario-módulo con ID {FormModuleId} para aplicar patch", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                bool updated = false;

                // Actualizar StatusProcedure si se proporciona y es diferente
                if (formModuleDto.StatusProcedure != null && formModuleDto.StatusProcedure != existingRelation.StatusProcedure)
                {
                    existingRelation.StatusProcedure = formModuleDto.StatusProcedure;
                    updated = true;
                }

                // No actualizamos FormId o ModuleId en PATCH

                if (updated)
                {
                    await _formModuleData.UpdateAsync(existingRelation);
                     _logger.LogInformation("Patch aplicado a relación formulario-módulo con ID: {FormModuleId}", id);
                }
                else
                {
                    _logger.LogInformation("No se realizaron cambios en la relación formulario-módulo con ID {FormModuleId} durante el patch.", id);
                }

                return MapToDTO(existingRelation);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch a la relación formulario-módulo con ID {FormModuleId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la relación formulario-módulo con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch a la relación formulario-módulo con ID {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la relación formulario-módulo con ID {id}", ex);
            }
        }

        // Método para eliminar una relación formulario-módulo (DELETE persistente)
        public async Task DeleteFormModuleAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar una relación formulario-módulo con ID inválido: {FormModuleId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación formulario-módulo debe ser mayor a 0");
            }
            try
            {
                 var existingRelation = await _formModuleData.GetByIdAsync(id); // Verificar existencia
                if (existingRelation == null)
                {
                     _logger.LogInformation("No se encontró la relación formulario-módulo con ID {FormModuleId} para eliminar", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                bool deleted = await _formModuleData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Relación formulario-módulo con ID {FormModuleId} eliminada exitosamente", id);
                }
                else
                {
                     _logger.LogWarning("No se pudo eliminar la relación formulario-módulo con ID {FormModuleId}.", id);
                    throw new EntityNotFoundException("FormModule", id); 
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar la relación formulario-módulo con ID {FormModuleId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar la relación formulario-módulo con ID {id}", ex);
            }
        }

        // Método para desactivar (eliminar lógicamente) una relación formulario-módulo
        public async Task SoftDeleteFormModuleAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar soft-delete a una relación formulario-módulo con ID inválido: {FormModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación formulario-módulo debe ser mayor a 0");
            }

            try
            {
                var existingRelation = await _formModuleData.GetByIdAsync(id);
                if (existingRelation == null)
                {
                    _logger.LogInformation("No se encontró la relación formulario-módulo con ID {FormModuleId} para soft-delete", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                await _formModuleData.UpdateAsync(existingRelation); 
                _logger.LogInformation("Relación formulario-módulo con ID {FormModuleId} desactivada (soft-delete) exitosamente", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete de la relación formulario-módulo con ID {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la relación formulario-módulo con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al realizar soft-delete de la relación formulario-módulo con ID {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la relación formulario-módulo con ID {id}", ex);
            }
        }

        //Funciones de mapeos 
        // Método para mapear de FormModule a FormModuleDto
        private FormModuleDto MapToDTO(FormModule formModule)
        {
            return new FormModuleDto
            {
                Id = formModule.Id,
                StatusProcedure = formModule.StatusProcedure,
                FormId = formModule.FormId,
                ModuleId = formModule.ModuleId
            };
        }

        // Método para mapear de FormModuleDto a FormModule
        private FormModule MapToEntity(FormModuleDto formModuleDto)
        {
            return new FormModule
            {
                Id = formModuleDto.Id,
                StatusProcedure = formModuleDto.StatusProcedure,
                FormId = formModuleDto.FormId,
                ModuleId = formModuleDto.ModuleId
            };
        }

        // Método para mapear una lista de FormModule a una lista de FormModuleDto
        private IEnumerable<FormModuleDto> MapToDTOList(IEnumerable<FormModule> formModules)
        {
            var formModulesDTO = new List<FormModuleDto>();
            foreach (var formModule in formModules)
            {
                formModulesDTO.Add(MapToDTO(formModule));
            }
            return formModulesDTO;
        }
    }
}
