using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entidades
{
    public class Paciente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="El Apellidos es Requerido")]
        [StringLength(60, MinimumLength = 1, ErrorMessage ="El Apellidos debe ser Minimo 1 Maximo 60" )]
        public string Apellidos { get; set; }


        [Required(ErrorMessage = "El Nombres es Requerido")]
        [StringLength(60, MinimumLength = 1, ErrorMessage = "Los Nombres debe ser Minimo 1 Maximo 60 ")]
        public string Nombres { get; set; }


        [Required(ErrorMessage = "La Direccion es Requerido")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "La Direccion  debe ser Minimo 1 Maximo 100 ")]
        public string Direccion { get; set; }


        [StringLength(40, MinimumLength = 1, ErrorMessage = "El Telefono debe ser Minimo 1 Maximo 40 ")]
        public string Telefono { get; set; }


        [Required(ErrorMessage = "El Generdo es Requerido")]
        public char Genero { get; set; }

        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        [Display(Name ="Creado Por")]
        public int? CreadoPorId { get; set; }

        [ForeignKey("CreadoPorId")]
        public UsuarioAplicacion CreadoPor { get; set; }

        [Display(Name = "Actualizado Por")]
        public int? ActualizadoPorId { get; set; }

        [ForeignKey("ActualizadoPorId")]
        public UsuarioAplicacion ActualizadoPor { get; set; }

        public HistoriaClinica HistoriaClinica { get; set; }
    }
}
