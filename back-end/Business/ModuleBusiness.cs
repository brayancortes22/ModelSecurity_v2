using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los módulos en el sistema.
    /// </summary>
    public class ModuleBusiness
    {
        private readonly ModuleData _moduleData;
        private readonly ILogger<ModuleBusiness> _logger;

        public ModuleBusiness(ModuleData moduleData, ILogger<ModuleBusiness> logger)
        {
            _moduleData = moduleData;
            _logger = logger;
        }

        // Método para obtener todos los módulos como DTOs
        public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
        {
            try
            {
                var modules = await _moduleData.GetAllAsync();
                return MapToDTOList(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de módulos", ex);
            }
        }

        // Método para obtener un módulo por ID como DTO
        public async Task<ModuleDto> GetModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un módulo con ID inválido: {ModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del módulo debe ser mayor que cero");
            }

            try
            {
                var module = await _moduleData.GetByIdAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontró ningún módulo con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }

                return MapToDTO(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el módulo con ID {id}", ex);
            }
        }

        // Método para crear un módulo desde un DTO
        public async Task<ModuleDto> CreateModuleAsync(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);
                var module = MapToEntity(moduleDto);
                module.CreateDate = DateTime.UtcNow;
                var moduleCreado = await _moduleData.CreateAsync(module);
                return MapToDTO(moduleCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo módulo: {Name}", moduleDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el módulo", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateModule(ModuleDto moduleDto)
        {
            if (moduleDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto módulo no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(moduleDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un módulo con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del módulo es obligatorio");
            }
        }

        // Método para actualizar un módulo existente (reemplazo completo)
        public async Task<ModuleDto> UpdateModuleAsync(int id, ModuleDto moduleDto)
        {
            if (id <= 0 || id != moduleDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar un módulo con ID inválido o no coincidente: {ModuleId}, DTO ID: {DtoId}", id, moduleDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateModule(moduleDto); // Reutilizamos la validación

            try
            {
                var existingModule = await _moduleData.GetByIdAsync(id); 
                if (existingModule == null)
                {
                    _logger.LogInformation("No se encontró el módulo con ID {ModuleId} para actualizar", id);
                    throw new EntityNotFoundException("Module", id);
                }

                // Mapear el DTO a la entidad existente (actualización completa)
                existingModule.Name = moduleDto.Name;
                existingModule.Description = moduleDto.Description;
                existingModule.Active = moduleDto.Active;
                existingModule.UpdateDate = DateTime.UtcNow;

                await _moduleData.UpdateAsync(existingModule);
                return MapToDTO(existingModule);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar el módulo con ID {ModuleId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar el módulo con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar el módulo con ID {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el módulo con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente un módulo (PATCH)
        public async Task<ModuleDto> PatchModuleAsync(int id, ModuleDto moduleDto)
        {
             if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a un módulo con ID inválido: {ModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del módulo debe ser mayor que cero.");
            }

            try
            {
                var existingModule = await _moduleData.GetByIdAsync(id); 
                if (existingModule == null)
                {
                    _logger.LogInformation("No se encontró el módulo con ID {ModuleId} para aplicar patch", id);
                    throw new EntityNotFoundException("Module", id);
                }

                bool updated = false;
                existingModule.UpdateDate = DateTime.UtcNow;

                // Actualizar Name si se proporciona y es diferente
                if (!string.IsNullOrWhiteSpace(moduleDto.Name) && moduleDto.Name != existingModule.Name)
                {
                    existingModule.Name = moduleDto.Name;
                    updated = true;
                }
                // Actualizar Description si se proporciona y es diferente (puede ser null)
                if (moduleDto.Description != null && moduleDto.Description != existingModule.Description)
                {
                     existingModule.Description = moduleDto.Description;
                     updated = true;
                }
                // No actualizamos Active en PATCH

                if (updated)
                {
                    await _moduleData.UpdateAsync(existingModule);
                    _logger.LogInformation("Patch aplicado al módulo con ID: {ModuleId}", id);
                }
                else
                {
                    _logger.LogInformation("No se realizaron cambios en el módulo con ID {ModuleId} durante el patch.", id);
                }

                return MapToDTO(existingModule);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch al módulo con ID {ModuleId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el módulo con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch al módulo con ID {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el módulo con ID {id}", ex);
            }
        }

        // Método para eliminar un módulo (DELETE persistente)
        public async Task DeleteModuleAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar un módulo con ID inválido: {ModuleId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del módulo debe ser mayor a 0");
            }
            try
            {
                var existingModule = await _moduleData.GetByIdAsync(id); 
                if (existingModule == null)
                {
                     _logger.LogInformation("No se encontró el módulo con ID {ModuleId} para eliminar", id);
                    throw new EntityNotFoundException("Module", id);
                }

                bool deleted = await _moduleData.DeleteAsync(id);
                if (deleted)
                {
                    _logger.LogInformation("Módulo con ID {ModuleId} eliminado exitosamente", id);
                }
                else
                {
                     _logger.LogWarning("No se pudo eliminar el módulo con ID {ModuleId}.", id);
                    throw new EntityNotFoundException("Module", id); 
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Capturar error si hay FKs
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar el módulo con ID {ModuleId}. Posible violación de FK.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el módulo con ID {id}. Verifique dependencias.", dbEx);
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar el módulo con ID {ModuleId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar el módulo con ID {id}", ex);
            }
        }

        // Método para desactivar (eliminar lógicamente) un módulo
        public async Task SoftDeleteModuleAsync(int id)
        {
             if (id <= 0)
            {
                 _logger.LogWarning("Se intentó realizar soft-delete a un módulo con ID inválido: {ModuleId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del módulo debe ser mayor a 0");
            }

             try
            {
                var existingModule = await _moduleData.GetByIdAsync(id); 
                if (existingModule == null)
                {
                    _logger.LogInformation("No se encontró el módulo con ID {ModuleId} para soft-delete", id);
                    throw new EntityNotFoundException("Module", id);
                }

                 if (!existingModule.Active)
                {
                     _logger.LogInformation("El módulo con ID {ModuleId} ya se encuentra inactivo.", id);
                     return; 
                }

                existingModule.Active = false;
                existingModule.DeleteDate = DateTime.UtcNow;
                await _moduleData.UpdateAsync(existingModule); 
                 _logger.LogInformation("Módulo con ID {ModuleId} desactivado (soft-delete) exitosamente", id);
            }
             catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete del módulo con ID {ModuleId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al desactivar el módulo con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error general al realizar soft-delete del módulo con ID {ModuleId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al desactivar el módulo con ID {id}", ex);
            }
        }

        //Funciones de mapeos 
        // Método para mapear de Module a ModuleDto
        private ModuleDto MapToDTO(Module module)
        {
            return new ModuleDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                Active = module.Active,
            };
        }

        // Método para mapear de ModuleDto a Module
        private Module MapToEntity(ModuleDto moduleDto)
        {
            return new Module
            {
                Id = moduleDto.Id,
                Name = moduleDto.Name,
                Description = moduleDto.Description,
                Active = moduleDto.Active,
            };
        }

        // Método para mapear una lista de Module a una lista de ModuleDto
        private IEnumerable<ModuleDto> MapToDTOList(IEnumerable<Module> modules)
        {
            var modulesDTO = new List<ModuleDto>();
            foreach (var module in modules)
            {
                modulesDTO.Add(MapToDTO(module));
            }
            return modulesDTO;
        }
    }
}
