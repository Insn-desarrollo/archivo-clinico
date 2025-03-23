using System;
using System.ComponentModel.DataAnnotations;

namespace INSN.ArchivoClinico.Models
{
    public class Paciente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        // Otras propiedades...

        // Relaciones con otras clases (ejemplo)
        public virtual ICollection<ConsultaMedica> ConsultasMedicas { get; set; }
    }
}