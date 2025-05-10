using Business.Base;
using Business.Interfaces;
using Data.Factory;
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
    /// Clase de negocio encargada de la lógica relacionada con las personas en el sistema.
    /// </summary>
    public class PersonBusiness : GenericBusiness<Person, PersonDto, int>, IGenericBusiness<PersonDto, int>
    {
        // Helper para validar email (simple)
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

        public PersonBusiness(IRepositoryFactory repositoryFactory, ILogger<PersonBusiness> logger)
            : base(repositoryFactory, logger)
        {
        }
        
        public PersonBusiness(IGenericRepository<Person, int> repository, ILogger<PersonBusiness> logger)
            : base(repository, logger)
        {
        }

        // Implementaciones específicas de los métodos abstractos
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con una persona con ID inválido: {PersonId}", id);
                throw new ValidationException("id", "El ID de la persona debe ser mayor que cero");
            }
        }

        protected override void ValidateDto(PersonDto personDto)
        {
            if (personDto == null)
            {
                throw new ValidationException("El objeto persona no puede ser nulo");
            }

            // Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(personDto.Name))
                throw new ValidationException("Name", "El Name de la persona es obligatorio");
            if (string.IsNullOrWhiteSpace(personDto.FirstName))
                throw new ValidationException("FirstName", "El FirstName (Primer Nombre) es obligatorio");
            if (string.IsNullOrWhiteSpace(personDto.FirstLastName))
                throw new ValidationException("FirstLastName", "El FirstLastName (Primer Apellido) es obligatorio");
            if (string.IsNullOrWhiteSpace(personDto.Email) || !IsValidEmail(personDto.Email))
                throw new ValidationException("Email", "El Email es obligatorio y debe ser válido");
            if (string.IsNullOrWhiteSpace(personDto.TypeIdentification))
                throw new ValidationException("TypeIdentification", "El TypeIdentification es obligatorio");
            if (personDto.NumberIdentification <= 0)
                throw new ValidationException("NumberIdentification", "El NumberIdentification debe ser un número positivo");
        }

        protected override PersonDto MapToDto(Person person)
        {
            return new PersonDto
            {
                Id = person.Id,
                Name = person.Name,
                FirstName = person.FirstName,
                SecondName = person.SecondName,
                FirstLastName = person.FirstLastName,
                SecondLastName = person.SecondLastName,
                PhoneNumber = person.PhoneNumber,
                Email = person.Email,
                TypeIdentification = person.TypeIdentification,
                NumberIdentification = person.NumberIdentification,
                Signing = person.Signing,
                Active = person.Active
            };
        }

        protected override Person MapToEntity(PersonDto personDto)
        {
            return new Person
            {
                Id = personDto.Id,
                Name = personDto.Name,
                FirstName = personDto.FirstName,
                SecondName = personDto.SecondName,
                FirstLastName = personDto.FirstLastName,
                SecondLastName = personDto.SecondLastName,
                PhoneNumber = personDto.PhoneNumber,
                Email = personDto.Email,
                TypeIdentification = personDto.TypeIdentification,
                NumberIdentification = personDto.NumberIdentification,
                Signing = personDto.Signing,
                Active = personDto.Active
            };
        }

        protected override void UpdateEntityFromDto(PersonDto personDto, Person person)
        {
            person.Name = personDto.Name;
            person.FirstName = personDto.FirstName;
            person.SecondName = personDto.SecondName;
            person.FirstLastName = personDto.FirstLastName;
            person.SecondLastName = personDto.SecondLastName;
            person.PhoneNumber = personDto.PhoneNumber;
            person.Email = personDto.Email;
            person.TypeIdentification = personDto.TypeIdentification;
            person.NumberIdentification = personDto.NumberIdentification;
            person.Signing = personDto.Signing;
            person.Active = personDto.Active;
        }

        protected override bool PatchEntityFromDto(PersonDto personDto, Person person)
        {
            bool updated = false;

            if (!string.IsNullOrWhiteSpace(personDto.Name) && personDto.Name != person.Name)
            {
                person.Name = personDto.Name;
                updated = true;
            }
            
            if (!string.IsNullOrWhiteSpace(personDto.FirstName) && personDto.FirstName != person.FirstName)
            {
                person.FirstName = personDto.FirstName;
                updated = true;
            }
            
            if (personDto.SecondName != null && personDto.SecondName != person.SecondName)
            {
                person.SecondName = personDto.SecondName;
                updated = true;
            }
            
            if (!string.IsNullOrWhiteSpace(personDto.FirstLastName) && personDto.FirstLastName != person.FirstLastName)
            {
                person.FirstLastName = personDto.FirstLastName;
                updated = true;
            }
            
            if (personDto.SecondLastName != null && personDto.SecondLastName != person.SecondLastName)
            {
                person.SecondLastName = personDto.SecondLastName;
                updated = true;
            }
            
            if (personDto.PhoneNumber != null && personDto.PhoneNumber != person.PhoneNumber)
            {
                person.PhoneNumber = personDto.PhoneNumber;
                updated = true;
            }
            
            if (!string.IsNullOrWhiteSpace(personDto.Email) && personDto.Email != person.Email)
            {
                if (!IsValidEmail(personDto.Email))
                    throw new ValidationException("Email", "El Email proporcionado no es válido");
                person.Email = personDto.Email;
                updated = true;
            }
            
            if (!string.IsNullOrWhiteSpace(personDto.TypeIdentification) && personDto.TypeIdentification != person.TypeIdentification)
            {
                person.TypeIdentification = personDto.TypeIdentification;
                updated = true;
            }
            
            if (personDto.NumberIdentification > 0 && personDto.NumberIdentification != person.NumberIdentification)
            {
                person.NumberIdentification = personDto.NumberIdentification;
                updated = true;
            }
            
            if (personDto.Signing != null && personDto.Signing != person.Signing)
            {
                person.Signing = personDto.Signing;
                updated = true;
            }

            return updated;
        }

        protected override IEnumerable<PersonDto> MapToDtoList(IEnumerable<Person> persons)
        {
            return persons.Select(MapToDto).ToList();
        }
    }
}
