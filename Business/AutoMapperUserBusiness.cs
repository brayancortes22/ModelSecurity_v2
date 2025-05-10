using Business.Base;
using Business.Interfaces;
using Business.Mappers;
using Data.Factory;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

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
        /// Validación específica para User antes de guardar
        /// </summary>
        protected override void ValidateBeforeSave(UserDto dto)
        {
            base.ValidateBeforeSave(dto);

            // Validaciones adicionales específicas para User
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                throw new Utilities.exeptions.BusinessExceptio("El nombre de usuario es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new Utilities.exeptions.BusinessExceptio("El email del usuario es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new Utilities.exeptions.BusinessExceptio("La contraseña es obligatoria");
            }

            if (dto.PersonId <= 0)
            {
                throw new Utilities.exeptions.BusinessExceptio("Se debe asociar una persona al usuario");
            }
        }

        /// <summary>
        /// Sobrescribimos el método para encriptar la contraseña antes de guardar
        /// </summary>
        public override int Save(UserDto dto)
        {
            // Encriptar contraseña si no está vacía
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                dto.Password = HashPassword(dto.Password);
            }

            // Continuar con el proceso de guardado
            return base.Save(dto);
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
    }
}
