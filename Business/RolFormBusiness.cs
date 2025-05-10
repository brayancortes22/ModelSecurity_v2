using Business.Base;
using Business.Factory;
using Business.Interfaces;
using Business.Mappers;
using Data;
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
    /// Clase de negocio encargada de la lógica relacionada con los roles de formulario en el sistema.
    /// </summary>
    public class RolFormBusiness : IGenericBusiness<RolFormDto, int>
    {
        private readonly IRolFormRepository _rolFormRepository;
        private readonly IGenericRepository<RolForm, int> _repository;
        private readonly ILogger<RolFormBusiness> _logger;
        private readonly IRepositoryFactory? _repositoryFactory;
        private readonly IMappingService _mappingService;

        /// <summary>
        /// Constructor con inyección de RepositoryFactory
        /// </summary>
        public RolFormBusiness(
            IRepositoryFactory repositoryFactory,
            ILogger<RolFormBusiness> logger,
            IMappingService mappingService)
        {
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _repository = _repositoryFactory.CreateSpecificRepository<IGenericRepository<RolForm, int>>();
            _rolFormRepository = _repositoryFactory.CreateSpecificRepository<IRolFormRepository>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
        }        /// <summary>
        /// Constructor tradicional para compatibilidad
        /// </summary>
        public RolFormBusiness(
            IRolFormRepository rolFormRepository,
            IGenericRepository<RolForm, int> repository,
            ILogger<RolFormBusiness> logger,
            IMappingService mappingService)
        {
            _rolFormRepository = rolFormRepository ?? throw new ArgumentNullException(nameof(rolFormRepository));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repositoryFactory = null;
            _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
        }

        // Método para obtener todos los roles de formulario como DTOs
        public async Task<IEnumerable<RolFormDto>> GetAllAsync()
        {
            try
            {
                var rolForms = await _repository.GetAllAsync();
                return MapToDTOList(rolForms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles de formulario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles de formulario", ex);
            }
        }

        // Método para obtener un rol de formulario por ID como DTO
        public async Task<RolFormDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol de formulario con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol de formulario debe ser mayor que cero");
            }

            try
            {
                var rolForm = await _repository.GetByIdAsync(id);
                if (rolForm == null)
                {
                    _logger.LogInformation("No se encontró ningún rol de formulario con ID: {Id}", id);
                    throw new EntityNotFoundException("rolForm", id);
                }

                return MapToDTO(rolForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol de formulario con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol de formulario con ID {id}", ex);
            }
        }

        // Método para crear un rol de formulario desde un DTO
        public async Task<RolFormDto> CreateAsync(RolFormDto rolFormDto)
        {
            try
            {
                // Si no se proporciona un permiso, establecemos uno predeterminado
                if (string.IsNullOrWhiteSpace(rolFormDto.Permission))
                {
                    rolFormDto.Permission = "READ"; // Valor predeterminado
                    _logger.LogInformation("Se estableció el permiso predeterminado 'READ' para la nueva asignación rol-formulario");
                }
                
                ValidateRolForm(rolFormDto);
                var rolForm = MapToEntity(rolFormDto);
                var rolFormCreado = await _repository.CreateAsync(rolForm);
                return MapToDTO(rolFormCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol de formulario: {Name}", rolFormDto?.Permission ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol de formulario", ex);
            }
        }

        // Método para actualizar una relación rol-formulario existente (reemplazo completo)
        public async Task<RolFormDto> UpdateAsync(int id, RolFormDto rolFormDto)
        {
            if (id <= 0 || id != rolFormDto.Id)
            {
                _logger.LogWarning("Se intentó actualizar una relación rol-formulario con ID inválido o no coincidente: {RolFormId}, DTO ID: {DtoId}", id, rolFormDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
            ValidateRolForm(rolFormDto); // Reutilizamos la validación

            try
            {
                var existingRolForm = await _repository.GetByIdAsync(id);
                if (existingRolForm == null)
                {
                    _logger.LogInformation("No se encontró la relación rol-formulario con ID {RolFormId} para actualizar", id);
                    throw new EntityNotFoundException("RolForm", id);
                }

                // Mapear el DTO a la entidad existente (actualización completa)
                existingRolForm.RolId = rolFormDto.RolId;
                existingRolForm.FormId = rolFormDto.FormId;
                existingRolForm.Permission = rolFormDto.Permission;

                await _repository.UpdateAsync(existingRolForm);
                return MapToDTO(existingRolForm);
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) // Podría ser violación de FK
            {
                 _logger.LogError(dbEx, "Error de base de datos al actualizar la relación rol-formulario con ID {RolFormId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación rol-formulario con ID {id}. Verifique la existencia de Rol y Form.", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar la relación rol-formulario con ID {RolFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación rol-formulario con ID {id}", ex);
            }
        }

        // Método para actualizar parcialmente una relación rol-formulario (PATCH)
        // Principalmente para actualizar Permission
        public async Task<RolFormDto> PatchAsync(int id, RolFormDto rolFormDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó aplicar patch a una relación rol-formulario con ID inválido: {RolFormId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación rol-formulario debe ser mayor que cero.");
            }

            try
            {
                var existingRolForm = await _repository.GetByIdAsync(id);
                if (existingRolForm == null)
                {
                    _logger.LogInformation("No se encontró la relación rol-formulario con ID {RolFormId} para aplicar patch", id);
                    throw new EntityNotFoundException("RolForm", id);
                }

                bool updated = false;

                // Actualizar Permission si se proporciona y es diferente
                if (!string.IsNullOrWhiteSpace(rolFormDto.Permission) && rolFormDto.Permission != existingRolForm.Permission)
                {
                    // Aquí también se podría validar el valor de Permission si hay un conjunto específico permitido
                    existingRolForm.Permission = rolFormDto.Permission;
                    updated = true;
                }

                // No actualizamos RolId o FormId en PATCH

                if (updated)
                {
                    await _repository.UpdateAsync(existingRolForm);
                }
                else
                {
                     _logger.LogInformation("No se realizaron cambios en la relación rol-formulario con ID {RolFormId} durante el patch.", id);
                }

                return MapToDTO(existingRolForm);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                 _logger.LogError(dbEx, "Error de base de datos al aplicar patch a la relación rol-formulario con ID {RolFormId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la relación rol-formulario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al aplicar patch a la relación rol-formulario con ID {RolFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la relación rol-formulario con ID {id}", ex);
            }
        }

        // Método para eliminar una relación rol-formulario (DELETE persistente)
        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar una relación rol-formulario con ID inválido: {RolFormId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación rol-formulario debe ser mayor a 0");
            }
            try
            {
                 var existingRolForm = await _repository.GetByIdAsync(id); // Verificar existencia
                if (existingRolForm == null)
                {
                     _logger.LogInformation("No se encontró la relación rol-formulario con ID {RolFormId} para eliminar", id);
                    throw new EntityNotFoundException("RolForm", id);
                }

                bool deleted = await _repository.DeleteAsync(id);
                if (!deleted)
                {
                     _logger.LogWarning("No se pudo eliminar la relación rol-formulario con ID {RolFormId}.", id);
                    throw new EntityNotFoundException("RolForm", id); 
                }
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
             catch (Exception ex)
            {
                 _logger.LogError(ex,"Error general al eliminar la relación rol-formulario con ID {RolFormId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar la relación rol-formulario con ID {id}", ex);
            }
        }

        // Método para desactivar (eliminar lógicamente) una relación rol-formulario
        public async Task SoftDeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó realizar soft-delete a una relación rol-formulario con ID inválido: {RolFormId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación rol-formulario debe ser mayor a 0");
            }

            try
            {
                var existingRolForm = await _repository.GetByIdAsync(id);
                if (existingRolForm == null)
                {
                    _logger.LogInformation("No se encontró la relación rol-formulario con ID {RolFormId} para soft-delete", id);
                    throw new EntityNotFoundException("RolForm", id);
                }

                
                await _repository.SoftDeleteAsync(id); 
                _logger.LogInformation("Relación rol-formulario con ID {RolFormId} desactivada (soft-delete) exitosamente", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al realizar soft-delete de la relación rol-formulario con ID {RolFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la relación rol-formulario con ID {id}", dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al realizar soft-delete de la relación rol-formulario con ID {RolFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar la relación rol-formulario con ID {id}", ex);
            }
        }
        
        // Implementación del método ActivateAsync requerido por la interfaz
        public async Task ActivateAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó activar una relación rol-formulario con ID inválido: {RolFormId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la relación rol-formulario debe ser mayor a 0");
            }

            try
            {
                var existingRolForm = await _repository.GetByIdAsync(id);
                if (existingRolForm == null)
                {
                    _logger.LogInformation("No se encontró la relación rol-formulario con ID {RolFormId} para activar", id);
                    throw new EntityNotFoundException("RolForm", id);
                }

                await _repository.ActivateAsync(id);
                _logger.LogInformation("Relación rol-formulario con ID {RolFormId} activada exitosamente", id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar la relación rol-formulario con ID {RolFormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al activar la relación rol-formulario con ID {id}", ex);
            }
        }        // Métodos específicos de RolFormBusiness que utilizan la interfaz IRolFormRepository
        
        /// <summary>
        /// Obtiene todos los roles de formulario para un rol específico
        /// </summary>
        public async Task<IEnumerable<RolFormDto>> GetByRolIdAsync(int rolId)
        {
            if (rolId <= 0)
            {
                _logger.LogWarning("Se intentó obtener roles de formulario para un rol con ID inválido: {RolId}", rolId);
                throw new ValidationException("rolId", "El ID del rol debe ser mayor que cero");
            }
            
            try
            {
                var rolForms = await _rolFormRepository.GetByRolIdAsync(rolId);
                return MapToDTOList(rolForms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles de formulario para el rol con ID: {RolId}", rolId);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar los roles de formulario para el rol con ID {rolId}", ex);
            }
        }
        
        /// <summary>
        /// Obtiene todos los roles de formulario para un formulario específico
        /// </summary>
        public async Task<IEnumerable<RolFormDto>> GetByFormIdAsync(int formId)
        {
            if (formId <= 0)
            {
                _logger.LogWarning("Se intentó obtener roles de formulario para un formulario con ID inválido: {FormId}", formId);
                throw new ValidationException("formId", "El ID del formulario debe ser mayor que cero");
            }
            
            try
            {
                var rolForms = await _rolFormRepository.GetByFormIdAsync(formId);
                return MapToDTOList(rolForms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles de formulario para el formulario con ID: {FormId}", formId);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar los roles de formulario para el formulario con ID {formId}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRolForm(RolFormDto rolFormDto)
        {
            if (rolFormDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto RolForm no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(rolFormDto.Permission))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol de formulario con permiso vacío");
                throw new Utilities.Exceptions.ValidationException("permission", "El permiso del rol de formulario es obligatorio");
            }
        }        //Funciones de mapeos 
        // Método para mapear de RolForm a RolFormDto
        private RolFormDto MapToDTO(RolForm rolForm)
        {
            return _mappingService.Map<RolForm, RolFormDto>(rolForm);
        }

        // Método para mapear de RolFormDto a RolForm
        private RolForm MapToEntity(RolFormDto rolFormDto)
        {
            return _mappingService.Map<RolFormDto, RolForm>(rolFormDto);
        }        // Método para mapear una lista de RolForm a una lista de RolFormDto
        private IEnumerable<RolFormDto> MapToDTOList(IEnumerable<RolForm> rolForms)
        {
            // En lugar de usar MapCollectionToDto que tiene restricciones de tipo,
            // mapeamos cada elemento individualmente
            List<RolFormDto> rolFormsDTO = new List<RolFormDto>();
            foreach (var rolForm in rolForms)
            {
                rolFormsDTO.Add(MapToDTO(rolForm));
            }
            return rolFormsDTO;
        }
    }
}

