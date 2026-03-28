using System;
using System.Collections.Generic;

namespace Proyecto_Restaurante.Models;

public partial class Reserva
{
    public int ReservaId { get; set; }

    public int? ClienteId { get; set; }

    public int? MesaId { get; set; }

    public DateTime? Fecha { get; set; }

    public int? CantidadPersonas { get; set; }

    public string? Estado { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Mesa? Mesa { get; set; }
}
