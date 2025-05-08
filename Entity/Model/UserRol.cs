using Entity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class UserRol : IEntity, IActivable, IAuditable
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        public int RolId { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Propiedades de navegación - cambiamos la inicialización
        // para evitar problemas de referencia circular
        public User? User { get; set; }
        public Rol? Rol { get; set; }
    }
}
