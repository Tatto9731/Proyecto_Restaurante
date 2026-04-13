using System;
using System.Collections.Generic;

namespace Proyecto_Restaurante.Models;

public partial class Factura
{
    public int FacturaId { get; set; }

    public int? OrdenId { get; set; }
    public decimal? Subtotal { get; set; }

    public decimal? Total { get; set; }

    public decimal? Impuestos { get; set; }

    public string? MetodoPago { get; set; }

    public string? NumeroFactura { get; set; }

    public virtual Orden? Orden { get; set; }
}
