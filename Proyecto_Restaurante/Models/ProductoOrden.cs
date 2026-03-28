using System;
using System.Collections.Generic;

namespace Proyecto_Restaurante.Models;

public partial class ProductoOrden
{
    public int ProductoOrdenId { get; set; }

    public int? OrdenId { get; set; }

    public int? ProductoId { get; set; }

    public int? Cantidad { get; set; }

    public decimal? Precio { get; set; }

    public virtual Orden? Orden { get; set; }

    public virtual Producto? Producto { get; set; }
}
