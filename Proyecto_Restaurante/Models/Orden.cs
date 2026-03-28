using System;
using System.Collections.Generic;

namespace Proyecto_Restaurante.Models;

public partial class Orden
{
    public int OrdenId { get; set; }

    public int? MesaId { get; set; }

    public int? SaloneroId { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual Mesa? Mesa { get; set; }

    public virtual ICollection<ProductoOrden> ProductoOrdens { get; set; } = new List<ProductoOrden>();

    public virtual Salonero? Salonero { get; set; }
}
