using Business;
using Business.Factory;
using Business.Interfaces;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de roles en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolController : ControllerBase
    {
        private readonly IGenericBusiness<RolDto, int> _rolBusiness;
        private readonly RolBusiness _rolBusinessSpecific; // Para métodos específicos
        private readonly ILogger<RolController> _logger;
        private readonly IBusinessFactory _businessFactory;

        /// <summary>
        /// Constructor del controlador de roles utilizando BusinessFactory
        /// </summary>
        /// <param name="businessFactory">Fábrica de servicios de negocio</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public RolController(
            IBusinessFactory businessFactory,
            ILogger<RolController> logger)
        {
            _businessFactory = businessFactory ?? throw new ArgumentNullException(nameof(businessFactory));
            _rolBusiness = _businessFactory.CreateBusiness<RolDto, int>();
            _rolBusinessSpecific = _businessFactory.CreateSpecificBusiness<RolBusiness>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Constructor alternativo que mantiene compatibilidad con código existente
        /// </summary>
        public RolController(
            IGenericBusiness<RolDto, int> rolBusiness, 
            RolBusiness rolBusinessSpecific, 
            ILogger<RolController> logger)
        {
            _rolBusiness = rolBusiness ?? throw new ArgumentNullException(nameof(rolBusiness));
            _rolBusinessSpecific = rolBusinessSpecific ?? throw new ArgumentNullException(nameof(rolBusinessSpecific));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _businessFactory = null;
        }

        /// <summary>
        /// Obtiene todos los roles del sistema
        /// </summary>
        /// <returns>Lista de roles</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRols()
        {
            try
            {
                var roles = await _rolBusiness.GetAllAsync();
                return Ok(roles);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener roles");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un rol específico por su ID
        /// </summary>
        /// <param name="id">ID del rol</param>
        /// <returns>Rol solicitado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolById(int id)
        {
            try
            {
                var rol = await _rolBusiness.GetByIdAsync(id);
                return Ok(rol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo rol en el sistema
        /// </summary>
        /// <param name="rolDto">Datos del rol a crear</param>
        /// <returns>Rol creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RolDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRol([FromBody] RolDto rolDto)
        {
            try
            {
                var createdRol = await _rolBusiness.CreateAsync(rolDto);
                return CreatedAtAction(nameof(GetRolById), new { id = createdRol.Id }, createdRol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear rol");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear rol");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un rol existente
        /// </summary>
        /// <param name="id">ID del rol a actualizar</param>
        /// <param name="rolDto">Datos actualizados del rol</param>
        /// <returns>Rol actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRol(int id, [FromBody] RolDto rolDto)
        {
            try
            {
                var updatedRol = await _rolBusiness.UpdateAsync(id, rolDto);
                return Ok(updatedRol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado para actualizar con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente un rol existente
        /// </summary>
        /// <param name="id">ID del rol a actualizar</param>
        /// <param name="rolDto">Datos parciales del rol</param>
        /// <returns>Rol actualizado</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(RolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchRol(int id, [FromBody] RolDto rolDto)
        {
            try
            {
                var patchedRol = await _rolBusiness.PatchAsync(id, rolDto);
                return Ok(patchedRol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al aplicar patch a rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado para patch con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al aplicar patch a rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un rol del sistema
        /// </summary>
        /// <param name="id">ID del rol a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRol(int id)
        {
            try
            {
                await _rolBusiness.DeleteAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado para eliminar con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Desactiva un rol (eliminación lógica)
        /// </summary>
        /// <param name="id">ID del rol a desactivar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("soft/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteRol(int id)
        {
            try
            {
                await _rolBusiness.SoftDeleteAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al desactivar rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado para desactivar con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al desactivar rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activa un rol previamente desactivado
        /// </summary>
        /// <param name="id">ID del rol a activar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("activate/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ActivateRol(int id)
        {
            try
            {
                await _rolBusiness.ActivateAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al activar rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado para activar con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al activar rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
