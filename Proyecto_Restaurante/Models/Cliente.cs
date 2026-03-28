using System;
using System.Collections.Generic;

namespace Proyecto_Restaurante.Models;

public partial class Cliente
{
    public int ClienteId { get; set; }

    public string? Nombre { get; set; }

    public string? Identificacion { get; set; }

    public string? CorreoElectronico { get; set; }

    public string? Telefono { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
