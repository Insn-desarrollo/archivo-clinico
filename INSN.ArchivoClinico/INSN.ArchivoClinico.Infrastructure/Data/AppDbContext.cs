using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INSN.ArchivoClinico.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace INSN.ArchivoClinico.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<AtencionDto> Atenciones { get; set; }
        public DbSet<Diagnostico> Diagnosticos { get; set; }
        public DbSet<Examen> Examenes { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Configuración adicional del contexto...
    }
}
