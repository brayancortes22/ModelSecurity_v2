using Business.Base;
using Business.Interfaces;
using Data;
using Data.Factory;
using Data.Interfaces;
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
    /// Clase de negocio encargada de la lógica relacionada con los usuarios del sistema.
    /// </summary>
    public class UserBusiness : GenericBusiness<User, UserDto, int>, IGenericBusiness<UserDto, int>
    {
        private readonly UserData _userDataSpecific;

        public UserBusiness(
            IRepositoryFactory repositoryFactory, 
            UserData userDataSpecific,
            ILogger<UserBusiness> logger)
            : base(repositoryFactory, logger)
        {
            _userDataSpecific = userDataSpecific ?? throw new ArgumentNullException(nameof(userDataSpecific));
        }
        
        public UserBusiness(
            IGenericRepository<User, int> repository, 
            UserData userDataSpecific,
            ILogger<UserBusiness> logger)
            : base(repository, logger)
        {
            _userDataSpecific = userDataSpecific ?? throw new ArgumentNullException(nameof(userDataSpecific));
        }

        /// <summary>
        /// Autentica a un usuario verificando sus credenciales
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="password">Contraseña</param>
        /// <returns>DTO del usuario si la autenticación es exitosa, null en caso contrario</returns>
        public async Task<UserDto?> AuthenticateAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return null;
                }

                // Buscar usuario por nombre de usuario
                var user = await _userDataSpecific.GetByUsernameAsync(username);
                
                // Verificar si el usuario existe
                if (user == null)
                {
                    _logger.LogWarning("Intento de autenticación fallido: usuario {Username} no encontrado", username);
                    return null;
                }

                // Verificar si la contraseña es correcta (comparación directa sin encriptación)
                bool isPasswordValid = password == user.Password;
                
                if (!isPasswordValid)
                {
                    _logger.LogWarning("Intento de autenticación fallido: contraseña incorrecta para usuario {Username}", username);
                    return null;
                }

                _logger.LogInformation("Autenticación exitosa para usuario: {Username}", username);
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la autenticación de usuario {Username}", username);
                throw new ExternalServiceException("Autenticación", "Error al intentar autenticar al usuario", ex);
            }
        }

        /// <summary>
        /// Actualiza la contraseña de un usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="newPassword">Nueva contraseña</param>
        /// <returns>Task completado cuando la operación finaliza</returns>
        public async Task UpdatePasswordAsync(int id, string newPassword)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar la contraseña de un usuario con ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                _logger.LogWarning("Se intentó actualizar la contraseña con un valor vacío para el usuario ID: {UserId}", id);
                throw new ValidationException("newPassword", "La nueva contraseña no puede estar vacía");
            }

            try
            {
                var existingUser = await GetByIdAsync(id);
                
                // Crear un DTO con solo la contraseña para aplicar el parche
                var userPatchDto = new UserDto 
                { 
                    Id = id,
                    Password = newPassword
                };
                
                // Utilizar el método de parche existente
                await PatchAsync(id, userPatchDto);
                
                _logger.LogInformation("Contraseña actualizada correctamente para el usuario con ID {UserId}", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al actualizar la contraseña del usuario {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la contraseña del usuario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar la contraseña del usuario {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la contraseña del usuario con ID {id}", ex);
            }
        }

        // Implementaciones específicas de los métodos abstractos
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con un usuario con ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }
        }

        protected override void ValidateDto(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ValidationException("El objeto usuario no puede ser nulo");
            }

            // Determinar si estamos en un contexto de creación o actualización
            bool isCreate = userDto.Id == 0;

            if (string.IsNullOrWhiteSpace(userDto.Username))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Username vacío");
                throw new ValidationException("Username", "El Username es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(userDto.Email) || !IsValidEmail(userDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Email vacío o inválido");
                throw new ValidationException("Email", "El Email es obligatorio y debe tener un formato válido");
            }

            if (isCreate)
            {
                if (string.IsNullOrWhiteSpace(userDto.Password))
                {
                    _logger.LogWarning("Se intentó crear un usuario sin contraseña");
                    throw new ValidationException("Password", "La contraseña es obligatoria al crear un usuario");
                }
            }

            if (userDto.PersonId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con PersonId inválido: {PersonId}", userDto.PersonId);
                throw new ValidationException("PersonId", "El PersonId asociado al usuario es obligatorio");
            }
        }

        protected override UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Active = user.Active,
                PersonId = user.PersonId,
                Password = user.Password
            };
        }

        protected override User MapToEntity(UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                Username = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password,
                PersonId = userDto.PersonId,
                Active = userDto.Active
            };
        }

        protected override void UpdateEntityFromDto(UserDto userDto, User user)
        {
            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.PersonId = userDto.PersonId;
            user.Active = userDto.Active;
            
            // También actualizamos la contraseña si se proporciona
            if (!string.IsNullOrWhiteSpace(userDto.Password))
            {
                user.Password = userDto.Password;
            }
        }

        protected override bool PatchEntityFromDto(UserDto userDto, User user)
        {
            bool updated = false;

            // Si el objeto solo contiene un ID y una contraseña, estamos en el caso de cambio de contraseña
            bool isPasswordChangeOnly = 
                !string.IsNullOrEmpty(userDto.Password) && 
                string.IsNullOrEmpty(userDto.Username) && 
                string.IsNullOrEmpty(userDto.Email) && 
                userDto.PersonId == 0;

            if (isPasswordChangeOnly)
            {
                // Actualizar solo la contraseña sin validar otros campos
                _logger.LogInformation("Aplicando PATCH solo para contraseña del usuario {UserId}", user.Id);
                user.Password = userDto.Password;
                return true;
            }

            // Actualizar Username si se proporciona y es diferente
            if (!string.IsNullOrEmpty(userDto.Username) && user.Username != userDto.Username)
            {
                if (string.IsNullOrWhiteSpace(userDto.Username))
                    throw new ValidationException("Username", "El Username no puede estar vacío en PATCH");
                user.Username = userDto.Username;
                updated = true;
            }

            // Actualizar Email si se proporciona y es diferente
            if (!string.IsNullOrEmpty(userDto.Email) && user.Email != userDto.Email)
            {
                if (string.IsNullOrWhiteSpace(userDto.Email) || !IsValidEmail(userDto.Email))
                    throw new ValidationException("Email", "El Email proporcionado no es válido en PATCH");
                user.Email = userDto.Email;
                updated = true;
            }

            // Actualizar PersonId si se proporciona y es diferente
            if (userDto.PersonId > 0 && user.PersonId != userDto.PersonId)
            {
                user.PersonId = userDto.PersonId;
                updated = true;
            }

            // Actualizar contraseña si se proporciona
            if (!string.IsNullOrWhiteSpace(userDto.Password))
            {
                user.Password = userDto.Password;
                updated = true;
                _logger.LogInformation("Contraseña actualizada para el usuario con ID {UserId}", user.Id);
            }

            return updated;
        }

        protected override IEnumerable<UserDto> MapToDtoList(IEnumerable<User> users)
        {
            return users.Select(MapToDto).ToList();
        }

        // Validación simple de formato de email
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
