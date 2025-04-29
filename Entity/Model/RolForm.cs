using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class RolForm
    {
        public int Id { get; set; }
        public string Permission { get; set; }
        public int RolId { get; set; }
        public int FormId { get; set; }
       
       // Propiedades de navegación
        public Form Form { get; set; } = new Form();
        public Rol Rol { get; set; } = new Rol();
    }
}
