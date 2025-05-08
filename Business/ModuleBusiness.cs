using Business.Base;
using Business.Interfaces;
using Data.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los módulos en el sistema.
    /// </summary>
    public class ModuleBusiness : GenericBusiness<Module, ModuleDto, int>, IGenericBusiness<ModuleDto, int>
    {
        public ModuleBusiness(IGenericRepository<Module, int> repository, ILogger<ModuleBusiness> logger)
            : base(repository, logger)
        {
        }

        // Implementaciones específicas de los métodos abstractos
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con un módulo con ID inválido: {ModuleId}", id);
                throw new ValidationException("id", "El ID del módulo debe ser mayor que cero");
            }
        }

        protected override void ValidateDto(ModuleDto moduleDto)
        {
            if (moduleDto == null)
            {
                throw new ValidationException("El objeto módulo no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(moduleDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un módulo con Name vacío");
                throw new ValidationException("Name", "El Name del módulo es obligatorio");
            }
        }

        protected override ModuleDto MapToDto(Module module)
        {
            return new ModuleDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                Active = module.Active
            };
        }

        protected override Module MapToEntity(ModuleDto moduleDto)
        {
            return new Module
            {
                Id = moduleDto.Id,
                Name = moduleDto.Name,
                Description = moduleDto.Description,
                Active = moduleDto.Active
            };
        }

        protected override void UpdateEntityFromDto(ModuleDto moduleDto, Module module)
        {
            module.Name = moduleDto.Name;
            module.Description = moduleDto.Description;
            module.Active = moduleDto.Active;
        }

        protected override bool PatchEntityFromDto(ModuleDto moduleDto, Module module)
        {
            bool updated = false;

            if (!string.IsNullOrWhiteSpace(moduleDto.Name) && moduleDto.Name != module.Name)
            {
                module.Name = moduleDto.Name;
                updated = true;
            }
            
            if (moduleDto.Description != null && moduleDto.Description != module.Description)
            {
                module.Description = moduleDto.Description;
                updated = true;
            }

            return updated;
        }

        protected override IEnumerable<ModuleDto> MapToDtoList(IEnumerable<Module> modules)
        {
            return modules.Select(MapToDto).ToList();
        }
    }
}
