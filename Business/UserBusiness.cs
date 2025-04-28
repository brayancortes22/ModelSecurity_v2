using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.EntityFrameworkCore; // Para DbUpdateException
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Para validación de Email
using System.Linq;
using System.Text.RegularExpressions; // Para validar Email
using System.Threading.Tasks;
using Utilities.Exceptions;
using BCrypt.Net;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los usuarios del sistema.
    /// </summary>
    public class UserBusiness
    {
        private readonly UserData _userData;
        private readonly ILogger<UserBusiness> _logger;

        public UserBusiness(UserData userData, ILogger<UserBusiness> logger)
        {
            _userData = userData;
            _logger = logger;
        }

        // Método para obtener todos los usuarios como DTOs
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userData.GetAllAsync();
                return MapToDTOList(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de usuarios", ex);
            }
        }

        // Método para obtener un usuario por ID como DTO
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un usuario con ID inválido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                return MapToDTO(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario con ID {id}", ex);
            }
        }

        // Método para crear un usuario desde un DTO (modificado)
        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            try
            {
                ValidateUser(userDto, isCreate: true);
                var user = MapToEntity(userDto);

                // Establecer siempre como activo al crear un usuario nuevo
                user.Active = true;

                // Hashear la contraseña antes de guardarla
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                var userCreado = await _userData.CreateAsync(user);
                return MapToDTO(userCreado);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al crear usuario. Posible duplicado de Username/Email.");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario. Verifique que el Username y Email no existan.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al crear nuevo usuario: {UserName}", userDto?.Username ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario", ex);
            }
        }

        // Método para actualizar un usuario existente (PUT)
        public async Task<UserDto> UpdateUserAsync(int id, UserDto userDto)
        {
            if (id <= 0 || id != userDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar un usuario con un ID invalido o no coincidente: {UserId}, DTO ID: {DtoId}", id, userDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            // Validar DTO, pero NO la contraseña ya que no se actualiza aquí
            ValidateUser(userDto, isCreate: false);

            try
            {
                var existingUser = await _userData.GetByIdAsync(id);
                if (existingUser == null)
                {
                    _logger.LogInformation("No se encontró el usuario con ID {UserId} para actualizar", id);
                    throw new EntityNotFoundException("User", id);
                }

                // Mapea los cambios del DTO a la entidad existente (SIN CONTRASEÑA)
                existingUser = MapToEntity(userDto, existingUser);
                // Considerar actualizar UpdateDate si existe en la entidad
                // existingUser.UpdateDate = DateTime.UtcNow;

                await _userData.UpdateAsync(existingUser); // Asume que UpdateAsync existe
                return MapToDTO(existingUser);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al actualizar usuario {UserId}. Posible duplicado Username/Email.", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el usuario con ID {id}. Verifique Username/Email.", dbEx);
            }
        }

        // Método para actualizar parcialmente un usuario (PATCH)
        public async Task<UserDto> PatchUserAsync(int id, UserDto userDto) // Idealmente usar JsonPatchDocument
        {
            if (id <= 0 || (userDto.Id != 0 && id != userDto.Id))
            {
                _logger.LogWarning("Se intentó aplicar patch a un usuario con un ID invalido o no coincidente: {UserId}, DTO ID: {DtoId}", id, userDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado en la URL es inválido o no coincide con el ID del DTO (si se proporcionó) para PATCH.");
            }

            try
            {
                var existingUser = await _userData.GetByIdAsync(id);
                if (existingUser == null)
                {
                    _logger.LogInformation("No se encontró el usuario con ID {UserId} para aplicar patch", id);
                    throw new EntityNotFoundException("User", id);
                }

                bool changed = false;

                // Actualizar Username si se proporciona y es diferente
                if (userDto.Username != null && existingUser.Username != userDto.Username)
                {
                    if (string.IsNullOrWhiteSpace(userDto.Username))
                        throw new Utilities.Exceptions.ValidationException("Username", "El Username no puede estar vacío en PATCH.");
                    existingUser.Username = userDto.Username;
                    changed = true;
                }

                // Actualizar Email si se proporciona y es diferente
                if (userDto.Email != null && existingUser.Email != userDto.Email)
                {
                    if (string.IsNullOrWhiteSpace(userDto.Email) || !IsValidEmail(userDto.Email))
                        throw new Utilities.Exceptions.ValidationException("Email", "El Email proporcionado no es válido en PATCH.");
                    existingUser.Email = userDto.Email;
                    changed = true;
                }

                // Actualizar PersonId si se proporciona y es diferente
                if (userDto.PersonId > 0 && existingUser.PersonId != userDto.PersonId) // Asumir PersonId > 0 es válido
                {
                    existingUser.PersonId = userDto.PersonId;
                    changed = true;
                }

                // Actualizar Active si es diferente
                if (existingUser.Active != userDto.Active)
                {
                    existingUser.Active = userDto.Active;
                    changed = true;
                    // Considerar lógica de DeleteDate si existiera
                }

                // NO ACTUALIZAR CONTRASEÑA AQUÍ
                // Si se incluyera userDto.Password != null, habría que validarlo y hashearlo.

                if (changed)
                {
                    // Considerar actualizar UpdateDate
                    // existingUser.UpdateDate = DateTime.UtcNow;
                    await _userData.UpdateAsync(existingUser);
                    _logger.LogInformation("Aplicado patch al usuario con ID {UserId}", id);
                }
                else
                {
                    _logger.LogInformation("No se detectaron cambios en la solicitud PATCH para el usuario con ID {UserId}", id);
                }

                return MapToDTO(existingUser);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al aplicar patch al usuario {UserId}. Posible duplicado Username/Email.", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch al usuario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch al usuario {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch al usuario con ID {id}", ex);
            }
        }

        // Método para eliminar un usuario (DELETE persistente)
        // ADVERTENCIA: Altamente propenso a fallar debido a FKs. Usar SoftDelete.
        public async Task DeleteUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un usuario con un ID invalido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor a 0");
            }
            try
            {
                var existingUser = await _userData.GetByIdAsync(id); // Verificar si existe
                if (existingUser == null)
                {
                    _logger.LogInformation("No se encontró el usuario con ID {UserId} para eliminar (persistente)", id);
                    throw new EntityNotFoundException("User", id);
                }

                // ADVERTENCIA: Fallará si hay UserRol, UserSede, Aprendiz, Instructor, etc. asociados
                bool deleted = await _userData.DeleteAsync(id); // Asume que DeleteAsync existe
                if (deleted)
                {
                    _logger.LogInformation("Usuario con ID {UserId} eliminado exitosamente (persistente)", id);
                }
                else
                {
                    _logger.LogWarning("No se pudo eliminar (persistente) el usuario con ID {UserId}.", id);
                    throw new EntityNotFoundException("User", id);
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx) // Captura FK violation
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar el usuario {UserId}. Violación de FK.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el usuario con ID {id}. Verifique dependencias.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al eliminar (persistente) el usuario {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el usuario con ID {id}", ex);
            }
        }

        // Método para desactivar un usuario (soft delete)
        public async Task SoftDeleteUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó desactivar un usuario con un ID invalido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor a 0");
            }
            try
            {
                var userToDeactivate = await _userData.GetByIdAsync(id);
                if (userToDeactivate == null)
                {
                    _logger.LogInformation("No se encontró el usuario con ID {UserId} para desactivar", id);
                    throw new EntityNotFoundException("User", id);
                }

                if (!userToDeactivate.Active)
                {
                    _logger.LogInformation("El usuario con ID {UserId} ya está inactivo.", id);
                    return;
                }

                userToDeactivate.Active = false;
                // Considerar actualizar UpdateDate/DeleteDate si existen
                // userToDeactivate.UpdateDate = DateTime.UtcNow;
                await _userData.UpdateAsync(userToDeactivate);

                _logger.LogInformation("Usuario con ID {UserId} marcado como inactivo.", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al desactivar usuario {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar el usuario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al desactivar usuario {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar el usuario con ID {id}", ex);
            }
        }

        // Método para activar un usuario
        public async Task ActivateUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó activar un usuario con un ID invalido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor a 0");
            }
            try
            {
                var userToActivate = await _userData.GetByIdAsync(id);
                if (userToActivate == null)
                {
                    _logger.LogInformation("No se encontró el usuario con ID {UserId} para activar", id);
                    throw new EntityNotFoundException("User", id);
                }

                if (userToActivate.Active)
                {
                    _logger.LogInformation("El usuario con ID {UserId} ya está activo.", id);
                    return;
                }

                userToActivate.Active = true;
                // Considerar limpiar DeleteDate y actualizar UpdateDate si existen
                // userToActivate.DeleteDate = null;
                // userToActivate.UpdateDate = DateTime.UtcNow;
                await _userData.UpdateAsync(userToActivate);

                _logger.LogInformation("Usuario con ID {UserId} marcado como activo.", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al activar usuario {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al activar el usuario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al activar usuario {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al activar el usuario con ID {id}", ex);
            }
        }

        // Método para validar el DTO (modificado)
        private void ValidateUser(UserDto userDto, bool isCreate = false)
        {
            if (userDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto usuario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(userDto.Username))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Username vacío");
                throw new Utilities.Exceptions.ValidationException("Username", "El Username es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(userDto.Email) || !IsValidEmail(userDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Email vacío o inválido");
                throw new Utilities.Exceptions.ValidationException("Email", "El Email es obligatorio y debe tener un formato válido");
            }

            if (isCreate)
            {
                if (string.IsNullOrWhiteSpace(userDto.Password))
                {
                    _logger.LogWarning("Se intentó crear un usuario sin contraseña");
                    throw new Utilities.Exceptions.ValidationException("Password", "La contraseña es obligatoria al crear un usuario");
                }
                // Añadir reglas de complejidad de contraseña si es necesario
                // if (userDto.Password.Length < 8) throw new Utilities.Exceptions.ValidationException("Password", "La contraseña debe tener al menos 8 caracteres.");
            }
            // En PATCH/PUT, la contraseña en el DTO podría ser null/vacía, lo cual está bien porque no la actualizamos.

            if (userDto.PersonId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con PersonId inválido: {PersonId}", userDto.PersonId);
                throw new Utilities.Exceptions.ValidationException("PersonId", "El PersonId asociado al usuario es obligatorio.");
            }
        }

        // Helper para validar email (simple)
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                // Usar regex más robusto si es necesario para cumplir RFC 5322
                // string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                // return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        //Funciones de mapeos 
        // Método para mapear de User a UserDto (SIN CONTRASEÑA - MODIFICADO)
        private UserDto MapToDTO(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Active = user.Active,
                PersonId = user.PersonId
                // NUNCA devolver la contraseña hasheada
                // Password = null // Explícitamente null o simplemente omitir
            };
        }

        // Método para mapear de UserDto a User (para creación)
        private User MapToEntity(UserDto userDto)
        {
            return new User
            {
                // Id = userDto.Id, // No en creación
                Username = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password, // La contraseña se mapea aquí, se hashea en CreateUserAsync
                PersonId = userDto.PersonId,
                Active = userDto.Active
            };
        }

        // Método para mapear de DTO a una entidad existente (para actualización PUT/PATCH - SIN CONTRASEÑA - NUEVO)
        private User MapToEntity(UserDto userDto, User existingUser)
        {
            existingUser.Username = userDto.Username;
            existingUser.Email = userDto.Email;
            existingUser.PersonId = userDto.PersonId;
            existingUser.Active = userDto.Active;
            // NO mapear la contraseña aquí para evitar sobrescribirla accidentalmente.
            // El cambio de contraseña debe ser un proceso separado.
            return existingUser;
        }

        // Método para mapear una lista de User a una lista de UserDto
        private IEnumerable<UserDto> MapToDTOList(IEnumerable<User> users)
        {
            return users.Select(MapToDTO).ToList(); // Usar LINQ y el MapToDTO modificado
        }
    }
}
