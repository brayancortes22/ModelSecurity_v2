using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class PersonDto
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FirstLastName { get; set; }
        public string SecondLastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string TypeIdentification { get; set; }
        public int NumberIdentification { get; set; }
        public string Signig { get; set; }
        public bool status { get; set; }

    }
}
