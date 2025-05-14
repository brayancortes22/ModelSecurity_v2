using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{    public class FormDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Route { get; set; } // Permitir valores nulos
        public string Cuestion { get; set; } = string.Empty;
        public string TypeCuestion { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public bool Active { get; set; }
    }
}
