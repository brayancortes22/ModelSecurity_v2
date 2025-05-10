using Business.Base;
using Business.Interfaces;
using Business.Mappers;
using Data.Factory;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Business
{
    /// <summary>
    /// Implementación de la lógica de negocio para Module utilizando AutoMapper
    /// </summary>
    public class AutoMapperModuleBusiness : AutoMapperGenericBusiness<Module, ModuleDto, int>, IGenericBusiness<ModuleDto, int>
    {
        public AutoMapperModuleBusiness(
            IRepositoryFactory repositoryFactory,
            ILogger<AutoMapperModuleBusiness> logger,
            IMappingService mappingService)
            : base(repositoryFactory, logger, mappingService)
        {
        }

        /// <summary>
        /// Validación específica para Module antes de guardar
        /// </summary>
        protected override void ValidateBeforeSave(ModuleDto dto)
        {
            base.ValidateBeforeSave(dto);

            // Validaciones adicionales específicas para Module
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new Utilities.exeptions.BusinessExceptio("El nombre del módulo es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(dto.Description))
            {
                throw new Utilities.exeptions.BusinessExceptio("La descripción del módulo es obligatoria");
            }
        }
    }
}
