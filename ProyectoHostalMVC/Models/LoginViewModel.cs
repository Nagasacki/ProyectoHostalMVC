using System.ComponentModel.DataAnnotations;

namespace ProyectoHostalMVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingrese su correo electrónico")]
        [EmailAddress(ErrorMessage = "Formato de correo no válido")]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese su contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Clave { get; set; } = string.Empty;

        [Display(Name = "Recordar sesión")]
        public bool Recordarme { get; set; } = false;
    }
}