using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion.pivote
{
    public class RolFormDto
    {
        public int Id { get; set; }
        public string Permission { get; set; }
        public int RolId { get; set; }
        public int FormId { get; set; }
    }
}
