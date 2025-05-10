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
using Data.Interfaces;

namespace Data
{
    public class RolFormData : IGenericRepository<RolForm, int>, IRolFormRepository
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
                _logger.LogError(ex, "Error al obtener rol con ID {Id}", id);
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
                _logger.LogError(ex, "Error al crear el rol: {Message}", ex.Message);
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
                _logger.LogError(ex, "Error al actualizar el rol: {Message}", ex.Message);
                return false;
            }
        }

        // Implementación de métodos faltantes de la interfaz IGenericRepository
        
        /// <summary>
        /// Actualiza parcialmente un rol existente en la base de datos
        /// </summary>
        public async Task<bool> PatchAsync(int id, RolForm entity)
        {
            try
            {
                var existing = await _context.Set<RolForm>().FindAsync(id);
                if (existing == null)
                    return false;

                // Actualiza solo las propiedades que no son null en entity
                if (!string.IsNullOrEmpty(entity.Permission))
                    existing.Permission = entity.Permission;
                
                if (entity.RolId != 0)
                    existing.RolId = entity.RolId;
                
                if (entity.FormId != 0)
                    existing.FormId = entity.FormId;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aplicar patch al rol con ID {Id}: {Message}", id, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Activa un rol en la base de datos (implementación básica ya que RolForm no tiene campo Active)
        /// </summary>
        public async Task<bool> ActivateAsync(int id)
        {
            // RolForm no tiene un campo 'Active', así que este método no tiene una implementación real
            // Devuelve true para cumplir con la interfaz
            _logger.LogWarning("ActivateAsync llamado para RolForm, pero la entidad no tiene campo Active");
            return true;
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
                _logger.LogError(ex, "Error al eliminar el rol con ID {Id}: {Message}", id, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Realiza un borrado lógico (no aplicable a RolForm que no tiene campo para borrado lógico)
        /// </summary>
        public async Task<bool> SoftDeleteAsync(int id)
        {
            // RolForm no tiene un campo para borrado lógico, así que este método no tiene una implementación real
            // Devuelve true para cumplir con la interfaz
            _logger.LogWarning("SoftDeleteAsync llamado para RolForm, pero la entidad no tiene campo para borrado lógico");
            return true;
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

        /// <summary>
        /// Obtiene todos los formularios asociados a un rol
        /// </summary>
        /// <param name="rolId">ID del rol</param>
        /// <returns>Lista de RolForm para el rol especificado</returns>
        public async Task<IEnumerable<RolForm>> GetByRolIdAsync(int rolId)
        {
            try
            {
                _logger.LogInformation("Obteniendo RolForms para el rol con ID: {RolId}", rolId);
                return await _context.RolForm
                    .Where(rf => rf.RolId == rolId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener RolForms para el rol con ID: {RolId}", rolId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los roles asociados a un formulario
        /// </summary>
        /// <param name="formId">ID del formulario</param>
        /// <returns>Lista de RolForm para el formulario especificado</returns>
        public async Task<IEnumerable<RolForm>> GetByFormIdAsync(int formId)
        {
            try
            {
                _logger.LogInformation("Obteniendo RolForms para el formulario con ID: {FormId}", formId);
                return await _context.RolForm
                    .Where(rf => rf.FormId == formId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener RolForms para el formulario con ID: {FormId}", formId);
                throw;
            }
        }
    }
}
