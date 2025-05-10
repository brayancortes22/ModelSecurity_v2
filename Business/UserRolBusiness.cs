using Business.Base;
using Business.Interfaces;
using Data;
using Data.Factory;
using Data.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los usuarios y sus roles en el sistema.
    /// </summary>
    public class UserRolBusiness : GenericBusiness<UserRol, UserRolDto, int>, IGenericBusiness<UserRolDto, int>
    {
        private readonly UserRolData _userRolDataSpecific;

        public UserRolBusiness(
            IRepositoryFactory repositoryFactory,
            UserRolData userRolDataSpecific,
            ILogger<UserRolBusiness> logger)
            : base(repositoryFactory, logger)
        {
            _userRolDataSpecific = userRolDataSpecific ?? throw new ArgumentNullException(nameof(userRolDataSpecific));
        }
        
        public UserRolBusiness(
            IGenericRepository<UserRol, int> repository,
            UserRolData userRolDataSpecific,
            ILogger<UserRolBusiness> logger)
            : base(repository, logger)
        {
            _userRolDataSpecific = userRolDataSpecific ?? throw new ArgumentNullException(nameof(userRolDataSpecific));
        }

        /// <summary>
        /// Obtiene todas las asignaciones de roles para un usuario específico
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Lista de asignaciones de roles para el usuario indicado</returns>
        public async Task<IEnumerable<UserRolDto>> GetRolesByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Se intentó obtener roles con ID de usuario inválido: {UserId}", userId);
                throw new ValidationException("userId", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var userRoles = await _userRolDataSpecific.GetRolesByUserIdAsync(userId);
                return MapToDtoList(userRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles del usuario con ID: {UserId}", userId);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar los roles para el usuario con ID {userId}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios asignados a un rol específico
        /// </summary>
        /// <param name="rolId">ID del rol</param>
        /// <returns>Lista de asignaciones de usuarios para el rol indicado</returns>
        public async Task<IEnumerable<UserRolDto>> GetUsersByRolIdAsync(int rolId)
        {
            if (rolId <= 0)
            {
                _logger.LogWarning("Se intentó obtener usuarios con ID de rol inválido: {RolId}", rolId);
                throw new ValidationException("rolId", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var userRoles = await _userRolDataSpecific.GetUsersByRolIdAsync(rolId);
                return MapToDtoList(userRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios del rol con ID: {RolId}", rolId);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar los usuarios para el rol con ID {rolId}", ex);
            }
        }

        // Implementaciones específicas de los métodos abstractos
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con una relación UserRol con ID inválido: {UserRolId}", id);
                throw new ValidationException("id", "El ID de la relación UserRol debe ser mayor que cero");
            }
        }

        protected override void ValidateDto(UserRolDto userRolDto)
        {
            if (userRolDto == null)
            {
                throw new ValidationException("El objeto UserRol no puede ser nulo");
            }

            if (userRolDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación UserRol con UserId inválido");
                throw new ValidationException("UserId", "El UserId es obligatorio y debe ser mayor que cero");
            }

            if (userRolDto.RolId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación UserRol con RolId inválido");
                throw new ValidationException("RolId", "El RolId es obligatorio y debe ser mayor que cero");
            }
        }

        protected override UserRolDto MapToDto(UserRol userRol)
        {
            return new UserRolDto
            {
                Id = userRol.Id,
                UserId = userRol.UserId,
                RolId = userRol.RolId
            };
        }

        protected override UserRol MapToEntity(UserRolDto userRolDto)
        {
            return new UserRol
            {
                Id = userRolDto.Id,
                UserId = userRolDto.UserId,
                RolId = userRolDto.RolId,
                Active = true,
                CreateDate = DateTime.UtcNow,
                // No inicializamos las propiedades de navegación para evitar referencias circulares
                User = null,
                Rol = null
            };
        }

        protected override void UpdateEntityFromDto(UserRolDto userRolDto, UserRol userRol)
        {
            userRol.UserId = userRolDto.UserId;
            userRol.RolId = userRolDto.RolId;
            userRol.UpdateDate = DateTime.UtcNow;
        }

        protected override bool PatchEntityFromDto(UserRolDto userRolDto, UserRol userRol)
        {
            bool updated = false;

            // Actualizar UserId si se proporciona y es diferente
            if (userRolDto.UserId > 0 && userRol.UserId != userRolDto.UserId)
            {
                userRol.UserId = userRolDto.UserId;
                updated = true;
            }

            // Actualizar RolId si se proporciona y es diferente
            if (userRolDto.RolId > 0 && userRol.RolId != userRolDto.RolId)
            {
                userRol.RolId = userRolDto.RolId;
                updated = true;
            }

            if (updated)
            {
                userRol.UpdateDate = DateTime.UtcNow;
            }

            return updated;
        }

        protected override IEnumerable<UserRolDto> MapToDtoList(IEnumerable<UserRol> userRoles)
        {
            return userRoles.Select(MapToDto).ToList();
        }
    }
}

