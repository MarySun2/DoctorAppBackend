using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class RegistroDto
    {
        [Required(ErrorMessage = "El Usuario es requerido")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La Contraseña es requerido")]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "El password debe ser Minimo 4 Maximo 10 Caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Apellidos es Requerido")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "Nombre es Requerido")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "El Email es Requerido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El Rol es Requerido")]
        public string Rol { get; set; }
    }
}
