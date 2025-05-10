using AutoMapper;
using Business.Base;
using Business.Interfaces;
using Business.Mappers;
using Data.Factory;
using Data.Interfaces;
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
    /// Ejemplo de implementación de un servicio de negocio que utiliza AutoMapper
    /// </summary>
    public class AutoMapperFormBusiness : AutoMapperGenericBusiness<Form, FormDto, int>, IGenericBusiness<FormDto, int>
    {
        public AutoMapperFormBusiness(
            IRepositoryFactory repositoryFactory, 
            ILogger<AutoMapperFormBusiness> logger,
            IMappingService mappingService)
            : base(repositoryFactory, logger, mappingService)
        {
        }

        // Implementaciones específicas de los métodos abstractos
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con un formulario con ID inválido: {FormId}", id);
                throw new ValidationException("id", "El ID del formulario debe ser mayor que cero");
            }
        }

        protected override void ValidateDto(FormDto formDto)
        {
            if (formDto == null)
            {
                throw new ValidationException("El objeto formulario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(formDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un formulario con Name vacío");
                throw new ValidationException("Name", "El Name del formulario es obligatorio");
            }
        }

        protected override bool PatchEntityFromDto(FormDto formDto, Form form)
        {
            bool updated = false;

            if (!string.IsNullOrWhiteSpace(formDto.Name) && formDto.Name != form.Name)
            {
                form.Name = formDto.Name;
                updated = true;
            }
            
            if (formDto.Description != null && formDto.Description != form.Description)
            {
                form.Description = formDto.Description;
                updated = true;
            }
            
            if (formDto.Cuestion != null && formDto.Cuestion != form.Cuestion)
            {
                form.Cuestion = formDto.Cuestion;
                updated = true;
            }
            
            if (formDto.TypeCuestion != null && formDto.TypeCuestion != form.TypeCuestion)
            {
                form.TypeCuestion = formDto.TypeCuestion;
                updated = true;
            }
            
            if (formDto.Answer != null && formDto.Answer != form.Answer)
            {
                form.Answer = formDto.Answer;
                updated = true;
            }
            
            if (formDto.Route != form.Route)
            {
                form.Route = formDto.Route;
                updated = true;
            }

            return updated;
        }
    }
}
