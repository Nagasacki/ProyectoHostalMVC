namespace ProyectoHostalMVC.Models
{
    public class ReporteResumen
    {
        public int TotalReservas { get; set; }

        public int ReservasFinalizadas { get; set; }

        public int ReservasCanceladas { get; set; }

        public decimal TotalIngresos { get; set; }
    }
}