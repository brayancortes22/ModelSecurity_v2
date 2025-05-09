﻿using Entity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Person : IEntity, IActivable, IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FirstLastName { get; set; }
        public string SecondLastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string TypeIdentification { get; set; }
        public int NumberIdentification { get; set; }
        public string Signing { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Propiedad de navegación
        public User User { get; set; }
    }
}
