using Business;
using Business.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Web.Controllers.Base;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de personas en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PersonController : ActivacionControllerBase<Person>
    {
        private readonly PersonBusiness _personBusiness;
        private readonly ILogger<PersonController> _logger;

        /// <summary>
        /// Constructor del controlador de personas
        /// </summary>
        /// <param name="personBusiness">Capa de negocio de personas</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public PersonController(PersonBusiness personBusiness, ILogger<PersonController> logger)
            : base(personBusiness)
        {
            _personBusiness = personBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las personas del sistema
        /// </summary>
        /// <returns>Lista de personas</returns>
        /// <response code="200">Retorna la lista de personas</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PersonDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPersons()
        {
            try
            {
                var persons = await _personBusiness.GetAllPersonsAsync();
                return Ok(persons);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener personas");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una persona específica por su ID
        /// </summary>
        /// <param name="id">ID de la persona</param>
        /// <returns>Persona solicitada</returns>
        /// <response code="200">Retorna la persona solicitada</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Persona no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PersonDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPersonById(int id)
        {
            try
            {
                var person = await _personBusiness.GetPersonByIdAsync(id);
                return Ok(person);
            }
            catch (Utilities.Exceptions.ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la persona con ID: {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener persona con ID: {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva persona en el sistema
        /// </summary>
        /// <param name="personDto">Datos de la persona a crear</param>
        /// <returns>Persona creada</returns>
        /// <response code="201">Retorna la persona creada</response>
        /// <response code="400">Datos de la persona no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(PersonDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePerson([FromBody] PersonDto personDto)
        {
            try
            {
                var createdPerson = await _personBusiness.CreatePersonAsync(personDto);
                return CreatedAtAction(nameof(GetPersonById), new { id = createdPerson.Id }, createdPerson);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear persona");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear persona");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una persona existente (reemplazo completo).
        /// </summary>
        /// <param name="id">ID de la persona a actualizar.</param>
        /// <param name="personDto">Datos actualizados de la persona.</param>
        /// <response code="200">Retorna la persona actualizada.</response>
        /// <response code="400">Si el ID o los datos son inválidos.</response>
        /// <response code="404">Si no se encuentra la persona.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PersonDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] PersonDto personDto)
        {
             // Opcional: if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updatedPerson = await _personBusiness.UpdatePersonAsync(id, personDto);
                return Ok(updatedPerson);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar persona {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada para actualizar con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al actualizar persona {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al actualizar persona {PersonId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Actualiza parcialmente una persona existente.
        /// </summary>
        /// <param name="id">ID de la persona a actualizar.</param>
        /// <param name="personDto">Datos parciales a actualizar.</param>
        /// <remarks>NOTA: Se recomienda usar JsonPatch.</remarks>
        /// <response code="200">Retorna la persona con los cambios aplicados.</response>
        /// <response code="400">Si el ID o los datos son inválidos.</response>
        /// <response code="404">Si no se encuentra la persona.</response>
        /// <response code="500">Si ocurre un error interno.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(PersonDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> PatchPerson(int id, [FromBody] PersonDto personDto)
        {
            try
            {
                var patchedPerson = await _personBusiness.PatchPersonAsync(id, personDto);
                return Ok(patchedPerson);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a persona {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada para aplicar patch con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al aplicar patch a persona {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al aplicar patch a persona {PersonId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Elimina permanentemente una persona por su ID.
        /// </summary>
        /// <param name="id">ID de la persona a eliminar.</param>
        /// <remarks>ADVERTENCIA: Operación destructiva. Fallará si tiene un Usuario asociado.</remarks>
        /// <response code="204">Si la eliminación fue exitosa.</response>
        /// <response code="400">Si el ID es inválido.</response>
        /// <response code="404">Si no se encuentra la persona.</response>
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
                await _personBusiness.DeletePersonAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar eliminar persona {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada para eliminar con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex) // Captura errores de BD (FK violation)
            {
                _logger.LogError(ex, "Error de servicio externo al eliminar persona {PersonId}", id);
                return StatusCode(500, new { message = ex.Message }); // Considerar 409 Conflict
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al eliminar persona {PersonId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) una persona por su ID.
        /// </summary>
        /// <param name="id">ID de la persona a desactivar.</param>
        /// <response code="204">Si la desactivación fue exitosa o ya estaba inactiva.</response>
        /// <response code="400">Si el ID es inválido.</response>
        /// <response code="404">Si no se encuentra la persona.</response>
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
                await _personBusiness.SoftDeletePersonAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar desactivar persona {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Persona no encontrada para desactivar con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al desactivar persona {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al desactivar persona {PersonId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        
        
    }
}
