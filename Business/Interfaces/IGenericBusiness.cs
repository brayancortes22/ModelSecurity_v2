using Data.Interfaces;
using Entity.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business.Interfaces
{
    /// <summary>
    /// Interfaz base para servicios de negocio
    /// </summary>
    public interface IGenericBusiness<TDto, TId>
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(TId id);
        Task<TDto> CreateAsync(TDto dto);
        Task<TDto> UpdateAsync(TId id, TDto dto);
        Task<TDto> PatchAsync(TId id, TDto dto);
        Task DeleteAsync(TId id);
        Task SoftDeleteAsync(TId id);
        Task ActivateAsync(TId id);
    }
}