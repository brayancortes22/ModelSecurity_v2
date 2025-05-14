using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data.Interfaces;
using Data.Repositories;

namespace Data
{
    public class FormModuleData : GenericRepository<FormModule, int>, IGenericRepository<FormModule, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormModuleData> _logger;

        public FormModuleData(ApplicationDbContext context, ILogger<FormModuleData> logger) 
            : base(context, logger)
        {
            _context = context;
            _logger = logger;
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
                    .Include(fm => fm.Form)
                    .Where(fm => fm.ModuleId == moduleId && fm.Form.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener formularios para el módulo con ID {moduleId}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los módulos asociados a un formulario específico
        /// </summary>
        /// <param name="formId">ID del formulario</param>
        /// <returns>Lista de relaciones FormModule para el formulario indicado</returns>
        public async Task<IEnumerable<FormModule>> GetModulesByFormIdAsync(int formId)
        {
            try
            {                
                return await _context.Set<FormModule>()
                    .Include(fm => fm.Module)
                    .Where(fm => fm.FormId == formId && fm.Module.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener módulos para el formulario con ID {formId}");
                throw;
            }
        }
    }
}
