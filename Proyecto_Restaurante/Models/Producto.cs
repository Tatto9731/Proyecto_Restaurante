using System;
using System.Collections.Generic;

namespace Proyecto_Restaurante.Models;

public partial class Producto
{
    public int ProductoId { get; set; }

    public string? Nombre { get; set; }

    public decimal? Precio { get; set; }

    public int? CategoriaId { get; set; }

    public virtual Categorium? Categoria { get; set; }

    public virtual ICollection<ProductoOrden> ProductoOrdens { get; set; } = new List<ProductoOrden>();
}
