using System.ComponentModel.DataAnnotations;

namespace ProyectoHostalMVC.Models
{
    public class Habitacion
    {
        public int IdHabitacion { get; set; }

        [Required(ErrorMessage = "El número de habitación es obligatorio")]
        [StringLength(10)]
        [Display(Name = "Número de Habitación")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de habitación es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Tipo de Habitación")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(1, 10000, ErrorMessage = "El precio debe ser mayor a 0")]
        [Display(Name = "Precio por Noche (S/)")]
        public decimal Precio { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Disponible";
    }
}