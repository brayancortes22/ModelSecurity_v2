using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class ChangeLogDto
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public int IdTable { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string Action { get; set; }
        public bool Active { get; set; }
        public string UserName { get; set; }
    }
}
