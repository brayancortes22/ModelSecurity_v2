using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class FormModule
    {
        public int Id { get; set; }
        public string StatusProcedure { get; set; }
        public int FormId { get; set; }
        public int ModuleId { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }

        // Propiedades de navegación
        public Module Module { get; set; }
        public Form Form { get; set; }
    }
}
