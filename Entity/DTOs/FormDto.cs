﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class FormDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Route { get; set; } // Permitir valores nulos
        public string Cuestion { get; set; }
        public string TypeCuestion { get; set; }
        public string Answer { get; set; }
        public bool Active { get; set; }
    }
}
