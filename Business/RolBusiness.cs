using Data;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using System.ComponentModel.Design;
using Entity.DTOautogestion;


namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los roles del sistema.
    /// </summary>
    public class RolBusiness
    {
        private readonly RolData _rolData;
        private readonly ILogger<RolBusiness> _logger;

        public RolBusiness(RolData rolData, ILogger<RolBusiness> logger)
        {
            _rolData = rolData;
            _logger = logger;
        }
        
        // Método para obtener todos los roles como DTOs
        public async Task<IEnumerable<RolDto>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _rolData.GetAllAsync();
                //var rolesDTO = new List<RolDto>();
                return MapToDTOList(roles);

                //foreach (var rol in roles)
                //{
                //    rolesDTO.Add(new RolDto
                //    {
                //        Id = rol.Id,
                //        TypeRol = rol.TypeRol,
                //        Description = rol.Description,
                //        Active = rol.Active //si existe la entidad
                //    });
                //}
                //return rolesDTO;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los rolez");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }
        // Método para obtener un rol por su ID como DTO
        public async Task<RolDto> GetRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con un ID invalido: {RolId}",id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor a 0");
            }
            try
            {
                var rol = await _rolData.GetByidAsync(id);
                if (rol == null)
                {
                    _logger.LogInformation("No se encontró el rol con ID {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }
                //return new RolDto
                //{
                //    Id = rol.Id,
                //    TypeRol = rol.TypeRol,
                //    Description = rol.Description,
                //    Active = rol.Active //si existe la entidad
                //};
                return MapToDTO(rol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error al obtener el rol con ID {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

       
        // Método para crear un rol desde un DTO
        public async Task<RolDto> CreateRolAsync(RolDto RolDto)
        {
            try
            {
                ValidateRol(RolDto);
                var rol = MapToEntity(RolDto);
                //var rol = new Rol
                //{
                //    Id = RolDto.Id,
                //    TypeRol = RolDto.TypeRol,
                //    Description = RolDto.Description,
                //    Active = RolDto.Active //si existe la entidad
                //};

                var rolCreado = await _rolData.CreateAsync(rol);
                return MapToDTO(rolCreado);
                //return new RolDto
                //{
                //    Id = rol.Id,
                //    TypeRol = rol.TypeRol,
                //    Description = rol.Description,
                //    Active = rol.Active //si existe la entidad

                //};
              }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo rol: {RolNombre}", RolDto?.TypeRol?? "null");
                throw new ExternalServiceException("Base de datos", $"Error al crear el rol", ex);
            }
        }

        // Método para actualizar un rol existente
        public async Task<RolDto> UpdateRolAsync(int id, RolDto rolDto)
        {
            if (id <= 0 || id != rolDto.Id)
            {
                 _logger.LogWarning("Se intentó actualizar un rol con un ID invalido o no coincidente: {RolId}, DTO ID: {DtoId}", id, rolDto.Id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO.");
            }
             ValidateRol(rolDto); // Reutiliza la validación existente

            try
            {
                var existingRol = await _rolData.GetByidAsync(id);
                if (existingRol == null)
                {
                    _logger.LogInformation("No se encontró el rol con ID {RolId} para actualizar", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                // Mapea los cambios del DTO a la entidad existente
                 existingRol.TypeRol = rolDto.TypeRol;
                 existingRol.Description = rolDto.Description;
                 existingRol.Active = rolDto.Active; // Actualiza también el estado activo si se incluye en el DTO

                var updatedRol = await _rolData.UpdateAsync(existingRol);
                // UpdateAsync devuelve bool, mapeamos la entidad que ya modificamos
                return MapToDTO(existingRol);
            }
            catch (EntityNotFoundException) // Reapropagar si no se encontró
            {
                throw;
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error al actualizar el rol con ID {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el rol con ID {id}", ex);
            }
        }

         // Método para actualizar parcialmente un rol (implementado como actualización completa)
        public async Task<RolDto> PatchRolAsync(int id, RolDto rolDto)
        {
             // Nota: Esta implementación de PATCH es idéntica a PUT.
             // Una implementación más específica de PATCH requeriría lógica adicional
             // para aplicar solo los campos presentes en el DTO.
             if (id <= 0 || id != rolDto.Id) // Asume que el DTO de patch también contiene el ID para consistencia
             {
                 _logger.LogWarning("Se intentó aplicar patch a un rol con un ID invalido o no coincidente: {RolId}, DTO ID: {DtoId}", id, rolDto.Id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID proporcionado es inválido o no coincide con el ID del DTO para PATCH.");
             }
             // Podrías tener una validación específica para Patch si fuera necesario
             // ValidateRolPatch(rolDto);
             ValidateRol(rolDto); // Usando la validación completa por ahora

            try
            {
                 var existingRol = await _rolData.GetByidAsync(id);
                 if (existingRol == null)
                 {
                     _logger.LogInformation("No se encontró el rol con ID {RolId} para aplicar patch", id);
                     throw new EntityNotFoundException("Rol", id);
                 }

                 // Actualiza solo si los valores en el DTO no son nulos o vacíos (Ejemplo básico de Patch)
                 // Una implementación real de PATCH podría ser más compleja.
                 if (!string.IsNullOrWhiteSpace(rolDto.TypeRol))
                     existingRol.TypeRol = rolDto.TypeRol;
                 if (rolDto.Description != null) // Permite borrar la descripción enviando ""
                     existingRol.Description = rolDto.Description;
                 // Considera si Active debe ser actualizable via PATCH y cómo manejarlo
                 existingRol.Active = rolDto.Active;


                await _rolData.UpdateAsync(existingRol); // Asume que no devuelve la entidad (devuelve Task o Task<bool>)
                 return MapToDTO(existingRol); // Mapea la entidad que modificamos localmente
            }
             catch (EntityNotFoundException) // Reapropagar si no se encontró
             {
                 throw;
             }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error al aplicar patch al rol con ID {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al aplicar patch al rol con ID {id}", ex);
            }
        }


        // Método para eliminar un rol (persistente)
        public async Task DeleteRolAsync(int id)
        {
            if (id <= 0)
            {
                 _logger.LogWarning("Se intentó eliminar un rol con un ID invalido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor a 0");
            }
            try
            {
                var existingRol = await _rolData.GetByidAsync(id);
                 if (existingRol == null)
                 {
                    // Decisión: Lanzar excepción o simplemente registrar y retornar.
                    // Lanzar excepción es más común para indicar que el recurso a eliminar no existe.
                     _logger.LogInformation("No se encontró el rol con ID {RolId} para eliminar", id);
                     throw new EntityNotFoundException("Rol", id);
                 }

                await _rolData.DeleteAsync(id); // Pasa el ID como espera el método
                _logger.LogInformation("Rol con ID {RolId} eliminado exitosamente (persistente)", id);
            }
            catch (EntityNotFoundException) // Reapropagar si no se encontró
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error al eliminar el rol con ID {RolId}", id);
                 throw new ExternalServiceException("Base de datos", $"Error al eliminar el rol con ID {id}", ex);
            }
        }

        // Método para eliminar lógicamente un rol (soft delete)
        public async Task SoftDeleteRolAsync(int id)
        {
             if (id <= 0)
             {
                 _logger.LogWarning("Se intentó realizar soft-delete a un rol con un ID invalido: {RolId}", id);
                 throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor a 0");
             }
            try
            {
                 var rolToDeactivate = await _rolData.GetByidAsync(id);
                 if (rolToDeactivate == null)
                 {
                     _logger.LogInformation("No se encontró el rol con ID {RolId} para desactivar (soft-delete)", id);
                     throw new EntityNotFoundException("Rol", id);
                 }

                 if (!rolToDeactivate.Active)
                 {
                    _logger.LogInformation("El rol con ID {RolId} ya está inactivo.", id);
                    // Decisión: ¿Lanzar excepción, retornar algún indicador o simplemente no hacer nada?
                    // Por ahora, no hacemos nada si ya está inactivo.
                    return;
                 }

                rolToDeactivate.Active = false; // Marcar como inactivo
                await _rolData.UpdateAsync(rolToDeactivate); // Persistir el cambio (asume que no devuelve la entidad)
                _logger.LogInformation("Rol con ID {RolId} marcado como inactivo (soft-delete)", id);

            }
            catch (EntityNotFoundException) // Reapropagar si no se encontró
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar soft-delete del rol con ID {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al desactivar el rol con ID {id}", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRol(RolDto RolDto)
        {
            if (RolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException( "El objeto rol no puede ser nulo");
            }
            if (string.IsNullOrWhiteSpace(RolDto.TypeRol))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con nombre vacio");
                throw new Utilities.Exceptions.ValidationException("Name", "El nombre del rol nes obligatorio");
            }
        }
        //Funciones de mapeos 
        // Método para mapear de Rol a RolDTO
        private RolDto MapToDTO(Rol rol)
        {
            return new RolDto
            {
                Id = rol.Id,
                TypeRol = rol.TypeRol,
                Description = rol.Description,
                Active = rol.Active //si existe la entidad
            };

        }
        // Método para mapear de RolDTO a Rol
        private Rol MapToEntity(RolDto rolDTO)
        {
            return new Rol
            {
                Id = rolDTO.Id,
                TypeRol = rolDTO.TypeRol,
                Description = rolDTO.Description,
                Active = rolDTO.Active //si existe la entidad
            };
        }

        // Método para mapear una lista de Rol a una lista de RolDTO
        private IEnumerable<RolDto> MapToDTOList(IEnumerable<Rol> roles)
        {
            var rolesDTO = new List<RolDto>();
            foreach (var rol in roles)
            {
                rolesDTO.Add(MapToDTO(rol));
            }
            return rolesDTO;
        }

    }
}
