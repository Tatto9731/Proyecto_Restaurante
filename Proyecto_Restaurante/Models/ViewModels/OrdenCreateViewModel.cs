using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Restaurante.Models.ViewModels
{
    public class OrdenCreateViewModel
    {
        [Display(Name = "Mesa")]
        public int? MesaId { get; set; }

        [Display(Name = "Salonero")]
        public int? SaloneroId { get; set; }

        [Display(Name = "Fecha")]
        public DateTime? Fecha { get; set; } = DateTime.Now;

        public List<OrdenDetalleItemViewModel> Detalles { get; set; } = new();
    }

    public class OrdenDetalleItemViewModel
    {
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }
}