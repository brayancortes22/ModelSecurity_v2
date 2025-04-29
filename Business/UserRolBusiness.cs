using Data;
using Entity.DTOautogestion;
using Entity.DTOautogestion.pivote;
using Microsoft.EntityFrameworkCore; 
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using System;


namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los usuarios y sus roles en el sistema.
    /// </summary>
    public class UserRolBusiness
    {
        private readonly UserRolData _rolUserData;
        private readonly ILogger<UserRolBusiness> _logger;

        public UserRolBusiness(UserRolData rolUserData, ILogger<UserRolBusiness> logger)
        {
            _rolUserData = rolUserData;
            _logger = logger;
        }

        // Método para obtener todos los roles de usuario como DTOs
        public async Task<IEnumerable<UserRolDto>> GetAllRolUsersAsync()
        {
            try
            {
                var rolUsers = await _rolUserData.GetAllAsync();
                return MapToDTOList(rolUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles de usuario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles de usuario", ex);
            }
        }

        // Método para obtener un rol de usuario por ID como DTO
        public async Task<UserRolDto> GetRolUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol de usuario con ID inválido: {RolUserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol de usuario debe ser mayor que cero");
            }

            try
            {
                var rolUser = await _rolUserData.GetByIdAsync(id);
                if (rolUser == null)
                {
                    _logger.LogInformation("No se encontró ningún rol de usuario con ID: {RolUserId}", id);
                    throw new EntityNotFoundException("RolUser", id);
                }

                return MapToDTO(rolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol de usuario con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol de usuario con ID {id}", ex);
            }
        }

        // Método para crear un rol de usuario desde un DTO
        public async Task<UserRolDto> CreateRolUserAsync(UserRolDto rolUserDto)
        {
            try
            {
                ValidateRolUser(rolUserDto);
                var rolUser = MapToEntity(rolUserDto);
                rolUser.CreateDate = DateTime.UtcNow;
                var rolUserCreado = await _rolUserData.CreateAsync(rolUser);
                return MapToDTO(rolUserCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol de usuario: {UserId}, {RolId}", rolUserDto?.UserId ?? 0, rolUserDto?.RolId ?? 0);
                throw new ExternalServiceException("Base de datos", "Error al crear el rol de usuario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRolUser(UserRolDto rolUserDto)
        {
            if (rolUserDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto rol de usuario no puede ser nulo");
            }

            if (rolUserDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol de usuario con UserId inválido");
                throw new Utilities.Exceptions.ValidationException("UserId", "El UserId del rol de usuario es obligatorio y debe ser mayor que cero");
            }

            if (rolUserDto.RolId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol de usuario con RolId inválido");
                throw new Utilities.Exceptions.ValidationException("RolId", "El RolId del rol de usuario es obligatorio y debe ser mayor que cero");
            }
        }

        // Método para actualizar una relación rol-usuario existente (reemplazo completo)
        // Permite cambiar el rol de un usuario o el usuario de un rol, manteniendo el ID de la relación
        public async Task<UserRolDto> UpdateRolUserAsync(int id, UserRolDto rolUserDto)
        {
            if (id <= 0 || id != rolUserDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar una relación rol-usuario con ID inválido o no coincidente: {RolUserId}, DTO ID: {DtoId}", id, rolUserDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateRolUser(rolUserDto); // Reutilizamos la validación

            try
            {
                var existingRolUser = await _rolUserData.GetByIdAsync(id);
                if (existingRolUser == null)
                {
                    _logger.LogInformation("No se encontró la relación rol-usuario con ID {RolUserId} para actualizar", id);
                    throw new EntityNotFoundException("RolUser", id);
                }

                // Mapear el DTO a la entidad existente (actualización completa)
                existingRolUser.UserId = rolUserDto.UserId;
                existingRolUser.RolId = rolUserDto.RolId;
                existingRolUser.UpdateDate = DateTime.UtcNow;

                await _rolUserData.UpdateAsync(existingRolUser);
                return MapToDTO(existingRolUser);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Podría ser violación de FK si UserId o RolId no existen
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar la relación rol-usuario con ID {RolUserId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación rol-usuario con ID {id}. Verifique la existencia de User y Rol.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar la relación rol-usuario con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación rol-usuario con ID {id}", ex);
            }
        }

        // Método para eliminar una relación rol-usuario (DELETE persistente)
        public async Task DeleteRolUserAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar una relación rol-usuario con ID inválido: {RolUserId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación rol-usuario debe ser mayor a 0");
            }
            try
            {
                 var existingRolUser = await _rolUserData.GetByIdAsync(id); // Verificar existencia
                if (existingRolUser == null)
                {
                     _logger.LogInformation("No se encontró la relación rol-usuario con ID {RolUserId} para eliminar", id);
                    throw new EntityNotFoundException("RolUser", id);
                }

                bool deleted = await _rolUserData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Relación rol-usuario con ID {RolUserId} eliminada exitosamente", id);
                }
                else
                {
                     _logger.LogWarning("No se pudo eliminar la relación rol-usuario con ID {RolUserId}.", id);
                    throw new EntityNotFoundException("RolUser", id); 
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar la relación rol-usuario con ID {RolUserId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar la relación rol-usuario con ID {id}", ex);
            }
        }

        // Método para eliminar lógicamente una relación UserRol (soft delete)
        // Renombrado de SoftDeleteStateAsync y corregido
        public async Task SoftDeleteUserRolAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar soft-delete a una relación UserRol con un ID inválido: {UserRolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación UserRol debe ser mayor a 0");
            }
            try
            {
                // Llamar al método SoftDeleteAsync de la capa de datos
                bool success = await _rolUserData.SoftDeleteAsync(id);

                if (!success)
                {
                     // Si SoftDeleteAsync devuelve false, probablemente no encontró la entidad
                     _logger.LogWarning("No se pudo realizar el borrado lógico para UserRol con ID {UserRolId}. La entidad no fue encontrada o ya estaba inactiva.", id);
                     // Lanzar EntityNotFoundException para que el controlador devuelva 404
                     throw new EntityNotFoundException("UserRol", id); 
                }
                 // El log de éxito ya se hace en la capa de datos
                // _logger.LogInformation("Relación UserRol con ID {UserRolId} marcada como inactiva (soft-delete)", id);
            }
            catch (EntityNotFoundException) 
            {
                 // Relanzar si SoftDeleteAsync lanzó (aunque no debería si devuelve false)
                throw; 
            }
            catch (DbUpdateException dbEx) // Capturar explícitamente errores de BD
            {
                _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete de UserRol {UserRolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la relación UserRol con ID {id}", dbEx);
            }
            catch (Exception ex) // Capturar cualquier otro error inesperado
            {
                _logger.LogError(ex, "Error general al realizar soft-delete de UserRol {UserRolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la relación UserRol con ID {id}", ex);
            }
        }

        //Funciones de mapeos 
        // Método para mapear de UserRol a UserRolDto
        private UserRolDto MapToDTO(UserRol rolUser)
        {
            return new UserRolDto
            {
                Id = rolUser.Id,
                UserId = rolUser.UserId,
                RolId = rolUser.RolId
            };
        }

        // Método para mapear de UserRolDto a UserRol
        private UserRol MapToEntity(UserRolDto rolUserDto)
        {
            return new UserRol
            {
                Id = rolUserDto.Id,
                UserId = rolUserDto.UserId,
                RolId = rolUserDto.RolId
            };
        }

        // Método para mapear una lista de UserRol a una lista de UserRolDto
        private IEnumerable<UserRolDto> MapToDTOList(IEnumerable<UserRol> rolUsers)
        {
            var rolUsersDTO = new List<UserRolDto>();
            foreach (var rolUser in rolUsers)
            {
                rolUsersDTO.Add(MapToDTO(rolUser));
            }
            return rolUsersDTO;
        }
    }
}

