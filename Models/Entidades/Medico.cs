using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entidades
{
    public class Medico
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Apellidos Requeridos")]
        public string Apellidos {get; set;}

        [Required(ErrorMessage = "Nombres Requeridos")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Direccion Requerida")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "La direccion debe ser minimo 1 Maximo 40 caracteres")]
        public string Direccion { get; set; }


        //[MaxLength(40)]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "La direccion debe ser minimo 1 Maximo 100 caracteres")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El Genero es  Requerido")]
        public char Genero { get; set; }

        [Required(ErrorMessage ="Especialidad Requerida")]
        public int EspecialidadId { get; set; }

        [ForeignKey("EspecialidadId")]
        public Especialidad Especialidad { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }

    }
}
