using Entity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class UserRol : IActivable
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        public int RolId { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Propiedades de navegación
        public User User { get; set; } = new User();
        public Rol Rol { get; set; } = new Rol();
    }
}
