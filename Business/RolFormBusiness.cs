using Data;
using Entity.DTOautogestion;
using Entity.DTOautogestion.pivote;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los roles de formulario en el sistema.
    /// </summary>
    public class RolFormBusiness
    {
        private readonly RolFormData _rolFormData;
        private readonly ILogger<RolFormBusiness> _logger;

        public RolFormBusiness(RolFormData rolFormData, ILogger<RolFormBusiness> logger)
        {
            _rolFormData = rolFormData;
            _logger = logger;
        }

        // Método para obtener todos los roles de formulario como DTOs
        public async Task<IEnumerable<RolFormDto>> GetAllRolFormsAsync()
        {
            try
            {
                var rolForms = await _rolFormData.GetAllAsync();
                return MapToDTOList(rolForms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles de formulario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles de formulario", ex);
            }
        }

        // Método para obtener un rol de formulario por ID como DTO
        public async Task<RolFormDto> GetRolFormByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol de formulario con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol de formulario debe ser mayor que cero");
            }

            try
            {
                var rolForm = await _rolFormData.GetByIdAsync(id);
                if (rolForm == null)
                {
                    _logger.LogInformation("No se encontró ningún rol de formulario con ID: {Id}", id);
                    throw new EntityNotFoundException("rolForm", id);
                }

                return MapToDTO(rolForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol de formulario con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol de formulario con ID {id}", ex);
            }
        }

        // Método para crear un rol de formulario desde un DTO
        public async Task<RolFormDto> CreateRolFormAsync(RolFormDto rolFormDto)
        {
            try
            {
                ValidateRolForm(rolFormDto);
                var rolForm = MapToEntity(rolFormDto);
                var rolFormCreado = await _rolFormData.CreateAsync(rolForm);
                return MapToDTO(rolFormCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol de formulario: {Name}", rolFormDto?.Permission ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol de formulario", ex);
            }
        }

        // Método para actualizar una relación rol-formulario existente (reemplazo completo)
        public async Task<RolFormDto> UpdateRolFormAsync(int id, RolFormDto rolFormDto)
        {
            if (id <= 0 || id != rolFormDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar una relación rol-formulario con ID inválido o no coincidente: {RolFormId}, DTO ID: {DtoId}", id, rolFormDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateRolForm(rolFormDto); // Reutilizamos la validación

            try
            {
                var existingRolForm = await _rolFormData.GetByIdAsync(id);
                if (existingRolForm == null)
                {
                    _logger.LogInformation("No se encontró la relación rol-formulario con ID {RolFormId} para actualizar", id);
                    throw new EntityNotFoundException("RolForm", id);
                }

                // Mapear el DTO a la entidad existente (actualización completa)
                existingRolForm.RolId = rolFormDto.RolId;
                existingRolForm.FormId = rolFormDto.FormId;
                existingRolForm.Permission = rolFormDto.Permission;

                await _rolFormData.UpdateAsync(existingRolForm);
                return MapToDTO(existingRolForm);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Podría ser violación de FK
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar la relación rol-formulario con ID {RolFormId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación rol-formulario con ID {id}. Verifique la existencia de Rol y Form.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar la relación rol-formulario con ID {RolFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación rol-formulario con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente una relación rol-formulario (PATCH)
        // Principalmente para actualizar Permission
        public async Task<RolFormDto> PatchRolFormAsync(int id, RolFormDto rolFormDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a una relación rol-formulario con ID inválido: {RolFormId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación rol-formulario debe ser mayor que cero.");
            }

            try
            {
                var existingRolForm = await _rolFormData.GetByIdAsync(id);
                if (existingRolForm == null)
                {
                    _logger.LogInformation("No se encontró la relación rol-formulario con ID {RolFormId} para aplicar patch", id);
                    throw new EntityNotFoundException("RolForm", id);
                }

                bool updated = false;

                // Actualizar Permission si se proporciona y es diferente
                if (!string.IsNullOrWhiteSpace(rolFormDto.Permission) && rolFormDto.Permission != existingRolForm.Permission)
                {
                    // Aquí también se podría validar el valor de Permission si hay un conjunto específico permitido
                    existingRolForm.Permission = rolFormDto.Permission;
                    updated = true;
                }

                // No actualizamos RolId o FormId en PATCH

                if (updated)
                {
                    await _rolFormData.UpdateAsync(existingRolForm);
                }
                else
                {
                     _logger.LogInformation("No se realizaron cambios en la relación rol-formulario con ID {RolFormId} durante el patch.", id);
                }

                return MapToDTO(existingRolForm);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch a la relación rol-formulario con ID {RolFormId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la relación rol-formulario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch a la relación rol-formulario con ID {RolFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la relación rol-formulario con ID {id}", ex);
            }
        }

        // Método para eliminar una relación rol-formulario (DELETE persistente)
        public async Task DeleteRolFormAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar una relación rol-formulario con ID inválido: {RolFormId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación rol-formulario debe ser mayor a 0");
            }
            try
            {
                 var existingRolForm = await _rolFormData.GetByIdAsync(id); // Verificar existencia
                if (existingRolForm == null)
                {
                     _logger.LogInformation("No se encontró la relación rol-formulario con ID {RolFormId} para eliminar", id);
                    throw new EntityNotFoundException("RolForm", id);
                }

                bool deleted = await _rolFormData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Relación rol-formulario con ID {RolFormId} eliminada exitosamente", id);
                }
                else
                {
                     _logger.LogWarning("No se pudo eliminar la relación rol-formulario con ID {RolFormId}.", id);
                    throw new EntityNotFoundException("RolForm", id); 
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar la relación rol-formulario con ID {RolFormId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar la relación rol-formulario con ID {id}", ex);
            }
        }

        // Método para desactivar (eliminar lógicamente) una relación rol-formulario
        public async Task SoftDeleteRolFormAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar soft-delete a una relación rol-formulario con ID inválido: {RolFormId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación rol-formulario debe ser mayor a 0");
            }

            try
            {
                var existingRolForm = await _rolFormData.GetByIdAsync(id);
                if (existingRolForm == null)
                {
                    _logger.LogInformation("No se encontró la relación rol-formulario con ID {RolFormId} para soft-delete", id);
                    throw new EntityNotFoundException("RolForm", id);
                }

                if (!existingRolForm.Active)
                {
                    _logger.LogInformation("La relación rol-formulario con ID {RolFormId} ya se encuentra inactiva.", id);
                    return; 
                }

                existingRolForm.Active = false;
                existingRolForm.DeleteDate = DateTime.UtcNow;
                await _rolFormData.UpdateAsync(existingRolForm); 
                _logger.LogInformation("Relación rol-formulario con ID {RolFormId} desactivada (soft-delete) exitosamente", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete de la relación rol-formulario con ID {RolFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la relación rol-formulario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al realizar soft-delete de la relación rol-formulario con ID {RolFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la relación rol-formulario con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRolForm(RolFormDto rolFormDto)
        {
            if (rolFormDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto RolForm no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(rolFormDto.Permission))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol de formulario con permiso vacío");
                throw new Utilities.Exceptions.ValidationException("permission", "El permiso del rol de formulario es obligatorio");
            }
        }

        //Funciones de mapeos 
        // Método para mapear de RolForm a RolFormDto
        private RolFormDto MapToDTO(RolForm rolForm)
        {
            return new RolFormDto
            {
                Id = rolForm.Id,
                Permission = rolForm.Permission,
                RolId = rolForm.RolId,
                FormId = rolForm.FormId,
            };
        }

        // Método para mapear de RolFormDto a RolForm
        private RolForm MapToEntity(RolFormDto rolFormDto)
        {
            return new RolForm
            {
                Id = rolFormDto.Id,
                Permission = rolFormDto.Permission,
                RolId = rolFormDto.RolId,
                FormId = rolFormDto.FormId
            };
        }

        // Método para mapear una lista de RolForm a una lista de RolFormDto
        private IEnumerable<RolFormDto> MapToDTOList(IEnumerable<RolForm> rolForms)
        {
            var rolFormsDTO = new List<RolFormDto>();
            foreach (var rolForm in rolForms)
            {
                rolFormsDTO.Add(MapToDTO(rolForm));
            }
            return rolFormsDTO;
        }
    }
}

