using System.ComponentModel.DataAnnotations;

namespace ProyectoHostalMVC.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        public int IdReserva { get; set; }

        [Required]
        [Range(0.01, 100000, ErrorMessage = "Ingrese un monto válido")]
        public decimal Monto { get; set; }

        [Required]
        public string Metodo { get; set; } = "Efectivo";

        [Required]
        public string Tipo { get; set; } = "Adelanto";

        [Display(Name = "Número de operación")]
        public string? NumeroOperacion { get; set; }

        public DateTime FechaPago { get; set; }
        public string? NombreCliente { get; set; }
        public string? NumeroHabitacion { get; set; }
    }

    public class PagoPorMetodo
    {
        public string Metodo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
}
