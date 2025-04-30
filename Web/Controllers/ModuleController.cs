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
    /// Controlador para la gestión de módulos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ModuleController : ControllerBase
    {
        private readonly ModuleBusiness _ModuleBusiness;
        private readonly ILogger<ModuleController> _logger;

        /// <summary>
        /// Constructor del controlador de módulos
        /// </summary>
        /// <param name="moduleBusiness">Capa de negocio de módulos</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public ModuleController(ModuleBusiness moduleBusiness, ILogger<ModuleController> logger)
        {
            _ModuleBusiness = moduleBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los módulos del sistema
        /// </summary>
        /// <returns>Lista de módulos</returns>
        /// <response code="200">Retorna la lista de módulos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ModuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllModules()
        {
            try
            {
                var modules = await _ModuleBusiness.GetAllModulesAsync();
                return Ok(modules);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener módulos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un módulo específico por su ID
        /// </summary>
        /// <param name="id">ID del módulo</param>
        /// <returns>Módulo solicitado</returns>
        /// <response code="200">Retorna el módulo solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Módulo no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetModuleById(int id)
        {
            try
            {
                var module = await _ModuleBusiness.GetModuleByIdAsync(id);
                return Ok(module);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el módulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo módulo en el sistema
        /// </summary>
        /// <param name="moduleDto">Datos del módulo a crear</param>
        /// <returns>Módulo creado</returns>
        /// <response code="201">Retorna el módulo creado</response>
        /// <response code="400">Datos del módulo no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ModuleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateModule([FromBody] ModuleDto moduleDto)
        {
            try
            {
                var createdModule = await _ModuleBusiness.CreateModuleAsync(moduleDto);
                return CreatedAtAction(nameof(GetModuleById), new { id = createdModule.Id }, createdModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear módulo");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear módulo");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un módulo existente (reemplazo completo)
        /// </summary>
        /// <param name="id">ID del módulo a actualizar</param>
        /// <param name="moduleDto">Datos completos del módulo para actualizar</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateModule(int id, [FromBody] ModuleDto moduleDto)
        {
            try
            {
                var updatedModule = await _ModuleBusiness.UpdateModuleAsync(id, moduleDto);
                return Ok(updatedModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar módulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado para actualizar con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un módulo existente
        /// </summary>
        /// <param name="id">ID del módulo a actualizar</param>
        /// <param name="moduleDto">Datos parciales a aplicar (Name, Description)</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchModule(int id, [FromBody] ModuleDto moduleDto)
        {
            try
            {
                var patchedModule = await _ModuleBusiness.PatchModuleAsync(id, moduleDto);
                return Ok(patchedModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a módulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado para aplicar patch con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch a módulo con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina permanentemente un módulo
        /// </summary>
        /// <remarks>Precaución: Esta operación es irreversible y fallará si existen entidades dependientes.</remarks>
        /// <param name="id">ID del módulo a eliminar</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)] // O 409 Conflict
        public async Task<IActionResult> DeleteModule(int id)
        {
            try
            {
                await _ModuleBusiness.DeleteModuleAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar módulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado para eliminar con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex) // Puede ser error de FK
            {
                _logger.LogError(ex, "Error al eliminar módulo con ID: {ModuleId}. Posible dependencia.", id);
                return StatusCode(500, new { message = "Error al eliminar el módulo. Verifique si hay dependencias." });
            }
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) un módulo
        /// </summary>
        /// <param name="id">ID del módulo a desactivar</param>
        [HttpDelete("{id}/soft")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteModule(int id)
        {
            try
            {
                await _ModuleBusiness.SoftDeleteModuleAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al realizar soft-delete de módulo con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado para soft-delete con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al realizar soft-delete de módulo con ID: {ModuleId}", id);
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
        public async Task<IActionResult> ActivateModule(int id)
        {
            try
            {
                await _ModuleBusiness.ActivateModuleAsync(id);
                return NoContent(); // Opcionalmente podrías devolver el usuario activado (Ok(ModuleDto))
            }
            catch (ValidationException ex)
            {
                 _logger.LogWarning(ex, "Validación fallida al intentar activar módulo {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Módulo no encontrado para activar con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error de servicio externo al activar módulo {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error inesperado al activar módulo {ModuleId}", id);
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

    }
}
