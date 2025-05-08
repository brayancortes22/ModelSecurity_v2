using Business;
using Business.Interfaces;
using Entity.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de usuarios en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IGenericBusiness<UserDto, int> _userBusiness;
        private readonly UserBusiness _userBusinessSpecific; // Para métodos específicos
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Constructor del controlador de usuarios
        /// </summary>
        public UserController(
            IGenericBusiness<UserDto, int> userBusiness, 
            UserBusiness userBusinessSpecific, 
            ILogger<UserController> logger)
        {
            _userBusiness = userBusiness ?? throw new ArgumentNullException(nameof(userBusiness));
            _userBusinessSpecific = userBusinessSpecific ?? throw new ArgumentNullException(nameof(userBusinessSpecific));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userBusiness.GetAllAsync();
                return Ok(users);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userBusiness.GetByIdAsync(id);
                return Ok(user);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el usuario con ID: {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID: {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            try
            {
                var createdUser = await _userBusiness.CreateAsync(userDto);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear usuario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un usuario existente (reemplazo completo, SIN contraseña).
        /// </summary>
        /// <param name="id">ID del usuario a actualizar.</param>
        /// <param name="userDto">Datos actualizados del usuario (la contraseña será ignorada).</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            try
            {
                var updatedUser = await _userBusiness.UpdateAsync(id, userDto);
                return Ok(updatedUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar usuario {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para actualizar con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un usuario existente.
        /// </summary>
        /// <param name="id">ID del usuario a actualizar.</param>
        /// <param name="userDto">Datos parciales a actualizar (incluida la contraseña si se requiere cambiar).</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchUser(int id, [FromBody] UserDto userDto)
        {
            try
            {
                var patchedUser = await _userBusiness.PatchAsync(id, userDto);
                return Ok(patchedUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a usuario {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para aplicar patch con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch a usuario {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina permanentemente un usuario por su ID.
        /// </summary>
        /// <param name="id">ID del usuario a eliminar.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userBusiness.DeleteAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al intentar eliminar usuario {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para eliminar con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) un usuario por su ID.
        /// </summary>
        /// <param name="id">ID del usuario a desactivar.</param>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            try
            {
                await _userBusiness.SoftDeleteAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar soft-delete de usuario {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para soft-delete con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar soft-delete de usuario {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Reactiva un usuario previamente desactivado.
        /// </summary>
        /// <param name="id">ID del usuario a reactivar.</param>
        [HttpPost("{id}/activate")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                await _userBusiness.ActivateAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al intentar activar usuario {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para activar con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al activar usuario {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza la contraseña de un usuario.
        /// </summary>
        /// <param name="id">ID del usuario cuya contraseña se actualizará.</param>
        /// <param name="passwordData">Objeto con la nueva contraseña.</param>
        [HttpPatch("{id}/password")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] UpdatePasswordDto passwordData)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID de usuario inválido" });
            }

            if (string.IsNullOrWhiteSpace(passwordData?.NewPassword))
            {
                return BadRequest(new { message = "La nueva contraseña no puede estar vacía" });
            }

            try
            {
                await _userBusinessSpecific.UpdatePasswordAsync(id, passwordData.NewPassword);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar contraseña de usuario {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para actualizar contraseña con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar contraseña de usuario {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los roles asignados a un usuario específico
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Lista de roles asignados al usuario</returns>
        [HttpGet("{id}/roles")]
        [ProducesResponseType(typeof(IEnumerable<UserRolDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserRolDto>>> GetUserRoles(int id)
        {
            try
            {
                // Primero verificamos si el usuario existe
                var user = await _userBusiness.GetByIdAsync(id);
                
                // Obtenemos los roles del usuario desde el servicio de UserRol
                var userRolBusiness = HttpContext.RequestServices.GetRequiredService<UserRolBusiness>();
                var roles = await userRolBusiness.GetRolesByUserIdAsync(id);
                
                _logger.LogInformation("Se obtuvieron {Count} roles del usuario con ID {UserId}", roles.Count(), id);
                return Ok(roles);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al obtener roles del usuario con ID: {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener roles del usuario con ID: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Autentica a un usuario con sus credenciales
        /// </summary>
        /// <param name="credentials">Credenciales de usuario (nombre de usuario y contraseña)</param>
        /// <returns>Información del usuario autenticado</returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Authenticate([FromBody] LoginDto credentials)
        {
            try
            {
                if (credentials == null || string.IsNullOrEmpty(credentials.Username) || string.IsNullOrEmpty(credentials.Password))
                {
                    return BadRequest(new { message = "Nombre de usuario y contraseña son requeridos" });
                }

                var user = await _userBusinessSpecific.AuthenticateAsync(credentials.Username, credentials.Password);
                
                if (user == null)
                {
                    return Unauthorized(new { message = "Nombre de usuario o contraseña incorrectos" });
                }

                return Ok(user);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error durante la autenticación del usuario {Username}", credentials?.Username);
                return StatusCode(500, new { message = "Error durante la autenticación del usuario" });
            }
        }
    }
    
    /// <summary>
    /// DTO para actualizar la contraseña de un usuario
    /// </summary>
    public class UpdatePasswordDto
    {
        /// <summary>
        /// Nueva contraseña
        /// </summary>
        public string NewPassword { get; set; }
    }
    
    /// <summary>
    /// DTO para la autenticación de usuarios
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Contraseña
        /// </summary>
        public string Password { get; set; }
    }
}
