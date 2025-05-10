using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de roles de formulario en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolFormController : ControllerBase
    {
        private readonly RolFormBusiness _rolFormBusiness;
        private readonly ILogger<RolFormController> _logger;

        /// <summary>
        /// Constructor del controlador de roles de formulario
        /// </summary>
        public RolFormController(RolFormBusiness rolFormBusiness, ILogger<RolFormController> logger)
        {
            _rolFormBusiness = rolFormBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los roles de formulario del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolFormDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolForms()
        {
            try
            {                var rolForms = await _rolFormBusiness.GetAllAsync();
                return Ok(rolForms);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener roles de formulario");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un rol de formulario específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolFormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolFormById(int id)
        {
            try
            {
                var rolForm = await _rolFormBusiness.GetByIdAsync(id);
                return Ok(rolForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el rol de formulario con ID: {RolFormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol de formulario no encontrado con ID: {RolFormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener rol de formulario con ID: {RolFormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo rol de formulario en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RolFormDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolForm([FromBody] RolFormDto rolFormDto)
        {
            try
            {
                var createdRolForm = await _rolFormBusiness.CreateAsync(rolFormDto);
                return CreatedAtAction(nameof(GetRolFormById), new { id = createdRolForm.Id }, createdRolForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear rol de formulario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear rol de formulario");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una relación rol-formulario existente
        /// </summary>
        /// <param name="id">ID de la relación a actualizar</param>
        /// <param name="rolFormDto">Datos completos de la relación para actualizar</param>
        /// <returns>Relación actualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RolFormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolForm(int id, [FromBody] RolFormDto rolFormDto)
        {
            try
            {
                var updatedRolForm = await _rolFormBusiness.UpdateAsync(id, rolFormDto);
                return Ok(updatedRolForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar relación rol-formulario con ID: {RolFormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación rol-formulario no encontrada para actualizar con ID: {RolFormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar relación rol-formulario con ID: {RolFormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente una relación rol-formulario (ej. el permiso)
        /// </summary>
        /// <param name="id">ID de la relación a actualizar</param>
        /// <param name="rolFormDto">Datos parciales a aplicar (principalmente Permission)</param>
        /// <returns>Relación actualizada</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(RolFormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchRolForm(int id, [FromBody] RolFormDto rolFormDto)
        {
            try
            {
                var patchedRolForm = await _rolFormBusiness.PatchAsync(id, rolFormDto);
                return Ok(patchedRolForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a relación rol-formulario con ID: {RolFormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación rol-formulario no encontrada para aplicar patch con ID: {RolFormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch a relación rol-formulario con ID: {RolFormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una relación rol-formulario existente
        /// </summary>
        /// <param name="id">ID de la relación a eliminar</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRolForm(int id)
        {
            try
            {
                await _rolFormBusiness.DeleteAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar relación rol-formulario con ID: {RolFormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación rol-formulario no encontrada para eliminar con ID: {RolFormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar relación rol-formulario con ID: {RolFormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todas las asignaciones de formularios a un rol específico
        /// </summary>
        /// <param name="rolId">ID del rol</param>
        /// <returns>Lista de asignaciones de formularios al rol</returns>
        [HttpGet("byRol/{rolId}")]
        [ProducesResponseType(typeof(IEnumerable<RolFormDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolFormsByRolId(int rolId)
        {
            try
            {
                var rolForms = await _rolFormBusiness.GetByRolIdAsync(rolId);
                return Ok(rolForms);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al obtener formularios para el rol con ID: {RolId}", rolId);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener formularios para el rol con ID: {RolId}", rolId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todas las asignaciones de roles a un formulario específico
        /// </summary>
        /// <param name="formId">ID del formulario</param>
        /// <returns>Lista de asignaciones de roles al formulario</returns>
        [HttpGet("byForm/{formId}")]
        [ProducesResponseType(typeof(IEnumerable<RolFormDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolFormsByFormId(int formId)
        {
            try
            {
                var rolForms = await _rolFormBusiness.GetByFormIdAsync(formId);
                return Ok(rolForms);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al obtener roles para el formulario con ID: {FormId}", formId);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener roles para el formulario con ID: {FormId}", formId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
