namespace ProyectoHostalMVC.Models
{
    public class ReservaDetalleViewModel
    {
        public Reserva Reserva { get; set; } = new();
        public List<Pago> Pagos { get; set; } = new();
    }
}
