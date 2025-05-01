using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    /// <summary>
    /// DTO para actualizar la contraseña de un usuario
    /// </summary>
    public class UpdatePasswordDto
    {
        /// <summary>
        /// La nueva contraseña del usuario
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;
    }
}