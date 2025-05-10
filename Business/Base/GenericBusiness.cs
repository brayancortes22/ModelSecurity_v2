using Business.Interfaces;
using Data.Factory;
using Data.Interfaces;
using Entity.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business.Base
{
    /// <summary>
    /// Clase base para implementar servicios de negocio con operaciones CRUD
    /// </summary>
    public abstract class GenericBusiness<TEntity, TDto, TId> : IGenericBusiness<TDto, TId>
        where TEntity : class, IEntity
        where TDto : class
        where TId : IConvertible
    {
        protected readonly IGenericRepository<TEntity, TId> _repository;
        protected readonly IRepositoryFactory? _repositoryFactory;
        protected readonly ILogger _logger;

        protected GenericBusiness(IRepositoryFactory repositoryFactory, ILogger logger)
        {
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _repository = _repositoryFactory.CreateRepository<TEntity, TId>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // Constructor alternativo que mantiene compatibilidad con código existente
        protected GenericBusiness(IGenericRepository<TEntity, TId> repository, ILogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repositoryFactory = null;
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            try
            {
                var entities = await _repository.GetAllAsync();
                return MapToDtoList(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener todas las entidades {typeof(TEntity).Name}");
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la lista de {typeof(TEntity).Name}", ex);
            }
        }

        public virtual async Task<TDto> GetByIdAsync(TId id)
        {
            ValidateId(id);
            
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogInformation($"No se encontró ninguna entidad {typeof(TEntity).Name} con ID: {id}");
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);
                }

                return MapToDto(entity);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la entidad {typeof(TEntity).Name} con ID: {id}");
                throw new ExternalServiceException("Base de datos", $"Error al recuperar {typeof(TEntity).Name} con ID {id}", ex);
            }
        }

        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            ValidateDto(dto);
            
            try
            {
                var entity = MapToEntity(dto);
                SetAuditFieldsForCreate(entity);
                
                var createdEntity = await _repository.CreateAsync(entity);
                return MapToDto(createdEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear nueva entidad {typeof(TEntity).Name}");
                throw new ExternalServiceException("Base de datos", $"Error al crear la entidad {typeof(TEntity).Name}", ex);
            }
        }

        public virtual async Task<TDto> UpdateAsync(TId id, TDto dto)
        {
            ValidateId(id);
            ValidateDto(dto);

            try
            {
                var existingEntity = await _repository.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    _logger.LogInformation($"No se encontró la entidad {typeof(TEntity).Name} con ID {id} para actualizar");
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);
                }

                UpdateEntityFromDto(dto, existingEntity);
                SetAuditFieldsForUpdate(existingEntity);

                await _repository.UpdateAsync(existingEntity);
                return MapToDto(existingEntity);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la entidad {typeof(TEntity).Name} con ID {id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la entidad {typeof(TEntity).Name} con ID {id}", ex);
            }
        }

        public virtual async Task<TDto> PatchAsync(TId id, TDto dto)
        {
            ValidateId(id);
            // No validamos todo el DTO porque es un patch parcial

            try
            {
                var existingEntity = await _repository.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    _logger.LogInformation($"No se encontró la entidad {typeof(TEntity).Name} con ID {id} para aplicar patch");
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);
                }

                bool updated = PatchEntityFromDto(dto, existingEntity);
                
                if (updated)
                {
                    SetAuditFieldsForUpdate(existingEntity);
                    await _repository.UpdateAsync(existingEntity);
                    _logger.LogInformation($"Patch aplicado a la entidad {typeof(TEntity).Name} con ID: {id}");
                }
                else
                {
                    _logger.LogInformation($"No se realizaron cambios en la entidad {typeof(TEntity).Name} con ID {id} durante el patch");
                }

                return MapToDto(existingEntity);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al aplicar patch a la entidad {typeof(TEntity).Name} con ID {id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la entidad {typeof(TEntity).Name} con ID {id}", ex);
            }
        }

        public virtual async Task DeleteAsync(TId id)
        {
            ValidateId(id);
            
            try
            {
                var existingEntity = await _repository.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    _logger.LogInformation($"No se encontró la entidad {typeof(TEntity).Name} con ID {id} para eliminar");
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);
                }

                bool deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning($"No se pudo eliminar la entidad {typeof(TEntity).Name} con ID {id}");
                    throw new ExternalServiceException("Base de datos", $"Error al eliminar la entidad {typeof(TEntity).Name} con ID {id}");
                }
                
                _logger.LogInformation($"Entidad {typeof(TEntity).Name} con ID {id} eliminada exitosamente");
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la entidad {typeof(TEntity).Name} con ID {id}");
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la entidad {typeof(TEntity).Name} con ID {id}", ex);
            }
        }

        public virtual async Task SoftDeleteAsync(TId id)
        {
            ValidateId(id);
            
            try
            {
                var existingEntity = await _repository.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    _logger.LogInformation($"No se encontró la entidad {typeof(TEntity).Name} con ID {id} para desactivar");
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);
                }

                if (existingEntity is IActivable activable && !activable.Active)
                {
                    _logger.LogInformation($"La entidad {typeof(TEntity).Name} con ID {id} ya está inactiva");
                    return;
                }

                bool success = await _repository.SoftDeleteAsync(id);
                if (!success)
                {
                    _logger.LogWarning($"No se pudo desactivar la entidad {typeof(TEntity).Name} con ID {id}");
                    throw new ExternalServiceException("Base de datos", $"Error al desactivar la entidad {typeof(TEntity).Name} con ID {id}");
                }
                
                _logger.LogInformation($"Entidad {typeof(TEntity).Name} con ID {id} desactivada exitosamente");
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al desactivar la entidad {typeof(TEntity).Name} con ID {id}");
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la entidad {typeof(TEntity).Name} con ID {id}", ex);
            }
        }

        public virtual async Task ActivateAsync(TId id)
        {
            ValidateId(id);
            
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogInformation($"No se encontró la entidad {typeof(TEntity).Name} con ID {id} para activar");
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);
                }

                if (entity is IActivable activable)
                {
                    if (activable.Active)
                    {
                        _logger.LogInformation($"La entidad {typeof(TEntity).Name} con ID {id} ya está activa");
                        return;
                    }

                    activable.Active = true;
                    if (entity is IAuditable auditable)
                    {
                        auditable.UpdateDate = DateTime.UtcNow;
                    }
                    
                    await _repository.UpdateAsync(entity);
                    _logger.LogInformation($"Entidad {typeof(TEntity).Name} con ID {id} activada exitosamente");
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al activar la entidad {typeof(TEntity).Name} con ID {id}");
                throw new ExternalServiceException("Base de datos", $"Error al activar la entidad {typeof(TEntity).Name} con ID {id}", ex);
            }
        }

        // Métodos auxiliares para auditoría
        protected virtual void SetAuditFieldsForCreate(TEntity entity)
        {
            if (entity is IAuditable auditable)
            {
                auditable.CreateDate = DateTime.UtcNow;
            }
        }

        protected virtual void SetAuditFieldsForUpdate(TEntity entity)
        {
            if (entity is IAuditable auditable)
            {
                auditable.UpdateDate = DateTime.UtcNow;
            }
        }

        // Métodos de validación y mapeo a implementar en las clases derivadas
        protected abstract void ValidateId(TId id);
        protected abstract void ValidateDto(TDto dto);
        protected abstract TDto MapToDto(TEntity entity);
        protected abstract TEntity MapToEntity(TDto dto);
        protected abstract void UpdateEntityFromDto(TDto dto, TEntity entity);
        protected abstract bool PatchEntityFromDto(TDto dto, TEntity entity);
        protected abstract IEnumerable<TDto> MapToDtoList(IEnumerable<TEntity> entities);
    }
}
