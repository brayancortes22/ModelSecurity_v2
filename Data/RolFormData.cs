using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Entity.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class RolFormData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolFormData> _logger;
        /// <summary>
        /// Constructor que recibe el contexto de la base de datos 
        /// </summary>
        /// <param name="context"> instancia de <see cref="ApplicationDbContext"/>para la conexion con la base de datos</param>
        public RolFormData(ApplicationDbContext context, ILogger<RolFormData> logger)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Obtiene todos los roles almacenados en la base de datos
        /// </summary>
        /// <returns> Lista de roles </returns>
        public async Task<IEnumerable<RolForm>> GetAllAsync()
        {
            return await _context.Set<RolForm>().ToListAsync();
        }

        public async Task<RolForm> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<RolForm>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener rol con ID{id}");
                throw;// Re-lanza la excepcion para que sea manejada en capas superiores
            }

        }

        /// <summary>
        /// Crea un nuevo rolForm en la base de datos 
        /// </summary>
        /// <param name="rolForm">instancia del rol a crear.</param>
        /// <returns>el rolForm creado</returns>
        public async Task<RolForm> CreateAsync(RolForm rolForm)
        {
            try
            {
                await _context.Set<RolForm>().AddAsync(rolForm);
                await _context.SaveChangesAsync();
                return rolForm;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el rol {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un rol existente en la base de datos 
        /// </summary>
        /// <param name="rolForm">Objeto con la infromacion actualizada</param>
        /// <returns>True si la operacion fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(RolForm rolForm)
        {
            try
            {
                _context.Set<RolForm>().Update(rolForm);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un rol en la base de datos 
        /// </summary>
        /// <param name="id">Identificador unico del rol a eliminar</param>
        /// <returns>True si la eliminacion fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolForm = await _context.Set<RolForm>().FindAsync(id);
                if (rolForm == null)
                    return false;

                _context.Set<RolForm>().Remove(rolForm);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el rol {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene todos los formularios asignados a un rol específico
        /// </summary>
        /// <param name="rolId">ID del rol</param>
        /// <returns>Lista de formularios asignados al rol</returns>
        public async Task<IEnumerable<FormDto>> GetFormsByRolIdAsync(int rolId)
        {
            try
            {
                // Obtenemos los formularios asignados al rol a través de la relación RolForm
                var formDtos = await _context.RolForm
                    .Where(rf => rf.RolId == rolId)
                    .Join(_context.Form,
                          rf => rf.FormId,
                          form => form.Id,
                          (rf, form) => new FormDto
                          {
                              Id = form.Id,
                              Name = form.Name,
                              Route = form.Route,
                              Active = form.Active
                          })
                    .ToListAsync();

                return formDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener formularios para el rol con ID: {RolId}", rolId);
                throw; // Re-lanzamos para que sea manejada en la capa superior
            }
        }
    }
}
