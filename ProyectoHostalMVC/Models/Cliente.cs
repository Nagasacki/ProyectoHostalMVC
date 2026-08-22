using System.ComponentModel.DataAnnotations;

namespace ProyectoHostalMVC.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El documento es obligatorio")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "Ingrese un documento válido")]
        [Display(Name = "Número de documento")]
        public string Dni { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tipo de documento")]
        public string TipoDocumento { get; set; } = "DNI";

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(15)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo no válido")]
        [StringLength(100)]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Nacionalidad { get; set; } = "Peruana";

        [StringLength(150)]
        public string? Direccion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [StringLength(100)]
        [Display(Name = "Contacto de emergencia")]
        public string? ContactoEmergencia { get; set; }

        [StringLength(250)]
        public string? Observaciones { get; set; }
    }
}
