using Business;
using Entity.DTOs; // Ruta correcta para UserRolDto
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de la asignación de Roles a Usuarios.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserRolController : ControllerBase
    {
        private readonly UserRolBusiness _userRolBusiness;
        private readonly ILogger<UserRolController> _logger;

        /// <summary>
        /// Constructor del controlador UserRol.
        /// </summary>
        public UserRolController(UserRolBusiness userRolBusiness, ILogger<UserRolController> logger)
        {
            _userRolBusiness = userRolBusiness ?? throw new ArgumentNullException(nameof(userRolBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todas las asignaciones de roles a usuarios.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserRolDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserRolDto>>> GetAllUserRoles()
        {
            try
            {
                var userRoles = await _userRolBusiness.GetAllRolUsersAsync();
                _logger.LogInformation("Se obtuvieron {Count} asignaciones de rol a usuario.", userRoles.Count());
                return Ok(userRoles);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al obtener todas las asignaciones rol-usuario.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener todas las asignaciones rol-usuario.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Obtiene una asignación rol-usuario específica por su ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserRolDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserRolDto>> GetUserRolById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de obtener una asignación rol-usuario con ID inválido: {UserRolId}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }
            try
            {
                var userRol = await _userRolBusiness.GetRolUserByIdAsync(id);
                _logger.LogInformation("Asignación rol-usuario con ID {UserRolId} obtenida exitosamente.", id);
                return Ok(userRol);
            }
            catch (ValidationException ex) // Aunque GetRolUserByIdAsync no lanza directamente ValidationException por ID<=0, lo mantenemos por si acaso.
            {
                _logger.LogWarning(ex, "Validación fallida para la asignación rol-usuario con ID: {UserRolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Asignación rol-usuario no encontrada con ID: {UserRolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al obtener asignación rol-usuario con ID: {UserRolId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener asignación rol-usuario con ID: {UserRolId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Crea una nueva asignación rol-usuario.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserRolDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserRolDto>> CreateUserRol([FromBody] UserRolDto userRolDto)
        {
            if (userRolDto == null)
            {
                _logger.LogWarning("Intento de crear una asignación rol-usuario con datos nulos.");
                return BadRequest(new { message = "La asignación rol-usuario no puede ser nula." });
            }
            // El ID no debería venir en la creación, o debería ser 0.
            if (userRolDto.Id != 0)
            {
                 _logger.LogWarning("Se intentó crear una asignación rol-usuario proporcionando un ID explícito: {UserRolId}", userRolDto.Id);
                 return BadRequest(new { message = "No se debe proporcionar un ID para crear una nueva asignación." });
            }

            try
            {
                var createdUserRol = await _userRolBusiness.CreateRolUserAsync(userRolDto);
                _logger.LogInformation("Asignación rol-usuario creada exitosamente con ID {UserRolId}.", createdUserRol.Id);
                // Devolver la ruta al recurso creado
                return CreatedAtAction(nameof(GetUserRolById), new { id = createdUserRol.Id }, createdUserRol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear asignación rol-usuario.");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al crear asignación rol-usuario.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear asignación rol-usuario.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Actualiza una asignación rol-usuario existente.
        /// </summary>
        /// <param name="id">ID de la asignación a actualizar.</param>
        /// <param name="userRolDto">Datos actualizados de la asignación.</param>
        /// <returns>Respuesta indicando éxito o fracaso.</returns>
        /// <response code="200">Asignación actualizada exitosamente.</response>
        /// <response code="400">ID inválido o datos de la asignación no válidos.</response>
        /// <response code="404">Asignación no encontrada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Cambiado de 204 a 200 para devolver el objeto actualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserRol(int id, [FromBody] UserRolDto userRolDto)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Intento de actualizar una asignación rol-usuario con ID de ruta inválido: {UserRolId}", id);
                 return BadRequest(new { message = "El ID proporcionado en la ruta es inválido." });
            }
             if (userRolDto == null || id != userRolDto.Id)
            {
                 _logger.LogWarning("Datos inválidos para actualizar la asignación rol-usuario con ID: {UserRolId}. DTO: {@UserRolDto}", id, userRolDto);
                 return BadRequest(new { message = "El ID de la ruta no coincide con el ID de la asignación proporcionada o el cuerpo es nulo." });
            }

            try
            {
                // UpdateRolUserAsync ahora devuelve el DTO actualizado
                var updatedUserRol = await _userRolBusiness.UpdateRolUserAsync(id, userRolDto);
                _logger.LogInformation("Asignación rol-usuario con ID {UserRolId} actualizada exitosamente.", id);
                return Ok(updatedUserRol); // Devolver el objeto actualizado
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar asignación rol-usuario con ID: {UserRolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Asignación rol-usuario no encontrada para actualizar con ID: {UserRolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al actualizar asignación rol-usuario con ID: {UserRolId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar asignación rol-usuario con ID: {UserRolId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Elimina una asignación rol-usuario del sistema (eliminación persistente).
        /// </summary>
        /// <param name="id">ID de la asignación a eliminar.</param>
        /// <returns>Respuesta indicando éxito.</returns>
        /// <response code="204">Asignación eliminada exitosamente.</response>
        /// <response code="400">ID proporcionado no válido.</response>
        /// <response code="404">Asignación no encontrada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserRol(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Intento de eliminar una asignación rol-usuario con ID inválido: {UserRolId}", id);
                 return BadRequest(new { message = "El ID proporcionado es inválido." });
            }

            try
            {
                await _userRolBusiness.DeleteRolUserAsync(id);
                _logger.LogInformation("Asignación rol-usuario con ID {UserRolId} eliminada exitosamente.", id);
                return NoContent(); // 204 No Content es apropiado para DELETE exitoso
            }
            catch (ValidationException ex) // Aunque DeleteRolUserAsync no debería lanzar ValidationException por ID<=0 aquí.
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar eliminar asignación rol-usuario con ID: {UserRolId}", id);
                 return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Asignación rol-usuario no encontrada para eliminar con ID: {UserRolId}", id);
                return NotFound(new { message = ex.Message }); // 404 si no existe
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error externo al eliminar asignación rol-usuario con ID: {UserRolId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar asignación rol-usuario con ID: {UserRolId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        /// <summary>
        /// Desactiva una asignación rol-usuario en el sistema (eliminación lógica).
        /// </summary>
        /// <param name="id">ID de la asignación a desactivar.</param>
        /// <returns>Respuesta indicando éxito.</returns>
        /// <response code="204">Asignación desactivada exitosamente.</response>
        /// <response code="400">ID proporcionado no válido.</response>
        /// <response code="404">Asignación no encontrada.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SoftDeleteUserRol(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de realizar borrado lógico a una asignación rol-usuario con ID inválido: {UserRolId}", id);
                return BadRequest(new { message = "El ID proporcionado es inválido." });
            }

            try
            {
                // Llamar al método de negocio corregido
                await _userRolBusiness.SoftDeleteUserRolAsync(id);
                _logger.LogInformation("Borrado lógico realizado exitosamente para la asignación rol-usuario con ID {UserRolId}.", id);
                return NoContent(); // 204 No Content indica éxito
            }
            catch (ValidationException ex) // Capturar si SoftDelete valida y falla (ej: ID <= 0)
            {
                _logger.LogWarning(ex, "Validación fallida al intentar desactivar asignación rol-usuario con ID: {UserRolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Asignación rol-usuario no encontrada para desactivar con ID: {UserRolId}", id);
                return NotFound(new { message = ex.Message }); // Devolver 404 si no se encuentra
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error externo al desactivar asignación rol-usuario con ID: {UserRolId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al desactivar asignación rol-usuario con ID: {UserRolId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado. Por favor, intente nuevamente." });
            }
        }

        // Nota: No se incluye endpoint para SoftDelete debido a la inconsistencia en UserRolBusiness.SoftDeleteStateAsync.
    }
} 