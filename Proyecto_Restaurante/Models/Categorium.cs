using System;
using System.Collections.Generic;

namespace Proyecto_Restaurante.Models;

public partial class Categorium
{
    public int CategoriaId { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
