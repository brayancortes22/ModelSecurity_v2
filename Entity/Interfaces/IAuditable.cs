using System;

namespace Entity.Interfaces
{
    /// <summary>
    /// Interfaz para entidades que tienen campos de auditoría
    /// </summary>
    public interface IAuditable
    {
        DateTime CreateDate { get; set; }
        DateTime? UpdateDate { get; set; }
        DateTime? DeleteDate { get; set; }
    }
}