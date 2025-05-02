using Business;
using Data;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private readonly UserBusiness _UserBusiness;
        private readonly ILogger<UserController> _logger;
        private readonly UserBusiness _userBusiness;

        /// <summary>
        /// Constructor del controlador de usuarios
        public UserController(UserBusiness UserBusiness, ILogger<UserController> logger)
        {
            _UserBusiness = UserBusiness;
            _userBusiness = UserBusiness;
            _UserBusiness = UserBusiness;
            _logger = logger;
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
                var users = await _UserBusiness.GetAllUsersAsync();
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
                var user = await _UserBusiness.GetUserByIdAsync(id);
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
        public async Task<IActionResult> CreateUser([FromBody] UserDto UserDto)
        {
            try
            {
                var createdUser = await _UserBusiness.CreateUserAsync(UserDto);
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
        /// <response code="200">Retorna el usuario actualizado (sin contraseña).</response>
        /// <response code="400">Si el ID o los datos son inválidos.</response>
        /// <response code="404">Si no se encuentra el usuario.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            // Nota: La contraseña en userDto será ignorada por la capa de negocio.
            // Para cambiar contraseña, usar un endpoint dedicado.
            try
            {
                var updatedUser = await _UserBusiness.UpdateUserAsync(id, userDto);
                return Ok(updatedUser); // DTO no incluye contraseña
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
                _logger.LogError(ex, "Error de servicio externo al actualizar usuario {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al actualizar usuario {UserId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un usuario existente.
        /// </summary>
        /// <param name="id">ID del usuario a actualizar.</param>
        /// <param name="userDto">Datos parciales a actualizar (incluida la contraseña si se requiere cambiar).</param>
        /// <remarks>
        /// Este endpoint puede usarse para actualizar la contraseña enviando solo el campo password en el body:
        /// {
        ///   "password": "nueva-contraseña"
        /// }
        /// </remarks>
        /// <response code="200">Retorna el usuario con los cambios aplicados.</response>
        /// <response code="400">Si el ID o los datos son inválidos.</response>
        /// <response code="404">Si no se encuentra el usuario.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> PatchUser(int id, [FromBody] UserDto userDto)
        {
            // Nota: Ahora manejamos correctamente la contraseña en PATCH
            try
            {
                var patchedUser = await _UserBusiness.PatchUserAsync(id, userDto);
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
                _logger.LogError(ex, "Error de servicio externo al aplicar patch a usuario {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al aplicar patch a usuario {UserId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Elimina permanentemente un usuario por su ID.
        /// </summary>
        /// <param name="id">ID del usuario a eliminar.</param>
        /// <remarks>ADVERTENCIA: Operación destructiva. Puede fallar por dependencias. Se recomienda usar el endpoint de desactivación.</remarks>
        /// <response code="204">Si la eliminación fue exitosa.</response>
        /// <response code="400">Si el ID es inválido.</response>
        /// <response code="404">Si no se encuentra el usuario.</response>
        /// <response code="500">Si ocurre un error interno (p.ej., violación de FK).</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _UserBusiness.DeleteUserAsync(id);
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
            catch (ExternalServiceException ex) // Captura errores de BD (FK violation)
            {
                _logger.LogError(ex, "Error de servicio externo al eliminar usuario {UserId}", id);
                 // Considerar 409 Conflict para FK violation
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al eliminar usuario {UserId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) un usuario por su ID.
        /// </summary>
        /// <param name="id">ID del usuario a desactivar.</param>
        /// <response code="204">Si la desactivación fue exitosa o ya estaba inactivo.</response>
        /// <response code="400">Si el ID es inválido.</response>
        /// <response code="404">Si no se encuentra el usuario.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            try
            {
                await _UserBusiness.SoftDeleteUserAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar desactivar usuario {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para desactivar con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al desactivar usuario {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al desactivar usuario {UserId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Reactiva un usuario previamente desactivado.
        /// </summary>
        /// <param name="id">ID del usuario a reactivar.</param>
        /// <response code="204">Si la activación fue exitosa o ya estaba activo.</response>
        /// <response code="400">Si el ID es inválido.</response>
        /// <response code="404">Si no se encuentra el usuario.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpPost("{id}/activate")] // Usar POST para acciones que cambian estado
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                await _UserBusiness.ActivateUserAsync(id);
                return NoContent(); // Opcionalmente podrías devolver el usuario activado (Ok(userDto))
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
                _logger.LogError(ex, "Error de servicio externo al activar usuario {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al activar usuario {UserId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Actualiza la contraseña de un usuario.
        /// </summary>
        /// <param name="id">ID del usuario cuya contraseña se actualizará.</param>
        /// <param name="passwordData">Objeto con la nueva contraseña.</param>
        /// <response code="204">Si la contraseña se actualizó correctamente.</response>
        /// <response code="400">Si el ID es inválido o la contraseña está vacía.</response>
        /// <response code="404">Si no se encuentra el usuario.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpPatch("{id}/password")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
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
                await _UserBusiness.UpdatePasswordAsync(id, passwordData.NewPassword);
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
                _logger.LogError(ex, "Error de servicio externo al actualizar contraseña de usuario {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar contraseña de usuario {UserId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
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
                var user = await _userBusiness.GetUserByIdAsync(id);
                
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
                _logger.LogError(ex, "Error de servicio externo al obtener roles del usuario con ID: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener roles del usuario con ID: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Ocurrió un error inesperado al obtener los roles del usuario." });
            }
        }
    }
}
