using Business.Base;
using Business.Interfaces;
using Business.Mappers;
using Data.Factory;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Implementación de la lógica de negocio para Person utilizando AutoMapper
    /// </summary>
    public class AutoMapperPersonBusiness : AutoMapperGenericBusiness<Person, PersonDto, int>, IGenericBusiness<PersonDto, int>
    {
        public AutoMapperPersonBusiness(
            IRepositoryFactory repositoryFactory,
            ILogger<AutoMapperPersonBusiness> logger,
            IMappingService mappingService)
            : base(repositoryFactory, logger, mappingService)
        {
        }

        /// <summary>
        /// Validación específica para ID de Persona
        /// </summary>
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con una persona con ID inválido: {PersonId}", id);
                throw new ValidationException("id", "El ID de la persona debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Validación específica para PersonDto
        /// </summary>
        protected override void ValidateDto(PersonDto personDto)
        {
            if (personDto == null)
            {
                throw new ValidationException("El objeto persona no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(personDto.FirstName))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con FirstName vacío");
                throw new ValidationException("FirstName", "El nombre de la persona es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(personDto.FirstLastName))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con FirstLastName vacío");
                throw new ValidationException("FirstLastName", "El apellido de la persona es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(personDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con Email vacío");
                throw new ValidationException("Email", "El email de la persona es obligatorio");
            }

            // Validar formato de email
            if (!IsValidEmail(personDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con un formato de email inválido");
                throw new ValidationException("Email", "El formato del email no es válido");
            }
        }

        /// <summary>
        /// Implementación para actualizar parcialmente una entidad Person
        /// </summary>
        protected override bool PatchEntityFromDto(PersonDto personDto, Person person)
        {
            bool updated = false;

            // Actualizamos solo los campos no nulos proporcionados en el DTO
            if (!string.IsNullOrWhiteSpace(personDto.FirstName) && personDto.FirstName != person.FirstName)
            {
                person.FirstName = personDto.FirstName;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(personDto.SecondName) && personDto.SecondName != person.SecondName)
            {
                person.SecondName = personDto.SecondName;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(personDto.FirstLastName) && personDto.FirstLastName != person.FirstLastName)
            {
                person.FirstLastName = personDto.FirstLastName;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(personDto.SecondLastName) && personDto.SecondLastName != person.SecondLastName)
            {
                person.SecondLastName = personDto.SecondLastName;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(personDto.Email) && personDto.Email != person.Email)
            {
                if (!IsValidEmail(personDto.Email))
                {
                    throw new ValidationException("Email", "El formato del email proporcionado no es válido");
                }
                person.Email = personDto.Email;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(personDto.PhoneNumber) && personDto.PhoneNumber != person.PhoneNumber)
            {
                person.PhoneNumber = personDto.PhoneNumber;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(personDto.Name) && personDto.Name != person.Name)
            {
                person.Name = personDto.Name;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(personDto.TypeIdentification) && personDto.TypeIdentification != person.TypeIdentification)
            {
                person.TypeIdentification = personDto.TypeIdentification;
                updated = true;
            }

            if (personDto.NumberIdentification != 0 && personDto.NumberIdentification != person.NumberIdentification)
            {
                person.NumberIdentification = personDto.NumberIdentification;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(personDto.Signing) && personDto.Signing != person.Signing)
            {
                person.Signing = personDto.Signing;
                updated = true;
            }

            return updated;
        }

        /// <summary>
        /// Valida el formato de un email
        /// </summary>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
