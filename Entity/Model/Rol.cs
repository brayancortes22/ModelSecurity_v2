using Entity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Rol : IEntity, IActivable, IAuditable
    {
        public int Id { get; set; }
        public string TypeRol { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Propiedades de navegación
        public ICollection<UserRol> UserRols { get; set; }
        public ICollection<RolForm> RolForms { get; set; }
    }
}
