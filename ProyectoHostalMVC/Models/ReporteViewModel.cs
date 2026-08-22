namespace ProyectoHostalMVC.Models
{
    public class ReporteViewModel
    {
        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public ReporteResumen Resumen { get; set; }
            = new ReporteResumen();

        public List<ReporteReserva> Reservas { get; set; }
            = new List<ReporteReserva>();

        public PaginacionRespuesta<ReporteReserva> ReservasPaginadas { get; set; }
            = new PaginacionRespuesta<ReporteReserva>();

        public List<Habitacion> Habitaciones { get; set; }
            = new List<Habitacion>();

        public List<PagoPorMetodo> PagosPorMetodo { get; set; }
            = new List<PagoPorMetodo>();
    }
}
