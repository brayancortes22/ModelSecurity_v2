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
    /// Clase de negocio encargada de la lógica relacionada con los formularios en el sistema.
    /// </summary>
    public class FormBusiness : GenericBusiness<Form, FormDto, int>, IGenericBusiness<FormDto, int>
    {
        public FormBusiness(IGenericRepository<Form, int> repository, ILogger<FormBusiness> logger)
            : base(repository, logger)
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

        protected override FormDto MapToDto(Form form)
        {
            return new FormDto
            {
                Id = form.Id,
                Name = form.Name,
                Description = form.Description,
                Cuestion = form.Cuestion,
                TypeCuestion = form.TypeCuestion,
                Answer = form.Answer,
                Route = form.Route,
                Active = form.Active
            };
        }

        protected override Form MapToEntity(FormDto formDto)
        {
            return new Form
            {
                Id = formDto.Id,
                Name = formDto.Name,
                Description = formDto.Description,
                Cuestion = formDto.Cuestion,
                TypeCuestion = formDto.TypeCuestion,
                Answer = formDto.Answer,
                Route = formDto.Route,
                Active = formDto.Active
            };
        }

        protected override void UpdateEntityFromDto(FormDto formDto, Form form)
        {
            form.Name = formDto.Name;
            form.Description = formDto.Description;
            form.Cuestion = formDto.Cuestion;
            form.TypeCuestion = formDto.TypeCuestion;
            form.Answer = formDto.Answer;
            form.Route = formDto.Route;
            form.Active = formDto.Active;
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

        protected override IEnumerable<FormDto> MapToDtoList(IEnumerable<Form> forms)
        {
            return forms.Select(MapToDto).ToList();
        }
    }
}
