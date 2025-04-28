using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Enum
{
    public enum Permission
    {
        [Display(Name = "Create")]
        CreateNeighbourhoods = 1,

        [Display(Name = "Read")]
        ReadNeighbourhoods = 2,

        [Display(Name = "Update")]
        UpdateNeighbourhoods = 3,

        [Display(Name = "Delete")]
        DeleteNeighbourhoods = 4,
    }
}