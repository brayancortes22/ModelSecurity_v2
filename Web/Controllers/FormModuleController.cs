using Business;
using Business.Interfaces;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

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
        private readonly IGenericBusiness<FormModuleDto, int> _genericBusiness;
        private readonly FormModuleBusiness _formModuleBusiness;
        private readonly ILogger<FormModuleController> _logger;

        /// <summary>
        /// Constructor del controlador de módulos de formularios
        /// </summary>
        /// <param name="genericBusiness">Interfaz genérica de negocio</param>
        /// <param name="formModuleBusiness">Implementación específica de negocio</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public FormModuleController(
            IGenericBusiness<FormModuleDto, int> genericBusiness,
            FormModuleBusiness formModuleBusiness,
            ILogger<FormModuleController> logger)
        {
            _genericBusiness = genericBusiness;
            _formModuleBusiness = formModuleBusiness;
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
                var formModules = await _genericBusiness.GetAllAsync();
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
                var formModule = await _genericBusiness.GetByIdAsync(id);
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
                var createdFormModule = await _genericBusiness.CreateAsync(formModuleDto);
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
                formModuleDto.Id = id; // Asegurar que el ID coincida
                var updatedRelation = await _genericBusiness.UpdateAsync(id, formModuleDto);
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
        /// Actualiza parcialmente una relación formulario-módulo
        /// </summary>
        /// <param name="id">ID de la relación a actualizar</param>
        /// <param name="formModuleDto">Datos parciales a aplicar</param>
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
                formModuleDto.Id = id; // Asegurar que el ID coincida
                var patchedRelation = await _genericBusiness.PatchAsync(id, formModuleDto);
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
                await _genericBusiness.DeleteAsync(id);
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

        /// <summary>
        /// Obtiene todos los formularios asignados a un módulo específico
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <returns>Lista de relaciones FormModule para el módulo indicado</returns>
        [HttpGet("by-module/{moduleId}")]
        [ProducesResponseType(typeof(IEnumerable<FormModuleDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFormsByModuleId(int moduleId)
        {
            try
            {
                var forms = await _formModuleBusiness.GetFormsByModuleIdAsync(moduleId);
                return Ok(forms);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al obtener formularios para el módulo con ID: {ModuleId}", moduleId);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener formularios para el módulo con ID: {ModuleId}", moduleId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los módulos asignados a un formulario específico
        /// </summary>
        /// <param name="formId">ID del formulario</param>
        /// <returns>Lista de relaciones FormModule para el formulario indicado</returns>
        [HttpGet("by-form/{formId}")]
        [ProducesResponseType(typeof(IEnumerable<FormModuleDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetModulesByFormId(int formId)
        {
            try
            {
                var modules = await _formModuleBusiness.GetModulesByFormIdAsync(formId);
                return Ok(modules);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al obtener módulos para el formulario con ID: {FormId}", formId);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener módulos para el formulario con ID: {FormId}", formId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
