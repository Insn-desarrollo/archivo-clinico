using System;

namespace INSN.ArchivoClinico.Models
{
    public class ConsultaMedica
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Motivo { get; set; }
        public string Diagnostico { get; set; }
        public string Tratamiento { get; set; }

        // Relación con la clase Paciente (Navegación inversa)
        public int PacienteId { get; set; }
        public virtual Paciente Paciente { get; set; }
    }

    public class InformacionAtencion
    {
        public int NumTriaje { get; set; }
        public string NroCuenta { get; set; }
        public string HistoriaClinica { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombres { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Servicio { get; set; }
        public string Medico { get; set; }
        public int Estado { get; set; } // Estado para el semáforo
    }

}