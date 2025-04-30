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
    public class PersonController : ControllerBase
    {
        private readonly PersonBusiness _PersonBusiness;
        private readonly ILogger<PersonController> _logger;

        /// <summary>
        /// Constructor del controlador de usuarios
        /// </summary>
        public PersonController(PersonBusiness PersonBusiness, ILogger<PersonController> logger)
        {
            _PersonBusiness = PersonBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PersonDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPerson()
        {
            try
            {
                var Person = await _PersonBusiness.GetAllPersonsAsync();
                return Ok(Person);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener personas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PersonDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPersonById(int id)
        {
            try
            {
                var person = await _PersonBusiness.GetPersonByIdAsync(id);
                return Ok(person);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el usuario con ID: {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID: {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PersonDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePerson([FromBody] PersonDto PersonDto)
        {
            try
            {
                var createdPerson = await _PersonBusiness.CreatePersonAsync(PersonDto);
                return CreatedAtAction(nameof(GetPersonById), new { id = createdPerson.Id }, createdPerson);
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
        /// <param name="PersonDto">Datos actualizados del usuario (la contraseña será ignorada).</param>
        /// <response code="200">Retorna el usuario actualizado (sin contraseña).</response>
        /// <response code="400">Si el ID o los datos son inválidos.</response>
        /// <response code="404">Si no se encuentra el usuario.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PersonDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] PersonDto PersonDto)
        {
            // Nota: La contraseña en PersonDto será ignorada por la capa de negocio.
            // Para cambiar contraseña, usar un endpoint dedicado.
            try
            {
                var updatedPerson = await _PersonBusiness.UpdatePersonAsync(id, PersonDto);
                return Ok(updatedPerson); // DTO no incluye contraseña
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar usuario {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para actualizar con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al actualizar usuario {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al actualizar usuario {PersonId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un usuario existente (SIN contraseña).
        /// </summary>
        /// <param name="id">ID del usuario a actualizar.</param>
        /// <param name="PersonDto">Datos parciales a actualizar (la contraseña será ignorada).</param>
        /// <remarks>NOTA: Se recomienda usar JsonPatch.</remarks>
        /// <response code="200">Retorna el usuario con los cambios aplicados (sin contraseña).</response>
        /// <response code="400">Si el ID o los datos son inválidos.</response>
        /// <response code="404">Si no se encuentra el usuario.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(PersonDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> PatchPerson(int id, [FromBody] PersonDto PersonDto)
        {
            // Nota: La contraseña en PersonDto será ignorada.
            try
            {
                var patchedPerson = await _PersonBusiness.PatchPersonAsync(id, PersonDto);
                return Ok(patchedPerson); // DTO no incluye contraseña
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a usuario {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para aplicar patch con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al aplicar patch a usuario {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al aplicar patch a usuario {PersonId}", id);
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
        public async Task<IActionResult> DeletePerson(int id)
        {
            try
            {
                await _PersonBusiness.DeletePersonAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar eliminar usuario {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para eliminar con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex) // Captura errores de BD (FK violation)
            {
                _logger.LogError(ex, "Error de servicio externo al eliminar usuario {PersonId}", id);
                 // Considerar 409 Conflict para FK violation
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al eliminar usuario {PersonId}", id);
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
        public async Task<IActionResult> SoftDeletePerson(int id)
        {
            try
            {
                await _PersonBusiness.SoftDeletePersonAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar desactivar usuario {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para desactivar con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al desactivar usuario {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al desactivar usuario {PersonId}", id);
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
        public async Task<IActionResult> ActivatePerson(int id)
        {
            try
            {
                await _PersonBusiness.ActivatePersonAsync(id);
                return NoContent(); // Opcionalmente podrías devolver el usuario activado (Ok(PersonDto))
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar activar usuario {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Usuario no encontrado para activar con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al activar usuario {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al activar usuario {PersonId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }
    }
}
