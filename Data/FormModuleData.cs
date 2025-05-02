using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class FormModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormModuleData> _logger;

        public FormModuleData(ApplicationDbContext context, ILogger<FormModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<FormModule>> GetAllAsync()
        {
            return await _context.Set<FormModule>().ToListAsync();
        }

        public async Task<FormModule?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<FormModule>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError( $"Error al obtener Formulario-Módulo con ID {id}");
                throw;
            }
        }

        public async Task<FormModule> CreateAsync(FormModule formModule)
        {
            try
            {
                await _context.Set<FormModule>().AddAsync(formModule);
                await _context.SaveChangesAsync();
                return formModule;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la relación Formulario-Módulo {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(FormModule formModule)
        {
            try
            {
                _context.Set<FormModule>().Update(formModule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la relación Formulario-Módulo {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var formModule = await _context.Set<FormModule>().FindAsync(id);
                if (formModule == null)
                    return false;

                _context.Set<FormModule>().Remove(formModule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la relación Formulario-Módulo {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene todas las relaciones FormModule para un módulo específico
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <returns>Lista de relaciones FormModule para el módulo indicado</returns>
        public async Task<IEnumerable<FormModule>> GetFormsByModuleIdAsync(int moduleId)
        {
            try
            {
                return await _context.Set<FormModule>()
                    .Where(fm => fm.ModuleId == moduleId && fm.StatusProcedure == "true")
                    .Include(fm => fm.Form)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener formularios para el módulo con ID {moduleId}");
                throw;
            }
        }
    }
}
