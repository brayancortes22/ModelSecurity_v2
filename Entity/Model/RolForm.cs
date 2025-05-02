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
        
        // Propiedades de navegación - removidas las inicializaciones predeterminadas
        public Form Form { get; set; }
        public Rol Rol { get; set; }
    }
}
