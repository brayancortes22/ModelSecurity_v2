using Business.Interfaces;
using Entity.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Web.Controllers.Base
{
    /// <summary>
    /// Controlador base que proporciona operaciones para activar/desactivar entidades
    /// </summary>
    /// <typeparam name="T">Tipo de entidad que implementa IActivable</typeparam>
    public abstract class ActivacionControllerBase<T> : ControllerBase where T : class, IActivable
    {
        protected readonly IActivacionBusiness<T, int> _activacionBusiness;

        protected ActivacionControllerBase(IActivacionBusiness<T, int> activacionBusiness)
        {
            _activacionBusiness = activacionBusiness;
        }

        /// <summary>
        /// Activa una entidad
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPatch("{id}/activar")]
        public virtual async Task<IActionResult> Activar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("El ID debe ser mayor que cero");
                }

                bool resultado = await _activacionBusiness.ActivarAsync(id);

                if (resultado)
                {
                    return Ok(new { mensaje = $"Entidad con ID {id} activada correctamente" });
                }
                else
                {
                    return NotFound($"No se encontró la entidad con ID {id}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al activar la entidad: {ex.Message}");
            }
        }
    }
}