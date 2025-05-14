using Entity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{    public class FormModule : IEntity
    {
        public int Id { get; set; }
        public string StatusProcedure { get; set; }
        public int FormId { get; set; }
        public int ModuleId { get; set; }
        

        // Propiedades de navegación
        public Module Module { get; set; }
        public Form Form { get; set; }
    }
}
