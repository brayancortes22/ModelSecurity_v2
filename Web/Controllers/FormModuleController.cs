using Business;
using Data;
using Entity.DTOautogestion;
using Entity.DTOautogestion.pivote;
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
    /// Controlador para la gestión de módulos de formularios en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FormModuleController : ControllerBase
    {
        private readonly FormModuleBusiness _FormModuleBusiness;
        private readonly ILogger<FormModuleController> _logger;

        /// <summary>
        /// Constructor del controlador de módulos de formularios
        /// </summary>
        /// <param name="formModuleBusiness">Capa de negocio de módulos de formularios</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public FormModuleController(FormModuleBusiness formModuleBusiness, ILogger<FormModuleController> logger)
        {
            _FormModuleBusiness = formModuleBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los módulos de formularios del sistema
        /// </summary>
        /// <returns>Lista de módulos de formularios</returns>
        /// <response code="200">Retorna la lista de módulos de formularios</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormModuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllFormModules()
        {
            try
            {
                var formModules = await _FormModuleBusiness.GetAllFormModulesAsync();
                return Ok(formModules);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener módulos de formularios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un módulo de formulario específico por su ID
        /// </summary>
        /// <param name="id">ID del módulo de formulario</param>
        /// <returns>Módulo de formulario solicitado</returns>
        /// <response code="200">Retorna el módulo de formulario solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Módulo de formulario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFormModuleById(int id)
        {
            try
            {
                var formModule = await _FormModuleBusiness.GetFormModuleByIdAsync(id);
                return Ok(formModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el módulo de formulario con ID: {FormModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo de formulario no encontrado con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener módulo de formulario con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo módulo de formulario en el sistema
        /// </summary>
        /// <param name="formModuleDto">Datos del módulo de formulario a crear</param>
        /// <returns>Módulo de formulario creado</returns>
        /// <response code="201">Retorna el módulo de formulario creado</response>
        /// <response code="400">Datos del módulo de formulario no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(FormModuleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateFormModule([FromBody] FormModuleDto formModuleDto)
        {
            try
            {
                var createdFormModule = await _FormModuleBusiness.CreateFormModuleAsync(formModuleDto);
                return CreatedAtAction(nameof(GetFormModuleById), new { id = createdFormModule.Id }, createdFormModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear módulo de formulario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear módulo de formulario");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una relación formulario-módulo existente
        /// </summary>
        /// <param name="id">ID de la relación a actualizar</param>
        /// <param name="formModuleDto">Datos completos de la relación para actualizar</param>
        /// <returns>Relación actualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(FormModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateFormModule(int id, [FromBody] FormModuleDto formModuleDto)
        {
            try
            {
                var updatedRelation = await _FormModuleBusiness.UpdateFormModuleAsync(id, formModuleDto);
                return Ok(updatedRelation);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar relación formulario-módulo con ID: {FormModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación formulario-módulo no encontrada para actualizar con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar relación formulario-módulo con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente una relación formulario-módulo (ej. StatusProcedure)
        /// </summary>
        /// <param name="id">ID de la relación a actualizar</param>
        /// <param name="formModuleDto">Datos parciales a aplicar (principalmente StatusProcedure)</param>
        /// <returns>Relación actualizada</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(FormModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchFormModule(int id, [FromBody] FormModuleDto formModuleDto)
        {
            try
            {
                var patchedRelation = await _FormModuleBusiness.PatchFormModuleAsync(id, formModuleDto);
                return Ok(patchedRelation);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a relación formulario-módulo con ID: {FormModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación formulario-módulo no encontrada para aplicar patch con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch a relación formulario-módulo con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una relación formulario-módulo existente
        /// </summary>
        /// <param name="id">ID de la relación a eliminar</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteFormModule(int id)
        {
            try
            {
                await _FormModuleBusiness.DeleteFormModuleAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar relación formulario-módulo con ID: {FormModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Relación formulario-módulo no encontrada para eliminar con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar relación formulario-módulo con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
