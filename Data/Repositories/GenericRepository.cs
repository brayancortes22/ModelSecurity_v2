using Data.Interfaces;
using Entity.Contexts;
using Entity.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories
{
    /// <summary>
    /// Implementación base de un repositorio genérico para operaciones CRUD
    /// </summary>
    public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> 
        where TEntity : class, IEntity
        where TId : IConvertible
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger<GenericRepository<TEntity, TId>> _logger;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext context, ILogger<GenericRepository<TEntity, TId>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            try
            {
                return await _dbSet.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener todas las entidades de tipo {typeof(TEntity).Name}");
                throw;
            }
        }

        public virtual async Task<TEntity> GetByIdAsync(TId id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la entidad {typeof(TEntity).Name} con ID {id}");
                throw;
            }
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear la entidad {typeof(TEntity).Name}");
                throw;
            }
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la entidad {typeof(TEntity).Name}");
                throw;
            }
        }

        public virtual async Task<bool> DeleteAsync(TId id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null) return false;
                
                _dbSet.Remove(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la entidad {typeof(TEntity).Name} con ID {id}");
                throw;
            }
        }

        public virtual async Task<bool> SoftDeleteAsync(TId id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null) return false;
                
                if (entity is IActivable activable)
                {
                    activable.Active = false;
                    if (entity is IAuditable auditable)
                    {
                        auditable.DeleteDate = DateTime.UtcNow;
                    }
                    return await UpdateAsync(entity);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al desactivar la entidad {typeof(TEntity).Name} con ID {id}");
                throw;
            }
        }

        public virtual async Task<bool> PatchAsync(TId id, TEntity entity)
        {
            try
            {
                var existingEntity = await GetByIdAsync(id);
                if (existingEntity == null) return false;

                // Aplicar solo los campos que no son nulos desde entity a existingEntity
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al aplicar patch a la entidad {typeof(TEntity).Name} con ID {id}");
                throw;
            }
        }

        public virtual async Task<bool> ActivateAsync(TId id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null) return false;

                // Asumiendo que IEntity tiene una propiedad Active o similar
                if (entity is IActivable activatableEntity)
                {
                    activatableEntity.Active = true;
                    _context.Entry(entity).State = EntityState.Modified;
                    return await _context.SaveChangesAsync() > 0;
                }
                
                _logger.LogWarning($"La entidad {typeof(TEntity).Name} no implementa IActivatable, no se puede activar.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al activar la entidad {typeof(TEntity).Name} con ID {id}");
                throw;
            }
        }
    }
}