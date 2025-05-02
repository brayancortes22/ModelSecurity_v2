using Business.Interfaces;
using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los Roles del sistema.
    /// </summary>
    public class RolBusiness
    {
        private readonly RolData _RolData;
        private readonly ILogger<RolBusiness> _logger;
        private readonly RolFormData _rolFormData; // Añadido para acceder a los formularios por rol

        public RolBusiness(RolData RolData, RolFormData rolFormData, ILogger<RolBusiness> logger)
        {
            _RolData = RolData;
            _rolFormData = rolFormData;
            _logger = logger;
        }
        
        // Método para obtener todos los Roles como DTOs
        public async Task<IEnumerable<RolDto>> GetAllRolesAsync()
        {
            try
            {
                var Roles = await _RolData.GetAllAsync();
                //var RolesDTO = new List<RolDto>();
                return MapToDTOList(Roles);

                //foreach (var Rol in Roles)
                //{
                //    RolesDTO.Add(new RolDto
                //    {
                //        Id = Rol.Id,
                //        TypeRol = Rol.TypeRol,
                //        Description = Rol.Description,
                //        Active = Rol.Active //si existe la entidad
                //    });
                //}
                //return RolesDTO;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Rolez");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de Roles", ex);
            }
        }
        // Método para obtener un Rol por su ID como DTO
        public async Task<RolDto> GetRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un Rol con un ID invalido: {RolId}",id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del Rol debe ser mayor a 0");
            }
            try
            {
                var Rol = await _RolData.GetByIdAsync(id);
                if (Rol == null)
                {
                    _logger.LogInformation("No se encontró el Rol con ID {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }
                //return new RolDto
                //{
                //    Id = Rol.Id,
                //    TypeRol = Rol.TypeRol,
                //    Description = Rol.Description,
                //    Active = Rol.Active //si existe la entidad
                //};
                return MapToDTO(Rol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error al obtener el Rol con ID {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el Rol con ID {id}", ex);
            }
        }

       
        // Método para crear un Rol desde un DTO
        public async Task<RolDto> CreateRolAsync(RolDto RolDto)
        {
            try
            {
                ValidateRol(RolDto);
                var Rol = MapToEntity(RolDto);
                //var Rol = new Rol
                //{
                //    Id = RolDto.Id,
                //    TypeRol = RolDto.TypeRol,
                //    Description = RolDto.Description,
                //    Active = RolDto.Active //si existe la entidad
                //};

                var RolCreado = await _RolData.CreateAsync(Rol);
                return MapToDTO(RolCreado);
                //return new RolDto
                //{
                //    Id = Rol.Id,
                //    TypeRol = Rol.TypeRol,
                //    Description = Rol.Description,
                //    Active = Rol.Active //si existe la entidad

                //};
              }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo Rol: {RolNombre}", RolDto?.TypeRol?? "null");
                throw new ExternalServiceException("Base de datos", $"Error al crear el Rol", ex);
            }
        }

        // Método para actualizar un Rol existente
        public async Task<RolDto> UpdateRolAsync(int id, RolDto RolDto)
        {
            if (id <= 0 || id != RolDto.Id)
            {
                 _logger.LogWarning("Se intentó actualizar un Rol con un ID invalido o no coincidente: {RolId}, DTO ID: {DtoId}", id, RolDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
             ValidateRol(RolDto); // Reutiliza la validación existente

            try
            {
                var existingRol = await _RolData.GetByIdAsync(id);
                if (existingRol == null)
                {
                    _logger.LogInformation("No se encontró el Rol con ID {RolId} para actualizar", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                // Mapea los cambios del DTO a la entidad existente
                 existingRol.TypeRol = RolDto.TypeRol;
                 existingRol.Description = RolDto.Description;
                 existingRol.Active = RolDto.Active; // Actualiza también el estado activo si se incluye en el DTO

                var updatedRol = await _RolData.UpdateAsync(existingRol);
                // UpdateAsync devuelve bool, mapeamos la entidad que ya modificamos
                return MapToDTO(existingRol);
            }
            catch (EntityNotFoundException) // Reapropagar si no se encontró
            {
                throw;
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error al actualizar el Rol con ID {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el Rol con ID {id}", ex);
            }
        }

         // Método para actualizar parcialmente un Rol (implementado como actualización completa)
        public async Task<RolDto> PatchRolAsync(int id, RolDto RolDto)
        {
             // Nota: Esta implementación de PATCH es idéntica a PUT.
             // Una implementación más específica de PATCH requeriría lógica adicional
             // para aplicar solo los campos presentes en el DTO.
             if (id <= 0 || id != RolDto.Id) // Asume que el DTO de patch también contiene el ID para consistencia
             {
                 _logger.LogWarning("Se intentó aplicar patch a un Rol con un ID invalido o no coincidente: {RolId}, DTO ID: {DtoId}", id, RolDto.Id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO para PATCH.");
             }
             // Podrías tener una validación específica para Patch si fuera necesario
             // ValidateRolPatch(RolDto);
             ValidateRol(RolDto); // Usando la validación completa por ahora

            try
            {
                 var existingRol = await _RolData.GetByIdAsync(id);
                 if (existingRol == null)
                 {
                     _logger.LogInformation("No se encontró el Rol con ID {RolId} para aplicar patch", id);
                     throw new EntityNotFoundException("Rol", id);
                 }

                 // Actualiza solo si los valores en el DTO no son nulos o vacíos (Ejemplo básico de Patch)
                 // Una implementación real de PATCH podría ser más compleja.
                 if (!string.IsNullOrWhiteSpace(RolDto.TypeRol))
                     existingRol.TypeRol = RolDto.TypeRol;
                 if (RolDto.Description != null) // Permite borrar la descripción enviando ""
                     existingRol.Description = RolDto.Description;
                 // Considera si Active debe ser actualizable via PATCH y cómo manejarlo
                 existingRol.Active = RolDto.Active;

                await _RolData.UpdateAsync(existingRol); // Asume que no devuelve la entidad (devuelve Task o Task<bool>)
                 return MapToDTO(existingRol); // Mapea la entidad que modificamos localmente
            }
             catch (EntityNotFoundException) // Reapropagar si no se encontró
             {
                 throw;
             }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error al aplicar patch al Rol con ID {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch al Rol con ID {id}", ex);
            }
        }

        // Método para eliminar un Rol (persistente)
        public async Task DeleteRolAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar un Rol con un ID invalido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del Rol debe ser mayor a 0");
            }
            try
            {
                var existingRol = await _RolData.GetByIdAsync(id);
                 if (existingRol == null)
                 {
                    // Decisión: Lanzar excepción o simplemente registrar y retornar.
                    // Lanzar excepción es más común para indicar que el recurso a eliminar no existe.
                     _logger.LogInformation("No se encontró el Rol con ID {RolId} para eliminar", id);
                     throw new EntityNotFoundException("Rol", id);
                 }

                await _RolData.DeleteAsync(id); // Pasa el ID como espera el método
                _logger.LogInformation("Rol con ID {RolId} eliminado exitosamente (persistente)", id);
            }
            catch (EntityNotFoundException) // Reapropagar si no se encontró
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error al eliminar el Rol con ID {RolId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar el Rol con ID {id}", ex);
            }
        }

        // Método para eliminar lógicamente un Rol (soft delete)
        public async Task SoftDeleteRolAsync(int id)
        {
             if (id <= 0)
             {
                 _logger.LogWarning("Se intentó realizar soft-delete a un Rol con un ID invalido: {RolId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del Rol debe ser mayor a 0");
             }
            try
            {
                 var RolToDeactivate = await _RolData.GetByIdAsync(id);
                 if (RolToDeactivate == null)
                 {
                     _logger.LogInformation("No se encontró el Rol con ID {RolId} para desactivar (soft-delete)", id);
                     throw new EntityNotFoundException("Rol", id);
                 }

                 if (!RolToDeactivate.Active)
                 {
                    _logger.LogInformation("El Rol con ID {RolId} ya está inactivo.", id);
                    // Decisión: ¿Lanzar excepción, retornar algún indicador o simplemente no hacer nada?
                    // Por ahora, no hacemos nada si ya está inactivo.
                    return;
                 }

                RolToDeactivate.Active = false; // Marcar como inactivo
                await _RolData.UpdateAsync(RolToDeactivate); // Persistir el cambio (asume que no devuelve la entidad)
                _logger.LogInformation("Rol con ID {RolId} marcado como inactivo (soft-delete)", id);

            }
            catch (EntityNotFoundException) // Reapropagar si no se encontró
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar soft-delete del Rol con ID {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar el Rol con ID {id}", ex);
            }
        }

         // Método para activar un módulo (restaurar)
        public async Task ActivateRolAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó activar un usuario con un ID invalido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor a 0");
            }
            try
            {
                var RolToActivate = await _RolData.GetByIdAsync(id);
                if (RolToActivate == null)
                {
                    _logger.LogInformation("No se encontró la Rola con ID {RolId} para activar", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                if (RolToActivate.Active)
                {
                    _logger.LogInformation("La Rola con ID {RolId} ya está activa.", id);
                    return;
                }

                RolToActivate.Active = true;
                // Considerar limpiar DeleteDate y actualizar UpdateDate si existen
                // RolToActivate.DeleteDate = null;
                // RolToActivate.UpdateDate = DateTime.UtcNow;
                await _RolData.UpdateAsync(RolToActivate);

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

        /// <summary>
        /// Obtiene todos los formularios asignados a un rol específico
        /// </summary>
        /// <param name="rolId">ID del rol</param>
        /// <returns>Lista de formularios asignados al rol</returns>
        public async Task<IEnumerable<FormDto>> GetFormsByRolIdAsync(int rolId)
        {
            if (rolId <= 0)
            {
                _logger.LogWarning("Se intentó obtener formularios para un rol con ID inválido: {RolId}", rolId);
                throw new Utilities.Exceptions.ValidationException("rolId", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                // Verificar que el rol existe
                var rol = await _RolData.GetByIdAsync(rolId);
                if (rol == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RolId}", rolId);
                    throw new EntityNotFoundException("rol", rolId);
                }

                // Obtener los formularios asignados al rol utilizando RolFormData
                // Esto requiere inyectar el servicio RolFormData en el constructor
                var formDtos = await _rolFormData.GetFormsByRolIdAsync(rolId);
                
                return formDtos;
            }
            catch (EntityNotFoundException)
            {
                throw; // Relanzar para mantener el mensaje específico
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los formularios para el rol con ID: {RolId}", rolId);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar los formularios para el rol con ID {rolId}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRol(RolDto RolDto)
        {
            if (RolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException( "El objeto Rol no puede ser nulo");
            }
            if (string.IsNullOrWhiteSpace(RolDto.TypeRol))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Rol con nombre vacio");
                throw new Utilities.Exceptions.ValidationException("Name", "El nombre del Rol nes obligatorio");
            }
        }
        //Funciones de mapeos 
        // Método para mapear de Rol a RolDTO
        private RolDto MapToDTO(Rol Rol)
        {
            return new RolDto
            {
                Id = Rol.Id,
                TypeRol = Rol.TypeRol,
                Description = Rol.Description,
                Active = Rol.Active //si existe la entidad
            };

        }
        // Método para mapear de RolDTO a Rol
        private Rol MapToEntity(RolDto RolDTO)
        {
            return new Rol
            {
                Id = RolDTO.Id,
                TypeRol = RolDTO.TypeRol,
                Description = RolDTO.Description,
                Active = RolDTO.Active //si existe la entidad
            };
        }

        // Método para mapear una lista de Rol a una lista de RolDTO
        private IEnumerable<RolDto> MapToDTOList(IEnumerable<Rol> Roles)
        {
            var RolesDTO = new List<RolDto>();
            foreach (var Rol in Roles)
            {
                RolesDTO.Add(MapToDTO(Rol));
            }
            return RolesDTO;
        }

    }
}
