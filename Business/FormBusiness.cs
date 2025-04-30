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
    /// Clase de negocio encargada de la lógica relacionada con los formularios en el sistema.
    /// </summary>
    public class FormBusiness
    {
        private readonly FormData _formData;
        private readonly ILogger<FormBusiness> _logger;

        public FormBusiness(FormData formData, ILogger<FormBusiness> logger)
        {
            _formData = formData;
            _logger = logger;
        }

        // Método para obtener todos los formularios como DTOs
        public async Task<IEnumerable<FormDto>> GetAllFormsAsync()
        {
            try
            {
                var forms = await _formData.GetAllAsync();
                return MapToDTOList(forms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de formularios", ex);
            }
        }

        // Método para obtener un formulario por ID como DTO
        public async Task<FormDto> GetFormByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un formulario con ID inválido: {FormId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del formulario debe ser mayor que cero");
            }

            try
            {
                var form = await _formData.GetByIdAsync(id);
                if (form == null)
                {
                    _logger.LogInformation("No se encontró ningún formulario con ID: {FormId}", id);
                    throw new EntityNotFoundException("Form", id);
                }

                return MapToDTO(form);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el formulario con ID {id}", ex);
            }
        }

        // Método para crear un formulario desde un DTO
        public async Task<FormDto> CreateFormAsync(FormDto formDto)
        {
            try
            {
                ValidateForm(formDto);
                var form = MapToEntity(formDto);
                form.CreateDate = DateTime.UtcNow;
                var formCreado = await _formData.CreateAsync(form);
                return MapToDTO(formCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo formulario: {Name}", formDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el formulario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateForm(FormDto formDto)
        {
            if (formDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto formulario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(formDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un formulario con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del formulario es obligatorio");
            }
        }

        // Método para actualizar un formulario existente (reemplazo completo)
        public async Task<FormDto> UpdateFormAsync(int id, FormDto formDto)
        {
            if (id <= 0 || id != formDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar un formulario con ID inválido o no coincidente: {FormId}, DTO ID: {DtoId}", id, formDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateForm(formDto); // Reutilizamos la validación

            try
            {
                var existingForm = await _formData.GetByIdAsync(id);
                if (existingForm == null)
                {
                    _logger.LogInformation("No se encontró el formulario con ID {FormId} para actualizar", id);
                    throw new EntityNotFoundException("Form", id);
                }

                // Mapear el DTO a la entidad existente (actualización completa)
                existingForm.Name = formDto.Name;
                existingForm.Description = formDto.Description;
                existingForm.Cuestion = formDto.Cuestion;
                existingForm.TypeCuestion = formDto.TypeCuestion;
                existingForm.Answer = formDto.Answer;
                existingForm.Active = formDto.Active;
                existingForm.UpdateDate = DateTime.UtcNow;

                await _formData.UpdateAsync(existingForm);
                return MapToDTO(existingForm);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar el formulario con ID {FormId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar el formulario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar el formulario con ID {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el formulario con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente un formulario (PATCH)
        public async Task<FormDto> PatchFormAsync(int id, FormDto formDto)
        {
             if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a un formulario con ID inválido: {FormId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del formulario debe ser mayor que cero.");
            }

            try
            {
                var existingForm = await _formData.GetByIdAsync(id);
                if (existingForm == null)
                {
                    _logger.LogInformation("No se encontró el formulario con ID {FormId} para aplicar patch", id);
                    throw new EntityNotFoundException("Form", id);
                }

                bool updated = false;
                existingForm.UpdateDate = DateTime.UtcNow;

                // Actualizar campos si se proporcionan en el DTO y son diferentes
                if (!string.IsNullOrWhiteSpace(formDto.Name) && formDto.Name != existingForm.Name)
                {
                    existingForm.Name = formDto.Name;
                    updated = true;
                }
                if (formDto.Description != null && formDto.Description != existingForm.Description)
                {
                    existingForm.Description = formDto.Description;
                    updated = true;
                }
                if (formDto.Cuestion != null && formDto.Cuestion != existingForm.Cuestion)
                {
                    existingForm.Cuestion = formDto.Cuestion;
                    updated = true;
                }
                if (formDto.TypeCuestion != null && formDto.TypeCuestion != existingForm.TypeCuestion)
                {
                    existingForm.TypeCuestion = formDto.TypeCuestion;
                    updated = true;
                }
                 if (formDto.Answer != null && formDto.Answer != existingForm.Answer)
                {
                    existingForm.Answer = formDto.Answer;
                    updated = true;
                }
                 // No actualizamos Active en PATCH

                if (updated)
                {
                    await _formData.UpdateAsync(existingForm);
                    _logger.LogInformation("Patch aplicado al formulario con ID: {FormId}", id);
                }
                else
                {
                    _logger.LogInformation("No se realizaron cambios en el formulario con ID {FormId} durante el patch.", id);
                }

                return MapToDTO(existingForm);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch al formulario con ID {FormId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el formulario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch al formulario con ID {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el formulario con ID {id}", ex);
            }
        }

        // Método para eliminar un formulario (DELETE persistente)
        public async Task DeleteFormAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar un formulario con ID inválido: {FormId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del formulario debe ser mayor a 0");
            }
            try
            {
                var existingForm = await _formData.GetByIdAsync(id);
                if (existingForm == null)
                {
                     _logger.LogInformation("No se encontró el formulario con ID {FormId} para eliminar", id);
                    throw new EntityNotFoundException("Form", id);
                }

                bool deleted = await _formData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Formulario con ID {FormId} eliminado exitosamente", id);
                }
                else
                {
                     _logger.LogWarning("No se pudo eliminar el formulario con ID {FormId}.", id);
                    throw new EntityNotFoundException("Form", id); 
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Capturar error si hay FKs
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar el formulario con ID {FormId}. Posible violación de FK.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el formulario con ID {id}. Verifique dependencias.", dbEx);
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar el formulario con ID {FormId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar el formulario con ID {id}", ex);
            }
        }

        // Método para desactivar (eliminar lógicamente) un formulario
        public async Task SoftDeleteFormAsync(int id)
        {
             if (id <= 0)
            {
                 _logger.LogWarning("Se intentó realizar soft-delete a un formulario con ID inválido: {FormId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del formulario debe ser mayor a 0");
            }

             try
            {
                var existingForm = await _formData.GetByIdAsync(id);
                if (existingForm == null)
                {
                    _logger.LogInformation("No se encontró el formulario con ID {FormId} para soft-delete", id);
                    throw new EntityNotFoundException("Form", id);
                }

                 if (!existingForm.Active)
                {
                     _logger.LogInformation("El formulario con ID {FormId} ya se encuentra inactivo.", id);
                     return; 
                }

                existingForm.Active = false;
                existingForm.DeleteDate = DateTime.UtcNow;
                await _formData.UpdateAsync(existingForm); 
                 _logger.LogInformation("Formulario con ID {FormId} desactivado (soft-delete) exitosamente", id);
            }
             catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete del formulario con ID {FormId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al desactivar el formulario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error general al realizar soft-delete del formulario con ID {FormId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al desactivar el formulario con ID {id}", ex);
            }
        }

        // Método para activar un formulario
        public async Task ActivateFormAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó activar un usuario con un ID invalido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor a 0");
            }
            try
            {
                var formToActivate = await _formData.GetByIdAsync(id);
                if (formToActivate == null)
                {
                    _logger.LogInformation("No se encontró el formulario con ID {FormId} para activar", id);
                    throw new EntityNotFoundException("Form", id);
                }

                if (formToActivate.Active)
                {
                    _logger.LogInformation("El formulario con ID {FormId} ya está activo.", id);
                    return;
                }

                formToActivate.Active = true;
                // Considerar limpiar DeleteDate y actualizar UpdateDate si existen
                // formToActivate.DeleteDate = null;
                // formToActivate.UpdateDate = DateTime.UtcNow;
                await _formData.UpdateAsync(formToActivate);

                _logger.LogInformation("Usuario con ID {UserId} marcado como activo.", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al activar formulario {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al activar el formulario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al activar formulario {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al activar el formulario con ID {id}", ex);
            }
        }

        //Funciones de mapeos 
        // Método para mapear de Form a FormDto
        private FormDto MapToDTO(Form form)
        {
            return new FormDto
            {
                Id = form.Id,
                Name = form.Name,
                Description = form.Description,
                Cuestion = form.Cuestion,
                TypeCuestion = form.TypeCuestion,
                Answer = form.Answer,
                Active = form.Active
            };
        }

        // Método para mapear de FormDto a Form
        private Form MapToEntity(FormDto formDto)
        {
            return new Form
            {
                Id = formDto.Id,
                Name = formDto.Name,
                Description = formDto.Description,
                Cuestion = formDto.Cuestion,
                TypeCuestion = formDto.TypeCuestion,
                Answer = formDto.Answer,
                Active = formDto.Active
            };
        }

        // Método para mapear una lista de Form a una lista de FormDto
        private IEnumerable<FormDto> MapToDTOList(IEnumerable<Form> forms)
        {
            var formsDTO = new List<FormDto>();
            foreach (var form in forms)
            {
                formsDTO.Add(MapToDTO(form));
            }
            return formsDTO;
        }
    }
}
