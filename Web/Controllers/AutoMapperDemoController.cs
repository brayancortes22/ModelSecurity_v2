using Business;
using Business.Mappers;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para demostrar el uso de AutoMapper
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AutoMapperDemoController : ControllerBase
    {
        private readonly AutoMapperFormBusiness _autoMapperFormBusiness;
        private readonly AutoMapperModuleBusiness _autoMapperModuleBusiness;
        private readonly AutoMapperPersonBusiness _autoMapperPersonBusiness;
        private readonly AutoMapperRolBusiness _autoMapperRolBusiness;
        private readonly AutoMapperUserBusiness _autoMapperUserBusiness;
        private readonly IMappingService _mappingService;

        public AutoMapperDemoController(
            AutoMapperFormBusiness autoMapperFormBusiness,
            AutoMapperModuleBusiness autoMapperModuleBusiness,
            AutoMapperPersonBusiness autoMapperPersonBusiness,
            AutoMapperRolBusiness autoMapperRolBusiness,
            AutoMapperUserBusiness autoMapperUserBusiness,
            IMappingService mappingService)
        {
            _autoMapperFormBusiness = autoMapperFormBusiness;
            _autoMapperModuleBusiness = autoMapperModuleBusiness;
            _autoMapperPersonBusiness = autoMapperPersonBusiness;
            _autoMapperRolBusiness = autoMapperRolBusiness;
            _autoMapperUserBusiness = autoMapperUserBusiness;
            _mappingService = mappingService;
        }

        /// <summary>
        /// Obtiene todos los formularios usando AutoMapper
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllForms()
        {
            var forms = await _autoMapperFormBusiness.GetAllAsync();
            return Ok(forms);
        }

        /// <summary>
        /// Obtiene un formulario específico usando AutoMapper
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFormById(int id)
        {
            var form = await _autoMapperFormBusiness.GetByIdAsync(id);
            return Ok(form);
        }

        /// <summary>
        /// Crea un nuevo formulario usando AutoMapper
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(FormDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateForm([FromBody] FormDto formDto)
        {
            var createdForm = await _autoMapperFormBusiness.CreateAsync(formDto);
            return CreatedAtAction(nameof(GetFormById), new { id = createdForm.Id }, createdForm);
        }

        /// <summary>
        /// Actualiza un formulario existente usando AutoMapper
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(FormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateForm(int id, [FromBody] FormDto formDto)
        {
            var updatedForm = await _autoMapperFormBusiness.UpdateAsync(id, formDto);
            return Ok(updatedForm);
        }

        /// <summary>
        /// Actualiza parcialmente un formulario usando AutoMapper
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(FormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PatchForm(int id, [FromBody] FormDto formDto)
        {
            var patchedForm = await _autoMapperFormBusiness.PatchAsync(id, formDto);
            return Ok(patchedForm);
        }

        /// <summary>
        /// Elimina un formulario usando AutoMapper
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]        public async Task<IActionResult> DeleteForm(int id)
        {
            await _autoMapperFormBusiness.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Demuestra el mapeo directo entre entidades y DTOs
        /// </summary>
        [HttpGet("mapping-demo")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public IActionResult DemoMappingService()
        {
            // Crear algunos objetos para demostrar el mapeo
            var person = new Person
            {
                Id = 1,
                Name = "Demo",
                FirstName = "John",
                SecondName = "Robert",
                FirstLastName = "Doe",
                SecondLastName = "Smith",
                Email = "john.doe@example.com",
                PhoneNumber = "123456789",
                TypeIdentification = "DNI",
                NumberIdentification = 12345678,
                Signing = "JD",
                Active = true,
                CreateDate = System.DateTime.Now
            };

            var module = new Entity.Model.Module
            {
                Id = 1,
                Name = "Admin Module",
                Description = "Administration module for system settings",
                Active = true,
                CreateDate = System.DateTime.Now
            };

            var rol = new Rol
            {
                Id = 1,
                TypeRol = "Administrator",
                Description = "Full access to all system features",
                Active = true,
                CreateDate = System.DateTime.Now
            };

            // Realizar mapeos usando el servicio de mapeo
            var personDto = _mappingService.Map<Person, PersonDto>(person);
            var moduleDto = _mappingService.Map<Entity.Model.Module, ModuleDto>(module);
            var rolDto = _mappingService.Map<Rol, RolDto>(rol);

            // Mapeo inverso de DTO a entidad
            var personDtoToUpdate = new PersonDto
            {
                Id = 1,
                Name = "Updated Name",
                FirstName = "Jane",
                SecondName = "Maria",
                FirstLastName = "Doe",
                SecondLastName = "Johnson",
                Email = "jane.doe@example.com",
                PhoneNumber = "987654321",
                TypeIdentification = "Passport",
                NumberIdentification = 87654321,
                Signing = "JJ",
                Active = true
            };

            var updatedPerson = _mappingService.Map<PersonDto, Person>(personDtoToUpdate);

            // Devolver los resultados para que se puedan verificar
            return Ok(new
            {
                OriginalPerson = person,
                MappedPersonDto = personDto,
                OriginalModule = module,
                MappedModuleDto = moduleDto,
                OriginalRol = rol,
                MappedRolDto = rolDto,
                PersonDtoToUpdate = personDtoToUpdate,
                UpdatedPerson = updatedPerson
            });
        }
    }
}
