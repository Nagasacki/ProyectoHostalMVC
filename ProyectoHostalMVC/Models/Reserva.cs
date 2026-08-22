using System.ComponentModel.DataAnnotations;

namespace ProyectoHostalMVC.Models
{
    public class Reserva
    {
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un huésped / cliente")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una habitación")]
        [Display(Name = "Habitación")]
        public int IdHabitacion { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Entrada")]
        public DateTime FechaEntrada { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La fecha de salida es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Salida")]
        public DateTime FechaSalida { get; set; } = DateTime.Today.AddDays(1);

        [Display(Name = "Días")]
        public int CantidadDias { get; set; } = 1;

        [Display(Name = "Precio por Día (S/)")]
        public decimal PrecioDia { get; set; }

        [Display(Name = "Monto Total (S/)")]
        public decimal Total { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Confirmada";

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Display(Name = "Check-in real")]
        public DateTime? FechaCheckInReal { get; set; }

        [Display(Name = "Check-out real")]
        public DateTime? FechaCheckOutReal { get; set; }

        [Display(Name = "Monto pagado")]
        public decimal MontoPagado { get; set; }

        [Display(Name = "Saldo pendiente")]
        public decimal Saldo => Math.Max(0, Total - MontoPagado);

        // Propiedades de navegación / JOIN para vistas
        public string? NombreCliente { get; set; }
        public string? NumeroHabitacion { get; set; }
        public string? TipoHabitacion { get; set; }
    }
}
