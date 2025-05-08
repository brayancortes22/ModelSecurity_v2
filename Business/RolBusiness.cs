using Business.Base;
using Business.Interfaces;
using Data;
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
    /// Clase de negocio encargada de la lógica relacionada con los roles en el sistema.
    /// </summary>
    public class RolBusiness : GenericBusiness<Rol, RolDto, int>, IGenericBusiness<RolDto, int>
    {
        private readonly RolFormData _rolFormData; // Para obtener los formularios asociados a un rol

        public RolBusiness(IGenericRepository<Rol, int> repository, RolFormData rolFormData, ILogger<RolBusiness> logger)
            : base(repository, logger)
        {
            _rolFormData = rolFormData ?? throw new ArgumentNullException(nameof(rolFormData));
        }

        // Implementaciones específicas de los métodos abstractos
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con un rol con ID inválido: {RolId}", id);
                throw new ValidationException("id", "El ID del rol debe ser mayor que cero");
            }
        }

        protected override void ValidateDto(RolDto rolDto)
        {
            if (rolDto == null)
            {
                throw new ValidationException("El objeto rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(rolDto.TypeRol))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con nombre vacío");
                throw new ValidationException("TypeRol", "El tipo de rol es obligatorio");
            }
        }

        protected override RolDto MapToDto(Rol rol)
        {
            return new RolDto
            {
                Id = rol.Id,
                TypeRol = rol.TypeRol,
                Description = rol.Description,
                Active = rol.Active
            };
        }

        protected override Rol MapToEntity(RolDto rolDto)
        {
            return new Rol
            {
                Id = rolDto.Id,
                TypeRol = rolDto.TypeRol,
                Description = rolDto.Description,
                Active = rolDto.Active
            };
        }

        protected override void UpdateEntityFromDto(RolDto rolDto, Rol rol)
        {
            rol.TypeRol = rolDto.TypeRol;
            rol.Description = rolDto.Description;
            rol.Active = rolDto.Active;
        }

        protected override bool PatchEntityFromDto(RolDto rolDto, Rol rol)
        {
            bool updated = false;

            if (!string.IsNullOrWhiteSpace(rolDto.TypeRol) && rolDto.TypeRol != rol.TypeRol)
            {
                rol.TypeRol = rolDto.TypeRol;
                updated = true;
            }
            
            if (rolDto.Description != null && rolDto.Description != rol.Description)
            {
                rol.Description = rolDto.Description;
                updated = true;
            }

            return updated;
        }

        protected override IEnumerable<RolDto> MapToDtoList(IEnumerable<Rol> rols)
        {
            return rols.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Obtiene todos los formularios asignados a un rol específico
        /// </summary>
        /// <param name="rolId">ID del rol</param>
        /// <returns>Lista de formularios asignados al rol</returns>
        public async Task<IEnumerable<FormDto>> GetFormsByRolIdAsync(int rolId)
        {
            if (rolId <= 0)
            {
                _logger.LogWarning("Se intentó obtener formularios para un rol con ID inválido: {RolId}", rolId);
                throw new ValidationException("rolId", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                // Verificar que el rol existe
                var rol = await GetByIdAsync(rolId);
                
                // Obtener los formularios asignados al rol utilizando RolFormData
                var formDtos = await _rolFormData.GetFormsByRolIdAsync(rolId);
                
                return formDtos;
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar para mantener el mensaje específico
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los formularios para el rol con ID: {RolId}", rolId);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar los formularios para el rol con ID {rolId}", ex);
            }
        }
    }
}
