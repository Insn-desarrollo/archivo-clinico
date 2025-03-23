using System.ComponentModel.DataAnnotations;

namespace INSN.ArchivoClinico.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usuario")]
        public required string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Clave")]
        public required string Clave { get; set; }
    }

    public class UsuarioPagModel
    {
        public string? musu_codi { get; set; }
        public string? musu_nom { get; set; }
    }
}
