namespace ProyectoHostalMVC.Models
{
    public class Dashboard
    {
        public int LlegadasHoy { get; set; }
        public int SalidasHoy { get; set; }
        public int HuespedesAlojados { get; set; }
        public int HabitacionesLimpieza { get; set; }
        public int TotalHabitaciones { get; set; }
        public int HabitacionesDisponibles { get; set; }
        public int HabitacionesOcupadas { get; set; }
        public int HabitacionesMantenimiento { get; set; }
        public decimal CobradoHoy { get; set; }

        public List<DashboardMovimiento> MovimientosHoy { get; set; } = new();
        public List<DashboardPagoPendiente> PagosPendientes { get; set; } = new();
    }

    public class DashboardMovimiento
    {
        public int IdReserva { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Huesped { get; set; } = string.Empty;
        public string Habitacion { get; set; } = string.Empty;
        public DateTime FechaProgramada { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class DashboardPagoPendiente
    {
        public int IdReserva { get; set; }
        public string Huesped { get; set; } = string.Empty;
        public string Habitacion { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
    }
}
