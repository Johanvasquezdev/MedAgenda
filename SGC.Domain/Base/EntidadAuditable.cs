using System;

namespace SGC.Domain.Base
{
    public abstract class EntidadAuditable : EntidadBase
    {
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}