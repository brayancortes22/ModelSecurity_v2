using Business.Interfaces;
using Data;
using Data.Interfaces;
using Entity.DTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore; // Para DbUpdateException
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions; // Para validar Email
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las personas en el sistema.
    /// </summary>
    public class PersonBusiness
    {
        private readonly PersonData _personData;
        private readonly ILogger<PersonBusiness> _logger;

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

        public PersonBusiness(PersonData personData, ILogger<PersonBusiness> logger)
        {
            _personData = personData;
            _logger = logger;
        }

        // Método para obtener todas las personas como DTOs
        public async Task<IEnumerable<PersonDto>> GetAllPersonsAsync()
        {
            try
            {
                var persons = await _personData.GetAllAsync();
                return MapToDTOList(persons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las personas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de personas", ex);
            }
        }

        // Método para obtener una persona por ID como DTO
        public async Task<PersonDto> GetPersonByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una persona con ID inválido: {PersonId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la persona debe ser mayor que cero");
            }

            try
            {
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ninguna persona con ID: {PersonId}", id);
                    throw new EntityNotFoundException("Person", id);
                }

                return MapToDTO(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la persona con ID {id}", ex);
            }
        }

        // Método para crear una persona desde un DTO
        public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
        {
            try
            {
                ValidatePerson(personDto);
                var person = MapToEntity(personDto);
                person.CreateDate = DateTime.UtcNow;
                person.Active = true;
                var personCreada = await _personData.CreateAsync(person);
                return MapToDTO(personCreada);
            }
            catch (DbUpdateException dbEx) // Captura posible UNIQUE constraint (Email, NumberIdentification?)
            {
                _logger.LogError(dbEx, "Error de base de datos al crear persona. Posible Email/Número Identificación duplicado.");
                throw new ExternalServiceException("Base de datos", "Error al crear la persona. Verifique Email/Número Identificación.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al crear nueva persona: {Name}", personDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la persona", ex);
            }
        }

        // Método para actualizar una persona existente (PUT)
        public async Task<PersonDto> UpdatePersonAsync(int id, PersonDto personDto)
        {
            if (id <= 0 || id != personDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar una persona con un ID inválido o no coincidente: {PersonId}, DTO ID: {DtoId}", id, personDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidatePerson(personDto); // Validar datos completos

            try
            {
                var existingPerson = await _personData.GetByIdAsync(id);
                if (existingPerson == null)
                {
                    _logger.LogInformation("No se encontró la persona con ID {PersonId} para actualizar", id);
                    throw new EntityNotFoundException("Person", id);
                }

                // Mapea los cambios del DTO a la entidad existente
                existingPerson = MapToEntity(personDto, existingPerson);
                existingPerson.UpdateDate = DateTime.UtcNow; // Actualizar fecha modificación

                await _personData.UpdateAsync(existingPerson); // Asume que UpdateAsync existe
                return MapToDTO(existingPerson);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al actualizar persona {PersonId}. Posible Email/Número Identificación duplicado.", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la persona con ID {id}. Verifique Email/Número Identificación.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar persona {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la persona con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente una persona (PATCH)
        public async Task<PersonDto> PatchPersonAsync(int id, PersonDto personDto)
        {
            if (id <= 0 || (personDto.Id != 0 && id != personDto.Id))
            {
                _logger.LogWarning("Se intentó aplicar patch a una persona con un ID inválido o no coincidente: {PersonId}, DTO ID: {DtoId}", id, personDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado en la URL es inválido o no coincide con el ID del DTO (si se proporcionó) para PATCH.");
            }

            try
            {
                var existingPerson = await _personData.GetByIdAsync(id);
                if (existingPerson == null)
                {
                    _logger.LogInformation("No se encontró la persona con ID {PersonId} para aplicar patch", id);
                    throw new EntityNotFoundException("Person", id);
                }

                bool changed = false;

                // Actualizar campos si se proporcionan y son diferentes
                if (personDto.Name != null && existingPerson.Name != personDto.Name)
                {
                    if (string.IsNullOrWhiteSpace(personDto.Name)) throw new Utilities.Exceptions.ValidationException("Name", "El Name no puede estar vacío en PATCH.");
                    existingPerson.Name = personDto.Name; changed = true;
                }
                if (personDto.FirstName != null && existingPerson.FirstName != personDto.FirstName)
                {
                    if (string.IsNullOrWhiteSpace(personDto.FirstName)) throw new Utilities.Exceptions.ValidationException("FirstName", "El FirstName no puede estar vacío en PATCH.");
                    existingPerson.FirstName = personDto.FirstName; changed = true;
                }
                if (personDto.SecondName != null && existingPerson.SecondName != personDto.SecondName)
                {
                    existingPerson.SecondName = personDto.SecondName; changed = true; // Permitir vacío
                }
                if (personDto.FirstLastName != null && existingPerson.FirstLastName != personDto.FirstLastName)
                {
                    if (string.IsNullOrWhiteSpace(personDto.FirstLastName)) throw new Utilities.Exceptions.ValidationException("FirstLastName", "El FirstLastName no puede estar vacío en PATCH.");
                    existingPerson.FirstLastName = personDto.FirstLastName; changed = true;
                }
                if (personDto.SecondLastName != null && existingPerson.SecondLastName != personDto.SecondLastName)
                {
                    existingPerson.SecondLastName = personDto.SecondLastName; changed = true; // Permitir vacío
                }
                if (personDto.PhoneNumber != null && existingPerson.PhoneNumber != personDto.PhoneNumber)
                {
                    existingPerson.PhoneNumber = personDto.PhoneNumber; changed = true; // Podría validar formato
                }
                if (personDto.Email != null && existingPerson.Email != personDto.Email)
                {
                    if (!IsValidEmail(personDto.Email)) throw new Utilities.Exceptions.ValidationException("Email", "El Email proporcionado no es válido en PATCH.");
                    existingPerson.Email = personDto.Email; changed = true;
                }
                if (personDto.TypeIdentification != null && existingPerson.TypeIdentification != personDto.TypeIdentification)
                {
                    if (string.IsNullOrWhiteSpace(personDto.TypeIdentification)) throw new Utilities.Exceptions.ValidationException("TypeIdentification", "El TypeIdentification no puede estar vacío en PATCH.");
                    existingPerson.TypeIdentification = personDto.TypeIdentification; changed = true;
                }
                if (personDto.NumberIdentification != default(int) && existingPerson.NumberIdentification != personDto.NumberIdentification)
                {
                    if (personDto.NumberIdentification <= 0) throw new Utilities.Exceptions.ValidationException("NumberIdentification", "El NumberIdentification debe ser positivo en PATCH.");
                    existingPerson.NumberIdentification = personDto.NumberIdentification; changed = true;
                }
                if (existingPerson.Signing != personDto.Signing)
                {
                    existingPerson.Signing = personDto.Signing; changed = true;
                }
                

                if (changed)
                {
                    existingPerson.UpdateDate = DateTime.UtcNow;
                    await _personData.UpdateAsync(existingPerson);
                    _logger.LogInformation("Aplicado patch a la persona con ID {PersonId}", id);
                }
                else
                {
                    _logger.LogInformation("No se detectaron cambios en la solicitud PATCH para la persona con ID {PersonId}", id);
                }

                return MapToDTO(existingPerson);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al aplicar patch a persona {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch a la persona con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch a persona {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch a la persona con ID {id}", ex);
            }
        }

        // Método para eliminar una persona (DELETE persistente)
        // ADVERTENCIA: Puede fallar si existe un User asociado.
        public async Task DeletePersonAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una persona con un ID inválido: {PersonId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la persona debe ser mayor a 0");
            }
            try
            {
                var existingPerson = await _personData.GetByIdAsync(id);
                if (existingPerson == null)
                {
                    _logger.LogInformation("No se encontró la persona con ID {PersonId} para eliminar (persistente)", id);
                    throw new EntityNotFoundException("Person", id);
                }

                // ADVERTENCIA: Fallará si existe un User asociado.
                bool deleted = await _personData.DeleteAsync(id);
                
                if (deleted)
                {
                    _logger.LogInformation("Persona con ID {PersonId} eliminada exitosamente (persistente)", id);
                }
                else
                {
                    _logger.LogWarning("No se pudo eliminar (persistente) la persona con ID {PersonId}.", id);
                    throw new EntityNotFoundException("Person", id);
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al eliminar la persona {PersonId}. Posible FK con User.", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la persona con ID {id}. Verifique si tiene un usuario asociado.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al eliminar (persistente) la persona {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la persona con ID {id}", ex);
            }
        }

        // Método para eliminar lógicamente una persona (soft delete)
        public async Task SoftDeletePersonAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar soft-delete a una persona con un ID inválido: {PersonId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la persona debe ser mayor a 0");
            }
            try
            {
                var personToDeactivate = await _personData.GetByIdAsync(id);
                if (personToDeactivate == null)
                {
                    _logger.LogInformation("No se encontró la persona con ID {PersonId} para desactivar (soft-delete)", id);
                    throw new EntityNotFoundException("Person", id);
                }

                if (!personToDeactivate.Active)
                {
                    _logger.LogInformation("La persona con ID {PersonId} ya está inactiva.", id);
                    return;
                }

                personToDeactivate.Active = false;
                personToDeactivate.DeleteDate = DateTime.UtcNow;
                
                await _personData.UpdateAsync(personToDeactivate);

                _logger.LogInformation("Persona con ID {PersonId} marcada como inactiva (soft-delete)", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete de la persona {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la persona con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al realizar soft-delete de la persona {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la persona con ID {id}", ex);
            }
        }

        // Método para activar un módulo (restaurar)
        public async Task ActivatePersonAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó activar un usuario con un ID invalido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor a 0");
            }
            try
            {
                var personToActivate = await _personData.GetByIdAsync(id);
                if (personToActivate == null)
                {
                    _logger.LogInformation("No se encontró la persona con ID {PersonId} para activar", id);
                    throw new EntityNotFoundException("Person", id);
                }

                if (personToActivate.Active)
                {
                    _logger.LogInformation("La persona con ID {PersonId} ya está activa.", id);
                    return;
                }

                personToActivate.Active = true;
                // Considerar limpiar DeleteDate y actualizar UpdateDate si existen
                // personToActivate.DeleteDate = null;
                // personToActivate.UpdateDate = DateTime.UtcNow;
                await _personData.UpdateAsync(personToActivate);

                _logger.LogInformation("Usuario con ID {UserId} marcado como activo.", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al activar formulario {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al activar el formulario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al activar formulario {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al activar el formulario con ID {id}", ex);
            }
        }

        // Método para validar el DTO (modificado)
        private void ValidatePerson(PersonDto personDto)
        {
            if (personDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto persona no puede ser nulo");
            }

            // Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(personDto.Name))
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la persona es obligatorio");
            if (string.IsNullOrWhiteSpace(personDto.FirstName))
                throw new Utilities.Exceptions.ValidationException("FirstName", "El FirstName (Primer Nombre) es obligatorio");
            if (string.IsNullOrWhiteSpace(personDto.FirstLastName))
                throw new Utilities.Exceptions.ValidationException("FirstLastName", "El FirstLastName (Primer Apellido) es obligatorio");
            if (string.IsNullOrWhiteSpace(personDto.Email) || !IsValidEmail(personDto.Email))
                throw new Utilities.Exceptions.ValidationException("Email", "El Email es obligatorio y debe ser válido");
            if (string.IsNullOrWhiteSpace(personDto.TypeIdentification))
                throw new Utilities.Exceptions.ValidationException("TypeIdentification", "El TypeIdentification es obligatorio");
            if (personDto.NumberIdentification <= 0) // Asumiendo que debe ser positivo
                throw new Utilities.Exceptions.ValidationException("NumberIdentification", "El NumberIdentification debe ser un número positivo");

             // Podrían añadirse más validaciones (longitud, formato de teléfono, etc.)
             _logger.LogDebug("Validación de Persona DTO exitosa para: {NumberIdentification}", personDto.NumberIdentification);
        }

        //Funciones de mapeos 
        // Método para mapear de Person a PersonDto
        private PersonDto MapToDTO(Person person)
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
                
            };
        }

        // Método para mapear de PersonDto a Person (para creación)
        private Person MapToEntity(PersonDto personDto)
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
                
            };
        }

        // Método para mapear de DTO a una entidad existente (para actualización PUT)
        private Person MapToEntity(PersonDto personDto, Person existingPerson)
        {
            existingPerson.Name = personDto.Name;
            existingPerson.FirstName = personDto.FirstName;
            existingPerson.SecondName = personDto.SecondName;
            existingPerson.FirstLastName = personDto.FirstLastName;
            existingPerson.SecondLastName = personDto.SecondLastName;
            existingPerson.PhoneNumber = personDto.PhoneNumber;
            existingPerson.Email = personDto.Email;
            existingPerson.TypeIdentification = personDto.TypeIdentification;
            existingPerson.NumberIdentification = personDto.NumberIdentification;
            existingPerson.Signing = personDto.Signing;
            

            // Actualizar DeleteDate basado en Active
            if (existingPerson.Active && existingPerson.DeleteDate.HasValue)
            {
                existingPerson.DeleteDate = null;
            }
            else if (!existingPerson.Active && !existingPerson.DeleteDate.HasValue)
            {
                existingPerson.DeleteDate = DateTime.UtcNow; // O dejar que SoftDelete lo maneje
            }

            return existingPerson;
        }

        // Método para mapear una lista de Person a una lista de PersonDto
        private IEnumerable<PersonDto> MapToDTOList(IEnumerable<Person> persons)
        {
            return persons.Select(MapToDTO).ToList();
        }
    }
}
