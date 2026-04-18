using System;
using System.Collections.Generic;

namespace Proyecto_Restaurante.Models;

public partial class Mesa
{
    public int MesaId { get; set; }
    public int? NumeroMesa { get; set; }
    public int? Capacidad { get; set; }

    public string? Estado { get; set; } = "Disponible";

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
