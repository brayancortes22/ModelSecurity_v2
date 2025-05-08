using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Interfaces
{
    /// <summary>
    /// Interfaz para entidades que pueden activarse o desactivarse
    /// </summary>
    public interface IActivable
    {
        bool Active { get; set; }
    }
}