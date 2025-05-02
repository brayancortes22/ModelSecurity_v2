using Business;
using Data;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de permisos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolController : ControllerBase
    {
        private readonly RolBusiness _RolBusiness;
        private readonly ILogger<RolController> _logger;

        /// <summary>
        /// Constructor del controlador de permisos
        /// </summary>
        /// <param name="RolBusiness">Capa de negocio de permisos</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public RolController(RolBusiness RolBusiness, ILogger<RolController> logger)
        {
            _RolBusiness = RolBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos del sistema
        /// </summary>
        /// <returns>Lista de permisos</returns>
        /// <response code="200">Retorna la lista de permisos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRols()
        {
            try
            {
                var Rols = await _RolBusiness.GetAllRolesAsync();
                return Ok(Rols);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un permiso específico por su ID
        /// </summary>
        /// <param name="id">ID del permiso</param>
        /// <returns>Permiso solicitado</returns>
        /// <response code="200">Retorna el permiso solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Permiso no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolData), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolById(int id)
        {
            try
            {
                var Rol = await _RolBusiness.GetRolByIdAsync(id);
                return Ok(Rol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="RolDto">Datos del permiso a crear</param>
        /// <returns>Permiso creado</returns>
        /// <response code="201">Retorna el permiso creado</response>
        /// <response code="400">Datos del permiso no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RolDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRol([FromBody] RolDto RolDto)
        {
            try
            {
                var createdRol = await _RolBusiness.CreateRolAsync(RolDto);
                return CreatedAtAction(nameof(GetRolById), new { id = createdRol.Id }, createdRol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear permiso");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear permiso");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un permiso existente en el sistema.
        /// </summary>
        /// <param name="id">ID del permiso a actualizar.</param>
        /// <param name="rolDto">Datos actualizados del permiso.</param>
        /// <returns>No Content si la actualización fue exitosa.</returns>
        /// <response code="200">Retorna el permiso actualizado.</response>
        /// <response code="400">ID inválido o datos del permiso no válidos.</response>
        /// <response code="404">Permiso no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRol(int id, [FromBody] RolDto rolDto)
        {
            if (id != rolDto.Id)
            {
                 _logger.LogWarning("El ID de la ruta ({RouteId}) no coincide con el ID del cuerpo ({BodyId}) para la actualización.", id, rolDto.Id);
                 return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo." });
            }

            try
            {
                var updatedRol = await _RolBusiness.UpdateRolAsync(id, rolDto);
                return Ok(updatedRol); // Devolvemos el rol actualizado
            }
             catch (ValidationException ex)
             {
                 _logger.LogWarning(ex, "Validación fallida al actualizar permiso con ID: {RolId}", id);
                 return BadRequest(new { message = ex.Message });
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Permiso no encontrado para actualizar con ID: {RolId}", id);
                 return NotFound(new { message = ex.Message });
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error al actualizar permiso con ID: {RolId}", id);
                 return StatusCode(500, new { message = ex.Message });
             }
        }

        /// <summary>
        /// Actualiza parcialmente un permiso existente en el sistema.
        /// </summary>
        /// <remarks>
        /// Nota: La implementación actual de negocio actualiza todos los campos proporcionados.
        /// </remarks>
        /// <param name="id">ID del permiso a actualizar parcialmente.</param>
        /// <param name="rolDto">Datos a actualizar del permiso.</param>
        /// <returns>El permiso actualizado.</returns>
        /// <response code="200">Retorna el permiso parcialmente actualizado.</response>
        /// <response code="400">ID inválido o datos del permiso no válidos.</response>
        /// <response code="404">Permiso no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(RolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
         public async Task<IActionResult> PatchRol(int id, [FromBody] RolDto rolDto)
         {
             // Considerar si es necesario validar que el ID en el DTO coincida (si se envía)
             // if (rolDto.Id != 0 && id != rolDto.Id) { ... }
             // Por ahora, se asume que el DTO podría no tener el ID o que el ID de ruta prevalece.
             // Sin embargo, la capa de negocio *sí* espera que rolDto.Id coincida para la implementación actual de PatchRolAsync.
             // Forzamos la coincidencia para cumplir con la capa de negocio actual.
             if (id != rolDto.Id)
             {
                 _logger.LogWarning("El ID de la ruta ({RouteId}) no coincide con el ID del cuerpo ({BodyId}) para el patch.", id, rolDto.Id);
                  return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo para PATCH." });
             }

            try
            {
                // Asumiendo que rolDto podría contener solo campos parciales
                // La lógica en RolBusiness.PatchRolAsync maneja la actualización parcial (o completa en la implementación actual)
                var patchedRol = await _RolBusiness.PatchRolAsync(id, rolDto);
                return Ok(patchedRol);
            }
             catch (ValidationException ex)
             {
                 _logger.LogWarning(ex, "Validación fallida al aplicar patch al permiso con ID: {RolId}", id);
                 return BadRequest(new { message = ex.Message });
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Permiso no encontrado para aplicar patch con ID: {RolId}", id);
                 return NotFound(new { message = ex.Message });
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error al aplicar patch al permiso con ID: {RolId}", id);
                 return StatusCode(500, new { message = ex.Message });
             }
         }

        /// <summary>
        /// Elimina un permiso del sistema (eliminación persistente).
        /// </summary>
        /// <param name="id">ID del permiso a eliminar.</param>
        /// <returns>No Content si la eliminación fue exitosa.</returns>
        /// <response code="204">Permiso eliminado exitosamente.</response>
        /// <response code="400">ID proporcionado no válido.</response>
        /// <response code="404">Permiso no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRol(int id)
        {
             try
             {
                 await _RolBusiness.DeleteRolAsync(id);
                 return NoContent(); // 204 No Content es apropiado para DELETE exitoso
             }
             catch (ValidationException ex) // Aunque DeleteRolAsync valida ID > 0, mantenemos por consistencia
             {
                 _logger.LogWarning(ex, "Validación fallida al intentar eliminar permiso con ID: {RolId}", id);
                 return BadRequest(new { message = ex.Message });
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Permiso no encontrado para eliminar con ID: {RolId}", id);
                 return NotFound(new { message = ex.Message }); // 404 si no existe
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error al eliminar permiso con ID: {RolId}", id);
                 return StatusCode(500, new { message = ex.Message });
             }
        }

         /// <summary>
        /// Desactiva un permiso en el sistema (eliminación lógica).
        /// </summary>
        /// <param name="id">ID del permiso a desactivar.</param>
        /// <returns>No Content si la desactivación fue exitosa.</returns>
        /// <response code="204">Permiso desactivado exitosamente.</response>
        /// <response code="400">ID proporcionado no válido.</response>
        /// <response code="404">Permiso no encontrado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPatch("soft-delete/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteRol(int id)
        {
             try
             {
                 await _RolBusiness.SoftDeleteRolAsync(id);
                 return NoContent(); // 204 No Content indica éxito sin devolver cuerpo
             }
             catch (ValidationException ex) // SoftDeleteRolAsync valida ID > 0
             {
                 _logger.LogWarning(ex, "Validación fallida al intentar desactivar permiso con ID: {RolId}", id);
                 return BadRequest(new { message = ex.Message });
             }
             catch (EntityNotFoundException ex)
             {
                 _logger.LogInformation(ex, "Permiso no encontrado para desactivar con ID: {RolId}", id);
                 return NotFound(new { message = ex.Message });
             }
             catch (ExternalServiceException ex)
             {
                 _logger.LogError(ex, "Error al desactivar permiso con ID: {RolId}", id);
                 return StatusCode(500, new { message = ex.Message });
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
        public async Task<IActionResult> ActivateRol(int id)
        {
            try
            {
                await _RolBusiness.ActivateRolAsync(id);
                return NoContent(); // Opcionalmente podrías devolver el usuario activado (Ok(RolDto))
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar activar usuario {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para activar con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al activar usuario {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al activar usuario {RolId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Obtiene todos los formularios asignados a un rol específico
        /// </summary>
        /// <param name="id">ID del rol</param>
        /// <returns>Lista de formularios asignados al rol</returns>
        /// <response code="200">Retorna la lista de formularios</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Rol no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}/forms")]
        [ProducesResponseType(typeof(IEnumerable<FormDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFormsByRolId(int id)
        {
            try
            {
                var forms = await _RolBusiness.GetFormsByRolIdAsync(id);
                return Ok(forms);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para obtener formularios del rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado para obtener formularios con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener formularios del rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}