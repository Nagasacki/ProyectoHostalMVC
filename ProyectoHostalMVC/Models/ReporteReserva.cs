namespace ProyectoHostalMVC.Models
{
    public class ReporteReserva
    {
        public int IdReserva { get; set; }
        public string? NombreCliente { get; set; }

        public string? NumeroHabitacion { get; set; }

        public string? TipoHabitacion { get; set; }

        public DateTime FechaEntrada { get; set; }

        public DateTime FechaSalida { get; set; }

        public int CantidadDias { get; set; }

        public decimal PrecioDia { get; set; }

        public decimal Total { get; set; }

        public string? Estado { get; set; }
    }
}