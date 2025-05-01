using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string Username { get; set; }= string.Empty;
        public string Email { get; set; }= string.Empty;
        public string Password { get; set; }= string.Empty;
        public int PersonId { get; set; }
    }
}
