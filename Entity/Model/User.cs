using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public int PersonId { get; set; }

        // Propiedades de navegación
        public Person Person { get; set; }
        public ICollection<UserRol> UserRols { get; set; }
        public ICollection<UserSede> UserSedes { get; set; }
        public Aprendiz Aprendiz { get; set; }
        public Instructor Instructor { get; set; }
    }
}