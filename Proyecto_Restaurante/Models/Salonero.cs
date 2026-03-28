using System;
using System.Collections.Generic;

namespace Proyecto_Restaurante.Models;

public partial class Salonero
{
    public int SaloneroId { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();
}
