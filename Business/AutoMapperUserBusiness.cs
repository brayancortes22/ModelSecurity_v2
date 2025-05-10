using Business.Base;
using Business.Interfaces;
using Business.Mappers;
using Data.Factory;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;
using System.Text;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Implementación de la lógica de negocio para User utilizando AutoMapper
    /// </summary>
    public class AutoMapperUserBusiness : AutoMapperGenericBusiness<User, UserDto, int>, IGenericBusiness<UserDto, int>
    {
        public AutoMapperUserBusiness(
            IRepositoryFactory repositoryFactory,
            ILogger<AutoMapperUserBusiness> logger,
            IMappingService mappingService)
            : base(repositoryFactory, logger, mappingService)
        {
        }

        /// <summary>
        /// Validación específica para ID de Usuario
        /// </summary>
        protected override void ValidateId(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó operar con un usuario con ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }
        }

        /// <summary>
        /// Validación específica para UserDto
        /// </summary>
        protected override void ValidateDto(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ValidationException("El objeto usuario no puede ser nulo");
            }

            // Determinar si estamos en un contexto de creación o actualización
            bool isCreate = userDto.Id == 0;

            if (string.IsNullOrWhiteSpace(userDto.Username))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Username vacío");
                throw new ValidationException("Username", "El Username es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(userDto.Email) || !IsValidEmail(userDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Email vacío o inválido");
                throw new ValidationException("Email", "El Email es obligatorio y debe tener un formato válido");
            }

            if (isCreate)
            {
                if (string.IsNullOrWhiteSpace(userDto.Password))
                {
                    _logger.LogWarning("Se intentó crear un usuario sin contraseña");
                    throw new ValidationException("Password", "La contraseña es obligatoria al crear un usuario");
                }
            }

            if (userDto.PersonId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con PersonId inválido: {PersonId}", userDto.PersonId);
                throw new ValidationException("PersonId", "El PersonId asociado al usuario es obligatorio");
            }
        }

        /// <summary>
        /// Implementación para actualizar parcialmente una entidad User
        /// </summary>
        protected override bool PatchEntityFromDto(UserDto userDto, User user)
        {
            bool updated = false;

            // Si el objeto solo contiene un ID y una contraseña, estamos en el caso de cambio de contraseña
            bool isPasswordChangeOnly = 
                !string.IsNullOrEmpty(userDto.Password) && 
                string.IsNullOrEmpty(userDto.Username) && 
                string.IsNullOrEmpty(userDto.Email) && 
                userDto.PersonId == 0;

            if (isPasswordChangeOnly)
            {
                // Actualizar solo la contraseña sin validar otros campos
                _logger.LogInformation("Aplicando PATCH solo para contraseña del usuario {UserId}", user.Id);
                user.Password = HashPassword(userDto.Password);
                return true;
            }

            // Actualizar Username si se proporciona y es diferente
            if (!string.IsNullOrEmpty(userDto.Username) && user.Username != userDto.Username)
            {
                if (string.IsNullOrWhiteSpace(userDto.Username))
                    throw new ValidationException("Username", "El Username no puede estar vacío en PATCH");
                user.Username = userDto.Username;
                updated = true;
            }

            // Actualizar Email si se proporciona y es diferente
            if (!string.IsNullOrEmpty(userDto.Email) && user.Email != userDto.Email)
            {
                if (string.IsNullOrWhiteSpace(userDto.Email) || !IsValidEmail(userDto.Email))
                    throw new ValidationException("Email", "El Email proporcionado no es válido en PATCH");
                user.Email = userDto.Email;
                updated = true;
            }

            // Actualizar PersonId si se proporciona y es diferente
            if (userDto.PersonId > 0 && user.PersonId != userDto.PersonId)
            {
                user.PersonId = userDto.PersonId;
                updated = true;
            }

            // Actualizar contraseña si se proporciona
            if (!string.IsNullOrWhiteSpace(userDto.Password))
            {
                user.Password = HashPassword(userDto.Password);
                updated = true;
                _logger.LogInformation("Contraseña actualizada para el usuario con ID {UserId}", user.Id);
            }

            return updated;
        }

        /// <summary>
        /// Método para encriptar la contraseña
        /// </summary>
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Validación simple de formato de email
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
