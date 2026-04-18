using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Restaurante.Models.ViewModels
{
    public class OrdenEditViewModel
    {
        public int OrdenId { get; set; }

        [Display(Name = "Mesa")]
        public int? MesaId { get; set; }

        [Display(Name = "Salonero")]
        public int? SaloneroId { get; set; }

        [Display(Name = "Fecha")]
        public DateTime? Fecha { get; set; }

        public List<OrdenDetalleEditItemViewModel> Detalles { get; set; } = new();
    }

    public class OrdenDetalleEditItemViewModel
    {
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }
}