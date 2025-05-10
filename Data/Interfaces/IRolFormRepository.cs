using Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    /// <summary>
    /// Interfaz específica para el repositorio de RolForm
    /// </summary>
    public interface IRolFormRepository : IGenericRepository<RolForm, int>
    {
        /// <summary>
        /// Obtiene todos los formularios asociados a un rol
        /// </summary>
        /// <param name="rolId">ID del rol</param>
        /// <returns>Lista de RolForm para el rol especificado</returns>
        Task<IEnumerable<RolForm>> GetByRolIdAsync(int rolId);

        /// <summary>
        /// Obtiene todos los roles asociados a un formulario
        /// </summary>
        /// <param name="formId">ID del formulario</param>
        /// <returns>Lista de RolForm para el formulario especificado</returns>
        Task<IEnumerable<RolForm>> GetByFormIdAsync(int formId);
    }
}
