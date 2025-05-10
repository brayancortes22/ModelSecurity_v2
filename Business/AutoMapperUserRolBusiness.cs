using Business.Base;
using Business.Interfaces;
using Business.Mappers;
using Data;
using Data.Factory;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Implementación de la lógica de negocio para UserRol utilizando AutoMapper
    /// </summary>
    public class AutoMapperUserRolBusiness : AutoMapperGenericBusiness<UserRol, UserRolDto, int>, IGenericBusiness<UserRolDto, int>
    {
        private readonly UserRolData _userRolData;

        public AutoMapperUserRolBusiness(
            IRepositoryFactory repositoryFactory,
            UserRolData userRolData,
            ILogger<AutoMapperUserRolBusiness> logger,
            IMappingService mappingService)
            : base(repositoryFactory, logger, mappingService)
        {
            _userRolData = userRolData ?? throw new ArgumentNullException(nameof(userRolData));
        }

        /// <summary>
        /// Validación específica para ID de UserRol
        /// </summary>
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con una relación Usuario-Rol con ID inválido: {UserRolId}", id);
                throw new ValidationException("id", "El ID de la relación Usuario-Rol debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Validación específica para UserRolDto
        /// </summary>
        protected override void ValidateDto(UserRolDto userRolDto)
        {
            if (userRolDto == null)
            {
                throw new ValidationException("El objeto relación Usuario-Rol no puede ser nulo");
            }

            if (userRolDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación Usuario-Rol con UserId inválido: {UserId}", userRolDto.UserId);
                throw new ValidationException("UserId", "El ID del usuario debe ser mayor que cero");
            }

            if (userRolDto.RolId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una relación Usuario-Rol con RolId inválido: {RolId}", userRolDto.RolId);
                throw new ValidationException("RolId", "El ID del rol debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Implementación para actualizar parcialmente una entidad UserRol
        /// </summary>
        protected override bool PatchEntityFromDto(UserRolDto userRolDto, UserRol userRol)
        {
            bool updated = false;

            if (userRolDto.UserId > 0 && userRolDto.UserId != userRol.UserId)
            {
                userRol.UserId = userRolDto.UserId;
                updated = true;
            }

            if (userRolDto.RolId > 0 && userRolDto.RolId != userRol.RolId)
            {
                userRol.RolId = userRolDto.RolId;
                updated = true;
            }

            return updated;
        }

        /// <summary>
        /// Obtener todos los roles para un usuario específico
        /// </summary>
        public async Task<IEnumerable<UserRolDto>> GetByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ValidationException("userId", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var userRol = await _userRolData.GetByIdAsync(userId);
                return _mappingService.MapCollectionToDto<UserRol, UserRolDto>(userRol != null ? new List<UserRol> { userRol } : new List<UserRol>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los roles para el usuario con ID {userId}");
                throw new ExternalServiceException("Base de datos", $"Error al recuperar los roles para el usuario con ID {userId}", ex);
            }
        }

        /// <summary>
        /// Obtener todos los usuarios para un rol específico
        /// </summary>
        public new async Task<IEnumerable<UserRolDto>> GetByIdAsync(int rolId)
        {
            if (rolId <= 0)
            {
                throw new ValidationException("rolId", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var userRol = await _userRolData.GetByIdAsync(rolId);
                return _mappingService.MapCollectionToDto<UserRol, UserRolDto>(userRol != null ? new List<UserRol> { userRol } : new List<UserRol>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los usuarios para el rol con ID {rolId}");
                throw new ExternalServiceException("Base de datos", $"Error al recuperar los usuarios para el rol con ID {rolId}", ex);
            }
        }
    }
}
